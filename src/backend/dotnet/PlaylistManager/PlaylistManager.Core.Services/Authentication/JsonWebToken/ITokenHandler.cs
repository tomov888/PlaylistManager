using System.Security.Claims;

namespace PlaylistManager.Core.Services.Authentication.JsonWebToken;

public interface ITokenHandler
{
	ClaimsPrincipal ValidateToken(string token);
	ClaimsPrincipal ValidateTokenOnRefreshToken(string token);
}