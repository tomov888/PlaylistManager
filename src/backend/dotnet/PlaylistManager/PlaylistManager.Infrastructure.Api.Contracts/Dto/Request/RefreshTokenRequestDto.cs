namespace PlaylistManager.Infrastructure.Api.Contracts.Dto.Request;

public record RefreshTokenRequestDto
{
	public string Token { get; init; }
	public string RefreshToken { get; init; }
}