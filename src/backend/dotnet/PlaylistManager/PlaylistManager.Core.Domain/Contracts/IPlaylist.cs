namespace PlaylistManager.Core.Domain.Contracts;

public interface IPlaylist: IDomainModel, IOwnedByUser
{
	public string Name { get; init; }
	public List<ISong> Songs { get; init; }
}
