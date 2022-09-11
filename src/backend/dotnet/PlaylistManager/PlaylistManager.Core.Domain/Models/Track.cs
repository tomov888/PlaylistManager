using PlaylistManager.Core.Domain.Contracts;

namespace PlaylistManager.Core.Domain.Models;

public record Track: ITrack
{
	public string Id { get; init; }
	public DateTime CreatedAtUtc { get; init; }
	public DateTime UpdatedAtUtc { get; init; }
	public string UserEmail { get; set; }
	public string Name { get; init; }
	public string Artist { get; init; }
	public string Description { get; init; }
	public int DurationInSeconds { get; init; }
	public List<string> Tags { get; init; }
	public DateTime ReleasedAtUtc { get; init; }
	
	public string FileUrl { get; init; }
}