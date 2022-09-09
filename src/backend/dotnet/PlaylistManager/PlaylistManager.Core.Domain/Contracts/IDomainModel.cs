namespace PlaylistManager.Core.Domain.Contracts;

public interface IDomainModel
{
	public string Id { get; init; }
	public DateTime CreatedAtUtc { get; init; }
	public DateTime UpdatedAtUtc { get; init; }
}