using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Contracts.Repository;
using PlaylistManager.Core.Domain.Models;
using PlaylistManager.Infrastructure.Repository.AzureTableStorage.Entity;

namespace PlaylistManager.Infrastructure.Repository.AzureTableStorage.EntityRepository;

public class PlaylistRepository: AzureTableStorageRepository<PlaylistEntity, Playlist>, IPlaylistRepository
{
	public PlaylistRepository(ILogger<PlaylistRepository> logger, TableServiceClient tableServiceClient) : base(logger, tableServiceClient)
	{
		
	}

	protected override Playlist DomainModel (PlaylistEntity entity) => (Playlist)entity;
	protected override PlaylistEntity Entity (Playlist model) => (PlaylistEntity)model;

	public async Task<Playlist> AddPlaylistAsync(Playlist model)
	{
		await InsertAsync(Entity(model));
		return model;
	}

	public async Task<List<Playlist>> GetPlaylistsAsync(string userEmail)
	{
		var playlists = await QueryAsync(x => x.PartitionKey == userEmail);
		return playlists.Select(DomainModel).ToList();
	}

	public async Task<Playlist> DeletePlaylistAsync(string userEmail, string playlistId)
	{
		var playlistOption = await FindOneAsync(userEmail, playlistId);

		if (playlistOption.HasValue) await DeleteAsync(userEmail, playlistId);
		
		return playlistOption.TryUnwrap();
	}
}