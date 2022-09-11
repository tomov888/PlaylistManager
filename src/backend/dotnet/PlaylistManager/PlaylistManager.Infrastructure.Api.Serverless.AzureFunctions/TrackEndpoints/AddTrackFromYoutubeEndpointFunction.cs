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

		var youtubeVideo = await YouTube.Default.GetVideoAsync(data.YoutubeUrl);
		var tempFolder = Path.GetTempPath();
		var trackId = Guid.NewGuid().ToString();
		var videoDownloadDestination = Path.Combine(tempFolder, trackId);
		var trackDownloadDestination = Path.Combine(tempFolder, $"{trackId}.mp3");

		try
		{
			_logger.LogWarning(
				$"Downloading Video: {youtubeVideo.FullName} from url : {data.YoutubeUrl} to destination: {videoDownloadDestination}");

			var videoBytes = await youtubeVideo.GetBytesAsync();

			_logger.LogWarning($"Video: {youtubeVideo.FullName} downloaded");

			await File.WriteAllBytesAsync(videoDownloadDestination, videoBytes);

			_logger.LogWarning($"Video download destination: {videoDownloadDestination}");
			_logger.LogWarning($"Track download destination: {trackDownloadDestination}");

			var inputFile = new MediaFile { Filename = videoDownloadDestination };
			var outputFile = new MediaFile { Filename = trackDownloadDestination };

			using (var engine = new Engine())
			{
				engine.GetMetadata(inputFile);
				engine.Convert(inputFile, outputFile);
			}

			_logger.LogWarning($"Uploading track: {trackDownloadDestination} to blob storage...");
			var uploadResult = await _blobStorageFileRepository.UploadFileAsync(trackDownloadDestination,
				Path.Combine("test@test.com", Path.GetFileName(trackDownloadDestination)));
			_logger.LogWarning($"Uploading track: {trackDownloadDestination} to blob storage done.");

			var track = new Track
			{
				Artist = data.Artist,
				Name = data.TrackName,
				Description = $"{data.Artist}-{data.TrackName}",
				Id = trackId,
				UserEmail = "test@test.com",
				Tags = new List<string>(),
				DurationInSeconds = youtubeVideo.Info.LengthSeconds ?? -1,
				ReleasedAtUtc = DateTime.UtcNow,
				CreatedAtUtc = DateTime.UtcNow,
				UpdatedAtUtc = DateTime.UtcNow,
				FileUrl = uploadResult.Payload.AbsoluteUri
			};

			await _trackRepository.InsertAsync(track);

			return new OkObjectResult(track);
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Downloading video: {youtubeVideo.FullName} failed. ");
			File.Delete(videoDownloadDestination);
			File.Delete(trackDownloadDestination);
			return new BadRequestObjectResult(ex.Message);
		}
		finally
		{
			_logger.LogWarning($"Deleting: {videoDownloadDestination}");
			File.Delete(videoDownloadDestination);
			_logger.LogWarning($"Deleting: {trackDownloadDestination}");
			File.Delete(trackDownloadDestination);
		}


		// var middleware = await _middlewarePipeline.AuthenticatedPipeline<Track>(req, new List<Permission> { Permission.VIEW_SONGS });
		//
		// return await middleware
		// 	.WithExecutingAction((_, payload, session) => _useCase.GetTracksAsync(session.UserEmail))
		// 	.ToIActionResultAsync();	
	}
}