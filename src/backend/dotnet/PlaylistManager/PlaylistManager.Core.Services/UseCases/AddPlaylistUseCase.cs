using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Contracts.Models.UseCases;
using PlaylistManager.Core.Contracts.Repository;
using PlaylistManager.Core.Contracts.UseCase;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Services.UseCases;

public class AddPlaylistUseCase : IAddPlaylistUseCase
{
	private readonly ILogger<AddPlaylistUseCase> _logger;
	private readonly IPlaylistRepository _playlistRepository;

	public AddPlaylistUseCase(IPlaylistRepository playlistRepository, ILogger<AddPlaylistUseCase> logger)
	{
		_playlistRepository = playlistRepository;
		_logger = logger;
	}

	public async Task<Playlist> AddPlaylistAsync(AddPlaylistRequest request)
	{
		var playlistToAdd = Playlist.NewPlaylist(request.UserEmail, request.Name, request.Tags);

		var addedPlaylist  = await _playlistRepository.AddPlaylistAsync(playlistToAdd);

		_logger.LogWarning($"[{nameof(AddPlaylistUseCase)}] => Successfully added playlist: {request.Name} for user: {request.UserEmail}");
		
		return addedPlaylist;
	}
}