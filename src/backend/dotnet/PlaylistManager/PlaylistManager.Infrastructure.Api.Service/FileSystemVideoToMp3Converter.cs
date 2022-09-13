using MediaToolkit;
using MediaToolkit.Model;
using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Contracts.Models.UseCases.YoutubeTrackDownload;
using PlaylistManager.Core.Contracts.Service;

namespace PlaylistManager.Infrastructure.Api.Service;

public class FileSystemVideoToMp3Converter: IFileSystemVideoToMp3Converter
{
	private readonly ILogger<FileSystemVideoToMp3Converter> _logger;

	public FileSystemVideoToMp3Converter(ILogger<FileSystemVideoToMp3Converter> logger)
	{
		_logger = logger;
	}

	public OperationResult<VideoToMp3ConversionResult> ConvertVideoToMp3(string trackDownloadDestination, string videoDownloadDestination)
	{
		try
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

			var result = new VideoToMp3ConversionResult
			{
				Name = trackDownloadDestination,
				SizeInBytes = new FileInfo(trackDownloadDestination).Length
			};
			
			return OperationResult<VideoToMp3ConversionResult>.Success(result);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			return OperationResult<VideoToMp3ConversionResult>.Failure(ex);
		}
	}
}