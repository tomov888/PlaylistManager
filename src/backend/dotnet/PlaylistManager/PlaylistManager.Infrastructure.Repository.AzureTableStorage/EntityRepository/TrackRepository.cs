using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Contracts.Repository;
using PlaylistManager.Core.Domain.Models;
using PlaylistManager.Infrastructure.Repository.AzureTableStorage.Entity;

namespace PlaylistManager.Infrastructure.Repository.AzureTableStorage.EntityRepository;

public class TrackRepository: AzureTableStorageRepository<TrackEntity, Track>, ITrackRepository
{
	public TrackRepository(ILogger logger, TableServiceClient tableServiceClient) : base(logger, tableServiceClient)
	{
		
	}
	
	protected override Track DomainModel (TrackEntity entity) => (Track)entity;
	protected override TrackEntity Entity (Track model) => (TrackEntity)model;

	public async Task<List<Track>> GetTracksAsync(string userEmail)
	{
		var tracks = await QueryAsync(x => x.PartitionKey == userEmail);
		return tracks.Select(DomainModel).ToList();
	}
}