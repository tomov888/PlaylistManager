using PlaylistManager.Core.Domain.Contracts;
using PlaylistManager.Core.Domain.Models;
using PlaylistManager.Infrastructure.Repository.AzureTableStorage.Attributes;

namespace PlaylistManager.Infrastructure.Repository.AzureTableStorage.Entity;

[TableName("track")]
public record TrackEntity : AzureTableStorageEntity, ITrack
{
	public string UserEmail { get; init; }
	public string Name { get; init; }
	public string Artist { get; init; }
	public string Description { get; init; }
	public int DurationInSeconds { get; init; }
	public long SizeInBytes { get; init; }
	public string Tags { get; init; }
	public string FileUrl { get; init; }
	
	public static explicit operator TrackEntity(Track model)
	{
		return new TrackEntity
		{
			PartitionKey = model.UserEmail,
			RowKey = model.Id,
			Timestamp = model.CreatedAtUtc,
			
			Id = model.Id,
			Artist = model.Artist,
			Name = model.Name,
			Description = model.Description,
			Tags = String.Join("#",model.Tags),
			UserEmail = model.UserEmail,
			DurationInSeconds = model.DurationInSeconds,
			CreatedAtUtc = model.CreatedAtUtc,
			UpdatedAtUtc = model.UpdatedAtUtc,
			FileUrl = model.FileUrl,
			SizeInBytes = model.SizeInBytes
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
			Tags = model.Tags.Split("#").ToList(),
			UserEmail = model.UserEmail,
			DurationInSeconds = model.DurationInSeconds,
			CreatedAtUtc = model.CreatedAtUtc,
			UpdatedAtUtc = model.UpdatedAtUtc,
			FileUrl = model.FileUrl,
			SizeInBytes = model.SizeInBytes
		};
	}		
}