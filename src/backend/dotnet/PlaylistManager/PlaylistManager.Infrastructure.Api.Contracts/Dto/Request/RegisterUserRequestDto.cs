namespace PlaylistManager.Infrastructure.Api.Contracts.Dto.Request;

public record RegisterUserRequestDto
{
	public string Email { get; init; }
	public string Username { get; init; }
	public string Password { get; init; }
	public DateTime DateOfBirth { get; init; }

	public string PhotoUrl { get; set; }
}