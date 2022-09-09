namespace PlaylistManager.Core.Domain.Contracts;

public interface ISong: IDomainModel, IOwnedByUser
{
	public string Name { get; init; }
	public string Artist { get; init; }
	public string Description { get; init; }
	public int DurationInSeconds { get; init; }
	public List<string> Tags { get; init; }
	public DateTime ReleasedAtUtc { get; set; }
}