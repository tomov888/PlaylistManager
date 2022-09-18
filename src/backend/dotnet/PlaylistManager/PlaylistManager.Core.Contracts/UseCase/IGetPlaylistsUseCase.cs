using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Contracts.UseCase;

public interface IGetPlaylistsUseCase
{
	Task<List<Playlist>> GetPlaylists(string userEmail);
}