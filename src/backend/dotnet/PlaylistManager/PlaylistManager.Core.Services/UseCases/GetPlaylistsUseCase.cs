using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Contracts.Repository;
using PlaylistManager.Core.Contracts.UseCase;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Services.UseCases;

public class GetPlaylistsUseCase: IGetPlaylistsUseCase
{
	private readonly ILogger<GetPlaylistsUseCase> _logger;
	private readonly IPlaylistRepository _playlistRepository;

	public GetPlaylistsUseCase(ILogger<GetPlaylistsUseCase> logger, IPlaylistRepository playlistRepository)
	{
		_logger = logger;
		_playlistRepository = playlistRepository;
	}

	public async Task<List<Playlist>> GetPlaylists(string userEmail)
	{
		return await _playlistRepository.GetPlaylistsAsync(userEmail);
	}
}