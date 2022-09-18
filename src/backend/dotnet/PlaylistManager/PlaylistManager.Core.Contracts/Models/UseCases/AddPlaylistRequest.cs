namespace PlaylistManager.Core.Contracts.Models.UseCases;

public record AddPlaylistRequest
{
	public string Name { get; init; }
	public string UserEmail { get; init; }
	public List<string> Tags { get; init; }
}