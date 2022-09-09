namespace PlaylistManager.Core.Contracts.Models.Authentication;

public class AuthenticationConfiguration
{
	public string Secret { get; set; }
	public string Issuer { get; set; }
	public TimeSpan TokenLifetime { get; set; }
}