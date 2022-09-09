using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Contracts.Models.Authentication;

public class RefreshToken
{
	public string Token { get; set; }
	public string JwtId { get; set; }
	public DateTime CreatedAtUtc { get; set; }
	public DateTime ExpiresAtUtc { get; set; }
	public bool Used { get; set; }
	public bool Invalidated { get; set; }
	public string UserId { get; set; }
	public string UserEmail { get; set; }

	public UserSession ToSession(string sessionId)
	{
		return new UserSession
		{
			Token = Token,
			JwtId = JwtId,
			CreatedAtUtc = CreatedAtUtc,
			ExpiresAtUtc = ExpiresAtUtc,
			UserId = UserId,
			UserEmail = UserEmail,
			SessionId = sessionId
		};
	}
}