namespace PlaylistManager.Core.Domain.Models;

public record UserSession
{
	public string Token { get; init; }
	public string JwtId { get; init; }
	public DateTime CreatedAtUtc { get; init; }
	public DateTime ExpiresAtUtc { get; init; }
	public string UserId { get; init; }
	public string UserEmail { get; init; }
	public string SessionId { get; init; }
}