namespace PlaylistManager.Core.Contracts.Models.Authentication;

public class GoogleTokenValidationPayload
{
	public string UserId { get; set; }
	public string Email { get; set; }
	public bool EmailVerified { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string FamilyName { get; set; }
	public string Picture { get; set; }
	public string FullName { get; set; }
}