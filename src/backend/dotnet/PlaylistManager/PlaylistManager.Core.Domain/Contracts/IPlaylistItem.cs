namespace PlaylistManager.Core.Domain.Contracts;

public interface IPlaylistTrack: IDomainModel, IOwnedByUser
{
	public string TrackId { get; init; }
	public string TrackName { get; set; }
	public string FileUrl { get; init; }
	public string ThumbnailUrl { get; init; }
}