using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Common.Models;

namespace PlaylistManager.Infrastructure.Repository.AzureBlobStorage;

public class AzureBlobStorageFileRepository
{
	protected readonly ILogger _logger;
	protected BlobContainerClient _containerClient;
	

	public AzureBlobStorageFileRepository(ILogger logger, BlobContainerClient containerClient)
	{
		_logger = logger;
		_containerClient = containerClient;
	}

	public async Task<OperationResult<Uri>> UploadFileAsync(string filePath)
	{
		try
		{
			var blobClient = _containerClient.GetBlobClient($"{Path.GetFileName(filePath)}"); 

			using (var fileStream = File.OpenRead(filePath))
			{
				await blobClient.UploadAsync(fileStream);
			}

			return OperationResult<Uri>.Success(blobClient.Uri);
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Failed to upload file: {filePath} to blob: {Path.GetFileName(filePath)}.");
			return OperationResult<Uri>.Failure(ex);
		}
	}

	public async Task<OperationResult<Uri>> UploadFileAsync(string filePath, string blobPath)
	{
		try
		{
			var blobClient = _containerClient.GetBlobClient(blobPath); 

			using (var fileStream = File.OpenRead(filePath))
			{
				await blobClient.UploadAsync(fileStream);
			}

			return OperationResult<Uri>.Success(blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.MaxValue));
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Failed to upload file: {filePath} to blob: {Path.GetFileName(filePath)}.");
			return OperationResult<Uri>.Failure(ex);
		}
	}
	
	public async Task<OperationResult<Uri>> UploadFileAsync(Stream fileStream, string blobPath)
	{
		try
		{
			var blobClient = _containerClient.GetBlobClient(blobPath);
			await blobClient.UploadAsync(fileStream);
		
			return OperationResult<Uri>.Success(blobClient.Uri);
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Failed to upload stream to blob: {blobPath}.");
			return OperationResult<Uri>.Failure(ex);
		}
	}

	public async Task DownloadFileAsync(string blobPath)
	{
		var blobClient = _containerClient.GetBlobClient(blobPath);
		using (var fileStream = new MemoryStream()) 
		{
			await blobClient.DownloadToAsync(fileStream);
			fileStream.Position = 0;
			Console.WriteLine($"Downloaded {fileStream.Length} bytes");
		}
	}
}