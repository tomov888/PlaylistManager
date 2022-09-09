using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Domain.Contracts;

public interface IUser: IDomainModel
{
	public string Email { get; init; }
	public string Username { get; init; }
	public string PasswordHash { get; init; }
	public string PasswordSalt { get; init; }
	public DateTime DateOfBirth { get; init; }
	public string PhotoUrl { get; init; }
	public UserRole Role { get; init; }
	public AuthProvider AuthProvider { get; init; }
}