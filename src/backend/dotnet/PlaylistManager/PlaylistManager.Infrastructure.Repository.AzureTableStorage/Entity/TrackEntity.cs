using PlaylistManager.Core.Domain.Contracts;
using PlaylistManager.Core.Domain.Models;
using PlaylistManager.Infrastructure.Repository.AzureTableStorage.Attributes;

namespace PlaylistManager.Infrastructure.Repository.AzureTableStorage.Entity;

[TableName("track")]
public record TrackEntity : AzureTableStorageEntity, ITrack
{
	public string UserEmail { get; set; }
	public string Name { get; init; }
	public string Artist { get; init; }
	public string Description { get; init; }
	public int DurationInSeconds { get; init; }
	public List<string> Tags { get; init; }
	public DateTime ReleasedAtUtc { get; set; }
	
	public static explicit operator TrackEntity(Track model)
	{
		return new TrackEntity
		{
			PartitionKey = model.UserEmail,
			RowKey = model.Id,
			Timestamp = DateTimeOffset.UtcNow,
			
			Id = model.Id,
			Artist = model.Artist,
			Name = model.Name,
			Description = model.Description,
			Tags = model.Tags,
			UserEmail = model.UserEmail,
			DurationInSeconds = model.DurationInSeconds,
			ReleasedAtUtc = model.ReleasedAtUtc,
			CreatedAtUtc = model.CreatedAtUtc,
			UpdatedAtUtc = model.UpdatedAtUtc
		};
	}	
	
	public static explicit operator Track(TrackEntity model)
	{
		return new Track
		{
			Id = model.Id,
			Artist = model.Artist,
			Name = model.Name,
			Description = model.Description,
			Tags = model.Tags,
			UserEmail = model.UserEmail,
			DurationInSeconds = model.DurationInSeconds,
			ReleasedAtUtc = model.ReleasedAtUtc,
			CreatedAtUtc = model.CreatedAtUtc,
			UpdatedAtUtc = model.UpdatedAtUtc
		};
	}		
}