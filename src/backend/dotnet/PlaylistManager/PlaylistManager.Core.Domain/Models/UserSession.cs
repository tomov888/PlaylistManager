using PlaylistManager.Core.Domain.Enums;

namespace PlaylistManager.Core.Domain.Models;

public record UserSession
{
	public string Token { get; init; }
	public string UserId { get; init; }
	public string UserEmail { get; init; }
	public string Username { get; init; }
	public AuthProvider AuthProvider { get; init; }
	public UserRole Role { get; init; }
	public string SessionId { get; init; }
	public string CorrelationId { get; init; }
}