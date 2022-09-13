using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Contracts.Models.UseCases.YoutubeTrackDownload;

namespace PlaylistManager.Core.Contracts.Service;

public interface IFileSystemVideoToMp3Converter
{
	OperationResult<VideoToMp3ConversionResult> ConvertVideoToMp3(string trackDownloadDestination, string videoDownloadDestination);
}