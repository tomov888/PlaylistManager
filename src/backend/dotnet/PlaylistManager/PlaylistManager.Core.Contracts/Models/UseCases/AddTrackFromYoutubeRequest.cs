namespace PlaylistManager.Core.Contracts.Models.UseCases;

public class AddTrackFromYoutubeRequest
{
	public string YoutubeUrl { get; init; }
	public string TrackName { get; init; }
	public string Artist { get; init; }
	public List<string> Tags { get; init; }
	public string UserEmail { get; init; }
}