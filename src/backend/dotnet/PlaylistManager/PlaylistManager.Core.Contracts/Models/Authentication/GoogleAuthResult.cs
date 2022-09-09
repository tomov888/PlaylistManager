namespace PlaylistManager.Core.Contracts.Models.Authentication;

public class GoogleAuthResult
{
	public bool Success { get; set; }
	public GoogleTokenValidationPayload Payload { get; set; }
}