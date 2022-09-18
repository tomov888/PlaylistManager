using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Contracts.Repository;

public interface IPlaylistRepository
{
	Task<Playlist> AddPlaylistAsync(Playlist model);
	Task<List<Playlist>> GetPlaylistsAsync(string userEmail);
	Task<Playlist> DeletePlaylistAsync(string userEmail, string playlistId);
}
