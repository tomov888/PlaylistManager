using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Contracts.Repository;
using PlaylistManager.Core.Contracts.UseCase;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Services.UseCases;

public class DeletePlaylistUseCase: IDeletePlaylistUseCase
{
	private readonly ILogger<DeletePlaylistUseCase> _logger;
	private readonly IPlaylistRepository _playlistRepository;

	public DeletePlaylistUseCase(ILogger<DeletePlaylistUseCase> logger, IPlaylistRepository playlistRepository)
	{
		_logger = logger;
		_playlistRepository = playlistRepository;
	}

	public async Task<Playlist> DeletePlaylistAsync(string userEmail, string playlistId)
	{
		var deletedPlaylist = await _playlistRepository.DeletePlaylistAsync(userEmail, playlistId);

		return deletedPlaylist;
	}
}