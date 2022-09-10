namespace PlaylistManager.Core.Domain.Contracts;

public interface IPlaylist: IDomainModel, IOwnedByUser
{
	public string Name { get; init; }
	public List<string> Tags { get; init; }
	public int NumberOfTracks { get; set; }
	public int DurationInSeconds { get; set; }
	public long SizeInBytes { get; set; }
}
