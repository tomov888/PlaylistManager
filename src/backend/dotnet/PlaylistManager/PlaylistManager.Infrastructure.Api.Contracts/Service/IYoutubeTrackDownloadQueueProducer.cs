using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Contracts.Models.UseCases;

namespace PlaylistManager.Infrastructure.Api.Contracts.Service;

public interface IYoutubeTrackDownloadQueueProducer
{
	Task<OperationResult<AddTrackFromYoutubeRequest>> EnqueueYoutubeTrackDownloadRequestAsync(AddTrackFromYoutubeRequest request);
}
