using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediaToolkit;
using MediaToolkit.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PlaylistManager.Api.Serverless.AzureFunctions.Middleware;
using PlaylistManager.Core.Common.Extensions.FunctionalExtensions.OperationResultExtensions;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Common.Utils;
using PlaylistManager.Core.Contracts.Models.UseCases.VideoDownload;
using PlaylistManager.Core.Contracts.Repository;
using PlaylistManager.Core.Domain.Models;
using PlaylistManager.Infrastructure.Api.Contracts.Dto.Request;
using PlaylistManager.Infrastructure.Repository.AzureBlobStorage;
using VideoLibrary;

namespace PlaylistManager.Infrastructure.Api.Serverless.AzureFunctions.TrackEndpoints;

public class AddTrackFromYoutubeEndpointFunction
{
	private readonly AzureBlobStorageFileRepository _blobStorageFileRepository;
	private readonly ILogger<GetTracksEndpointFunction> _logger;
	private readonly AzureFunctionsHttpMiddlewarePipelineFactory _middlewarePipeline;
	private readonly ITrackRepository _trackRepository;

	public AddTrackFromYoutubeEndpointFunction(
		ILogger<GetTracksEndpointFunction> logger,
		AzureFunctionsHttpMiddlewarePipelineFactory middlewarePipeline,
		AzureBlobStorageFileRepository blobStorageFileRepository,
		ITrackRepository trackRepository)
	{
		_logger = logger;
		_middlewarePipeline = middlewarePipeline;
		_blobStorageFileRepository = blobStorageFileRepository;
		_trackRepository = trackRepository;
	}

	[FunctionName(nameof(AddTrackFromYoutubeEndpointFunction))]
	public async Task<IActionResult> AddTrackFromYoutubeVideo(
		[HttpTrigger(AuthorizationLevel.Function, "post", Route = "tracks/add-youtube-track")]
		HttpRequest req,
		ExecutionContext context)
	{
		var requestBody = string.Empty;
		using var streamReader = new StreamReader(req.Body);
		requestBody = await streamReader.ReadToEndAsync();
		var data = JsonConvert.DeserializeObject<AddTrackFromYoutubeRequestDto>(requestBody);

		var result = await Process(data, "test@test.com");

		return result
			.OnSuccess(x => (IActionResult)new OkObjectResult(x))
			.OnFailure(x => new BadRequestObjectResult(x.Message))
			.Finally();
	}

	public async Task<OperationResult<Track>> Process(AddTrackFromYoutubeRequestDto requestDto, string userEmail)
	{
		var tempFolder = Path.GetTempPath();
		var trackFullName = $"{requestDto.Artist}-{requestDto.TrackName}";
		var trackId = TrackId.New(trackFullName);
		var videoDownloadDestination = Path.Combine(tempFolder, trackId);
		var trackDownloadDestination = Path.Combine(tempFolder, $"{trackId}.mp3");
		var blobStorageFilePath = Path.Combine(userEmail, Path.GetFileName(trackDownloadDestination));

		try
		{
			var videoDownloadResult = await DownloadYoutubeVideoAsync(requestDto, videoDownloadDestination);
			var downloadedVideoInfo = videoDownloadResult.TryUnwrap();
			
			ConvertVideoToMp3(trackDownloadDestination, videoDownloadDestination);

			var uploadResult = await UploadTrackFileToBlobStorageAsync(trackDownloadDestination, blobStorageFilePath);
			var trackFileUrl = uploadResult.TryUnwrap().AbsoluteUri;
			
			var track = await AddTrackAsync(
				userEmail, 
				trackId, 
				trackFullName,
				requestDto.Artist, 
				requestDto.TrackName, 
				requestDto.Tags, 
				trackFileUrl,
				downloadedVideoInfo.LengthInSeconds 
			);

			return OperationResult<Track>.Success(track);
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Downloading video: {requestDto.YoutubeUrl} failed. ");
			
			File.Delete(videoDownloadDestination);
			File.Delete(trackDownloadDestination);
			await _trackRepository.DeleteAsync(userEmail, trackId);
			
			return OperationResult<Track>.Failure(ex);
		}
		finally
		{
			_logger.LogWarning($"Deleting: {videoDownloadDestination}");
			File.Delete(videoDownloadDestination);
			_logger.LogWarning($"Deleting: {trackDownloadDestination}");
			File.Delete(trackDownloadDestination);
		}
	}

	private async Task<Track> AddTrackAsync(
		string userEmail,
		string trackId,
		string trackFullName,
		string artist, 
		string trackName, 
		List<string> tags,
		string trackFileUrl, 
		int durationInSeconds)
	{
		var track = new Track
		{
			Artist = artist,
			Name = trackName,
			Description = trackFullName,
			Id = trackId,
			UserEmail = userEmail,
			Tags = tags,
			DurationInSeconds = durationInSeconds,
			ReleasedAtUtc = DateTime.UtcNow,
			CreatedAtUtc = DateTime.UtcNow,
			UpdatedAtUtc = DateTime.UtcNow,
			FileUrl = trackFileUrl
		};
		await _trackRepository.InsertAsync(track);
		return track;
	}

	private async Task<OperationResult<Uri>> UploadTrackFileToBlobStorageAsync(string trackDownloadDestination, string blobStorageFilePath)
	{
		_logger.LogWarning($"Uploading track: {trackDownloadDestination} to blob storage destination: {blobStorageFilePath}");
		var uploadResult = await _blobStorageFileRepository.UploadFileAsync(trackDownloadDestination, blobStorageFilePath);
		_logger.LogWarning($"Uploading track: {trackDownloadDestination} to blob storage done.");
		return uploadResult;
	}

	private void ConvertVideoToMp3(string trackDownloadDestination, string videoDownloadDestination)
	{
		_logger.LogWarning($"Converting video to track: {trackDownloadDestination}");
		var inputFile = new MediaFile { Filename = videoDownloadDestination };
		var outputFile = new MediaFile { Filename = trackDownloadDestination };
		using (var engine = new Engine())
		{
			engine.GetMetadata(inputFile);
			engine.Convert(inputFile, outputFile);
		}
		_logger.LogWarning($"Video conversion finished. Track destination: {trackDownloadDestination}");
	}

	private async Task<OperationResult<YoutubeVideoInfo>> DownloadYoutubeVideoAsync(AddTrackFromYoutubeRequestDto requestDto, string videoDownloadDestination)
	{
		try
		{
			var youtubeVideo = await YouTube.Default.GetVideoAsync(requestDto.YoutubeUrl);
			_logger.LogWarning($"Downloading Video: {requestDto.YoutubeUrl} to destination: {videoDownloadDestination}");
			var videoBytes = await youtubeVideo.GetBytesAsync();
			await File.WriteAllBytesAsync(videoDownloadDestination, videoBytes);
			_logger.LogWarning($"Video: {requestDto.YoutubeUrl} downloaded");

			var videoInfo =  new YoutubeVideoInfo
			(
				youtubeVideo.Title,
				youtubeVideo.Info.LengthSeconds ?? -1,
				youtubeVideo.ContentLength ?? -1,
				youtubeVideo.Uri
			);

			return OperationResult<YoutubeVideoInfo>.Success(videoInfo);
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex,$"Failed to download Video: {requestDto.YoutubeUrl}.");
			return OperationResult<YoutubeVideoInfo>.Failure(ex);
		}
	}
}