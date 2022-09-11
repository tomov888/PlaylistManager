namespace PlaylistManager.Core.Domain.Contracts;

public interface ITrack: IDomainModel, IOwnedByUser
{
	public string Name { get; init; }
	public string Artist { get; init; }
	public string Description { get; init; }
	public int DurationInSeconds { get; init; }
	public string FileUrl { get; init; }
}