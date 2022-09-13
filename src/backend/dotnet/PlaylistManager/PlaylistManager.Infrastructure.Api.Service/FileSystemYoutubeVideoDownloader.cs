using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Contracts.Models.UseCases;
using PlaylistManager.Core.Contracts.Models.UseCases.YoutubeTrackDownload;
using PlaylistManager.Core.Contracts.Service;
using VideoLibrary;

namespace PlaylistManager.Infrastructure.Api.Service;

public class FileSystemYoutubeVideoDownloader : IFileSystemYoutubeVideoDownloader
{
	private readonly ILogger<FileSystemYoutubeVideoDownloader> _logger;

	public FileSystemYoutubeVideoDownloader(ILogger<FileSystemYoutubeVideoDownloader> logger)
	{
		_logger = logger;
	}

	public async Task<OperationResult<YoutubeVideoInfo>> DownloadYoutubeVideoAsync(AddTrackFromYoutubeRequest request, string videoDownloadDestination)
	{
		try
		{
			var youtubeVideo = await YouTube.Default.GetVideoAsync(request.YoutubeUrl);
			_logger.LogWarning($"Downloading Video: {request.YoutubeUrl} to destination: {videoDownloadDestination}");
			var videoBytes = await youtubeVideo.GetBytesAsync();
			await File.WriteAllBytesAsync(videoDownloadDestination, videoBytes);
			_logger.LogWarning($"Video: {request.YoutubeUrl} downloaded");

			var videoInfo = new YoutubeVideoInfo
			(
				youtubeVideo.Title,
				youtubeVideo.Info.LengthSeconds ?? -1,
				youtubeVideo.ContentLength ?? -1,
				youtubeVideo.Uri,
				videoDownloadDestination
			);

			return OperationResult<YoutubeVideoInfo>.Success(videoInfo);
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Failed to download Video: {request.YoutubeUrl}.");
			return OperationResult<YoutubeVideoInfo>.Failure(ex);
		}
	}
}