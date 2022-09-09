namespace PlaylistManager.Core.Contracts.Models.Authentication;

public class AuthenticationTokenPair
{
	public string Token { get; set; }
	public string RefreshToken { get; set; }
}