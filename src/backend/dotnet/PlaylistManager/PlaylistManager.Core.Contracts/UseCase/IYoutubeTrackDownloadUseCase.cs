using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Contracts.Models.UseCases;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Contracts.UseCase;

public interface IYoutubeTrackDownloadUseCase
{
	Task<OperationResult<Track>> DownloadTrackFromYoutubeAsync(AddTrackFromYoutubeRequest request);
}
