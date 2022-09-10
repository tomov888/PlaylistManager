using PlaylistManager.Core.Contracts.Models.UseCases;
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Infrastructure.Api.Contracts.Dto.Request;

public record RegisterUserRequestDto
{
	public string Email { get; init; }
	public string Username { get; init; }
	public string Password { get; init; }
	public DateTime DateOfBirth { get; init; }
	public string PhotoUrl { get; set; }

	public UserRegistrationInfo ToUserRegistrationInfo(UserRole role, AuthProvider authProvider)
	{
		var userRegistrationInfo = new UserRegistrationInfo
		(
			Email,
			Username,
			Password,
			DateOfBirth,
			PhotoUrl,
			role,
			authProvider
		);

		return userRegistrationInfo;
	}
}