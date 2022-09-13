using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Common.Utils;
using PlaylistManager.Core.Contracts.Models.UseCases;
using PlaylistManager.Core.Contracts.Repository;
using PlaylistManager.Core.Contracts.Service;
using PlaylistManager.Core.Contracts.UseCase;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Infrastructure.Api.Service;

public class AzureStorageYoutubeTrackDownloadUseCase : IYoutubeTrackDownloadUseCase
{
	private readonly ILogger<AzureStorageYoutubeTrackDownloadUseCase> _logger;
	private readonly ITrackRepository _trackRepository;
	private readonly IFileSystemVideoToMp3Converter _converter;
	private readonly IFileSystemYoutubeVideoDownloader _downloader;
	private readonly IFileSystemToBlobStorageTrackUploadService _uploader;

	public AzureStorageYoutubeTrackDownloadUseCase(
		ILogger<AzureStorageYoutubeTrackDownloadUseCase> logger,
		ITrackRepository trackRepository,
		IFileSystemVideoToMp3Converter converter,
		IFileSystemYoutubeVideoDownloader downloader,
		IFileSystemToBlobStorageTrackUploadService uploader)
	{
		_logger = logger;
		_trackRepository = trackRepository;
		_converter = converter;
		_downloader = downloader;
		_uploader = uploader;
	}

	public async Task<OperationResult<Track>> DownloadTrackFromYoutubeAsync(AddTrackFromYoutubeRequest request)
	{
		var tempFolder = Path.GetTempPath();
		var trackFullName = $"{request.Artist}-{request.TrackName}";
		var trackId = TrackId.New(trackFullName);
		var videoDownloadDestination = Path.Combine(tempFolder, trackId);
		var trackDownloadDestination = Path.Combine(tempFolder, $"{trackId}.mp3");
		var blobStorageFilePath = Path.Combine(request.UserEmail, Path.GetFileName(trackDownloadDestination));

		try
		{
			var videoDownloadResult = await _downloader.DownloadYoutubeVideoAsync(request, videoDownloadDestination);
			var downloadedVideoInfo = videoDownloadResult.TryUnwrap();

			var conversionResult = _converter.ConvertVideoToMp3(trackDownloadDestination, videoDownloadDestination);
			var convertedFileInfo = conversionResult.TryUnwrap();

			var uploadResult = await _uploader.UploadTrackFileToBlobStorageAsync(trackDownloadDestination, blobStorageFilePath);
			var trackFileUrl = uploadResult.TryUnwrap().AbsoluteUri;

			var track = await AddTrackAsync(
				request.UserEmail,
				trackId,
				trackFullName,
				request.Artist,
				request.TrackName,
				request.Tags,
				trackFileUrl,
				downloadedVideoInfo.LengthInSeconds,
				convertedFileInfo.SizeInBytes
			);

			return OperationResult<Track>.Success(track);
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Downloading video: {request.YoutubeUrl} failed. ");

			File.Delete(videoDownloadDestination);
			File.Delete(trackDownloadDestination);
			await _trackRepository.DeleteAsync(request.UserEmail, trackId);

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
		int durationInSeconds,
		long sizeInBytes
	)
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
			FileUrl = trackFileUrl,
			SizeInBytes = sizeInBytes
		};
		await _trackRepository.InsertAsync(track);
		return track;
	}
}