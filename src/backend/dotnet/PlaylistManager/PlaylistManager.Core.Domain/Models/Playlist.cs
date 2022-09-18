using PlaylistManager.Core.Domain.Contracts;

namespace PlaylistManager.Core.Domain.Models;

public record Playlist: IPlaylist
{
	public string Id { get; init; }
	public DateTime CreatedAtUtc { get; init; }
	public DateTime UpdatedAtUtc { get; init; }
	public string UserEmail { get; init; }
	public string Name { get; init; }
	public List<string> Tags { get; init; }
	public int NumberOfTracks { get; init; }
	public int DurationInSeconds { get; init; }
	public long SizeInBytes { get; init; }

	public static Playlist NewPlaylist(string userEmail, string name, List<string> tags)
	{
		return new Playlist
		{
			Id = Guid.NewGuid().ToString(),
			Name = name,
			Tags = tags,
			UserEmail = userEmail,
			CreatedAtUtc = DateTime.UtcNow,
			UpdatedAtUtc = DateTime.UtcNow,
			DurationInSeconds = 0,
			NumberOfTracks = 0,
			SizeInBytes = 0
		};
	}
}