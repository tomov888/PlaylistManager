using System.Threading.Tasks;
using System;
using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Contracts.Models.UseCases;
using PlaylistManager.Core.Contracts.UseCase;
using PlaylistManager.Infrastructure.Api.Contracts;

namespace PlaylistManager.Infrastructure.Api.Serverless.AzureFunctions.QueueConsumers;

public class YoutubeTrackDownloadQueueConsumerFunction
{
	private readonly ILogger<YoutubeTrackDownloadQueueConsumerFunction> _logger;
	private readonly IYoutubeTrackDownloadUseCase _useCase;

	public YoutubeTrackDownloadQueueConsumerFunction(ILogger<YoutubeTrackDownloadQueueConsumerFunction> logger, IYoutubeTrackDownloadUseCase useCase)
	{
		_logger = logger;
		_useCase = useCase;
	}

	[FunctionName(nameof(YoutubeTrackDownloadQueueConsumerFunction))]
	public async Task RunAsync([QueueTrigger("youtube-track-download-queue", Connection = "AzureWebJobsStorage")] YoutubeTrackDownloadQueueMessage message)
	{

		_logger.LogWarning($"[{nameof(YoutubeTrackDownloadQueueConsumerFunction)}] => Received message: {JsonSerializer.Serialize(message)}");
		
		var request = new AddTrackFromYoutubeRequest
		{
			Artist = message.Artist,
			Tags = message.Tags,
			TrackName = message.TrackName,
			UserEmail = message.UserEmail,
			YoutubeUrl = message.YoutubeUrl
		};

		var result = await _useCase.DownloadTrackFromYoutubeAsync(request);

		result
			.OnSuccessResult(x => _logger.LogWarning($"[{nameof(YoutubeTrackDownloadQueueConsumerFunction)}] => Successfully downloaded youtube track: {request.YoutubeUrl}"))
			.OnFailureResult(x => _logger.LogWarning($"[{nameof(YoutubeTrackDownloadQueueConsumerFunction)}] => Failed to download youtube track: {request.YoutubeUrl} because of: {x.Message}."));
	}
}