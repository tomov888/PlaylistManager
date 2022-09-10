using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Contracts.Models.Authentication;

public class RefreshToken
{
	public string Token { get; set; }
	public string JwtId { get; set; }
	public DateTime CreatedAtUtc { get; set; }
	public DateTime ExpiresAtUtc { get; set; }
	public string UserId { get; set; }
	public string UserEmail { get; set; }
}