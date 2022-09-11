namespace PlaylistManager.Infrastructure.Api.Contracts.Dto.Request;

public record AddTrackFromYoutubeRequestDto
{
	public string YoutubeUrl { get; init; }
	public string TrackName { get; init; }
	public string Artist { get; init; }
}