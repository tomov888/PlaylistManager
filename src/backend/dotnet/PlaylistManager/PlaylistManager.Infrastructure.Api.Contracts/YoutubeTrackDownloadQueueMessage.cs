namespace PlaylistManager.Infrastructure.Api.Contracts;

public record YoutubeTrackDownloadQueueMessage
{
	public string YoutubeUrl { get; init; }
	public string TrackName { get; init; }
	public string Artist { get; init; }
	public List<string> Tags { get; init; }
	public string UserEmail { get; init; }
}