using PlaylistManager.Core.Domain.Contracts;
using PlaylistManager.Core.Domain.Enums;

namespace PlaylistManager.Core.Domain.Models;

public record User: IUser
{
	public string Id { get; init; }
	public DateTime CreatedAtUtc { get; init; }
	public DateTime UpdatedAtUtc { get; init; }
	public string Email { get; init; }
	public string Username { get; init; }
	public string PasswordHash { get; init; }
	public string PasswordSalt { get; init; }
	public DateTime DateOfBirth { get; init; }
	public string PhotoUrl { get; init; }
	public UserRole Role { get; init; }
	public AuthProvider AuthProvider { get; init; }
}