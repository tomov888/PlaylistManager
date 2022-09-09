namespace PlaylistManager.Core.Contracts.Models.Authentication;

public class UserLoginResult
{
	public string Token { get; set; }
	public string RefreshToken { get; set; }
}