namespace PlaylistManager.Core.Domain.Contracts;

public interface IPlaylist: IDomainModel, IOwnedByUser
{
	public string Name { get; init; }
	public int NumberOfTracks { get; init; }
	public int DurationInSeconds { get; init; }
	public long SizeInBytes { get; init; }
}
