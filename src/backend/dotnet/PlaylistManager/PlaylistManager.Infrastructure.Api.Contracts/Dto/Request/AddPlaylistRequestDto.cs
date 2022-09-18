namespace PlaylistManager.Infrastructure.Api.Contracts.Dto.Request;

public record AddPlaylistRequestDto
{
	public string Name { get; init; }
	public List<string> Tags { get; init; }
}