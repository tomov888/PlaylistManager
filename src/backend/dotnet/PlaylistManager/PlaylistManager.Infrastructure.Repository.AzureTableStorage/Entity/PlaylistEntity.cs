using PlaylistManager.Core.Domain.Contracts;
using PlaylistManager.Core.Domain.Models;
using PlaylistManager.Infrastructure.Repository.AzureTableStorage.Attributes;

namespace PlaylistManager.Infrastructure.Repository.AzureTableStorage.Entity;

[TableName("playlist")]
public record PlaylistEntity : AzureTableStorageEntity, IPlaylist
{
	public string UserEmail { get; init; }
	public string Name { get; init; }
	public string Tags { get; init; }
	public int NumberOfTracks { get; init; }
	public int DurationInSeconds { get; init; }
	public long SizeInBytes { get; init; }
	
	public static explicit operator Playlist(PlaylistEntity model)
	{
		return new Playlist
		{
			Id = model.Id,
			Name = model.Name,
			Tags = model.Tags.Split("#").ToList(),
			UserEmail = model.UserEmail,
			CreatedAtUtc = model.CreatedAtUtc,
			UpdatedAtUtc = model.UpdatedAtUtc,
			DurationInSeconds = model.DurationInSeconds,
			NumberOfTracks = model.NumberOfTracks,
			SizeInBytes = model.SizeInBytes
		};
	}
	
	public static explicit operator PlaylistEntity(Playlist model)
	{
		return new PlaylistEntity
		{
			PartitionKey = model.UserEmail,
			RowKey = model.Id,
			Timestamp = model.CreatedAtUtc,
			
			Id = model.Id,
			Name = model.Name,
			Tags = String.Join("#",model.Tags),
			UserEmail = model.UserEmail,
			CreatedAtUtc = model.CreatedAtUtc,
			UpdatedAtUtc = model.UpdatedAtUtc,
			DurationInSeconds = model.DurationInSeconds,
			NumberOfTracks = model.NumberOfTracks,
			SizeInBytes = model.SizeInBytes
		};
	}
}