using PlaylistManager.Core.Common.Models;

namespace PlaylistManager.Core.Contracts.Service;

public interface IFileSystemToBlobStorageTrackUploadService
{
	Task<OperationResult<Uri>> UploadTrackFileToBlobStorageAsync(string trackDownloadDestination, string blobStorageFilePath);
}