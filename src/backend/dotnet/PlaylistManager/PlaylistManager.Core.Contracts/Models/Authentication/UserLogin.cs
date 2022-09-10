namespace PlaylistManager.Core.Contracts.Models.Authentication;

public record UserLogin
{
	public string Token { get; init; }
	public string RefreshToken { get; init; }
}