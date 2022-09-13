using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Contracts.Models.UseCases;
using PlaylistManager.Core.Contracts.Models.UseCases.YoutubeTrackDownload;

namespace PlaylistManager.Core.Contracts.Service;

public interface IFileSystemYoutubeVideoDownloader
{
	Task<OperationResult<YoutubeVideoInfo>> DownloadYoutubeVideoAsync(AddTrackFromYoutubeRequest request, string videoDownloadDestination);
}
