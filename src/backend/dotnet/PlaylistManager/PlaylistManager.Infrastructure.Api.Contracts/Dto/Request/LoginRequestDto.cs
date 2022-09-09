namespace PlaylistManager.Infrastructure.Api.Contracts.Dto.Request;

public record LoginRequestDto
{
	public string Email { get; init; }
	public string Password { get; init; }
}