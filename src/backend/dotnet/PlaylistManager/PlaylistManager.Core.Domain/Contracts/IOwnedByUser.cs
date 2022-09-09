namespace PlaylistManager.Core.Domain.Contracts;

public interface IOwnedByUser
{
	public string UserEmail { get; set; }
}