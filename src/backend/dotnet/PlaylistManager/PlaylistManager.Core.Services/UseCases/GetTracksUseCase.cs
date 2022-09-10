using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Contracts.Repository;
using PlaylistManager.Core.Contracts.UseCase;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Services.UseCases;

public class GetTracksUseCase : IGetTracksUseCase
{
	private readonly ILogger<GetTracksUseCase> _logger;
	private readonly ITrackRepository _repository;

	public GetTracksUseCase(ILogger<GetTracksUseCase> logger, ITrackRepository repository)
	{
		_logger = logger;
		_repository = repository;
	}

	public async Task<List<Track>> GetTracksAsync(string userEmail)
	{
		var tracks = await _repository.GetTracksAsync(userEmail);
		return tracks;
	}
}