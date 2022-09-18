using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Contracts.Models.UseCases;
using PlaylistManager.Infrastructure.Api.Contracts;
using PlaylistManager.Infrastructure.Api.Contracts.Service;

namespace PlaylistManager.Infrastructure.Api.Service;

public class YoutubeTrackDownloadAzureQueueStorageProducer : IYoutubeTrackDownloadQueueProducer
{
	private readonly ILogger<YoutubeTrackDownloadAzureQueueStorageProducer> _logger;
	private readonly QueueClient _queueClient;

	public YoutubeTrackDownloadAzureQueueStorageProducer(
		QueueClient client, 
		ILogger<YoutubeTrackDownloadAzureQueueStorageProducer> logger)
	{
		_queueClient = client;
		_logger = logger;
	}

	public async Task<OperationResult<AddTrackFromYoutubeRequest>> EnqueueYoutubeTrackDownloadRequestAsync(AddTrackFromYoutubeRequest request)
	{
		try
		{
			var payload = new YoutubeTrackDownloadQueueMessage
			{
				Artist = request.Artist,
				Tags = request.Tags,
				TrackName = request.TrackName,
				UserEmail = request.UserEmail,
				YoutubeUrl = request.YoutubeUrl
			};

			await _queueClient.SendMessageAsync(JsonSerializer.Serialize(payload));

			return OperationResult<AddTrackFromYoutubeRequest>.Success(request);
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"[{nameof(YoutubeTrackDownloadAzureQueueStorageProducer)}] => Failed to enqueue [{nameof(YoutubeTrackDownloadQueueMessage)}].");
			return OperationResult<AddTrackFromYoutubeRequest>.Failure(ex);
		}
	}
}