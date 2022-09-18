using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Contracts.UseCase;

public interface IDeletePlaylistUseCase
{
	Task<Playlist> DeletePlaylistAsync(string userEmail, string playlistId);
}