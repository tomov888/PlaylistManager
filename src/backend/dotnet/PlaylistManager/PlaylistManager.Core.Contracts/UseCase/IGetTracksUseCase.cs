using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Contracts.UseCase;

public interface IGetTracksUseCase
{
	Task<List<Track>> GetTracksAsync(string userEmail);
}