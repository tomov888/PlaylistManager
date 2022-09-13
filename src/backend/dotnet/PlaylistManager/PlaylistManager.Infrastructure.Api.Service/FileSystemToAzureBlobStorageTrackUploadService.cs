using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Contracts.Service;
using PlaylistManager.Infrastructure.Repository.AzureBlobStorage;

namespace PlaylistManager.Infrastructure.Api.Service;

public class FileSystemToAzureBlobStorageTrackUploadService : IFileSystemToBlobStorageTrackUploadService
{
	private readonly ILogger<FileSystemToAzureBlobStorageTrackUploadService> _logger;
	private readonly AzureBlobStorageFileRepository _blobStorageFileRepository;

	public FileSystemToAzureBlobStorageTrackUploadService(
		ILogger<FileSystemToAzureBlobStorageTrackUploadService> logger, 
		AzureBlobStorageFileRepository blobStorageFileRepository)
	{
		_logger = logger;
		_blobStorageFileRepository = blobStorageFileRepository;
	}


	public async Task<OperationResult<Uri>> UploadTrackFileToBlobStorageAsync(string trackDownloadDestination, string blobStorageFilePath)
	{
		_logger.LogWarning($"Uploading track: {trackDownloadDestination} to blob storage destination: {blobStorageFilePath}");
		var uploadResult = await _blobStorageFileRepository.UploadFileAsync(trackDownloadDestination, blobStorageFilePath);
		_logger.LogWarning($"Uploading track: {trackDownloadDestination} to blob storage done.");
		return uploadResult;
	}
}