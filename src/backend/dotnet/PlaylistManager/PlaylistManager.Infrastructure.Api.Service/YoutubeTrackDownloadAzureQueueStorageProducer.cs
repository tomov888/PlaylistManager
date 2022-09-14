using System.Text;
using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Contracts.Models.UseCases;
using PlaylistManager.Infrastructure.Api.Contracts;
using PlaylistManager.Infrastructure.Api.Contracts.Service;
namespace PlaylistManager.Infrastructure.Api.Service;

public class YoutubeTrackDownloadAzureQueueStorageProducer : IYoutubeTrackDownloadQueueProducer
{
	private readonly QueueClient _queueClient;

	public YoutubeTrackDownloadAzureQueueStorageProducer()
	{
		_queueClient = new QueueClient("UseDevelopmentStorage=true","youtube-track-download-queue", new QueueClientOptions()
		{
			MessageEncoding = QueueMessageEncoding.Base64
		});
	}

	public async Task<OperationResult<AddTrackFromYoutubeRequest>> EnqueueYoutubeTrackDownloadRequestAsync(AddTrackFromYoutubeRequest request)
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
}