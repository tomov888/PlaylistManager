using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace PlaylistManager.Core.Services.Authentication.JsonWebToken;

public class TokenHandler : ITokenHandler
{
	private readonly TokenValidationParameters _tokenValidationParameters;
	private readonly JwtSecurityTokenHandler _tokenHandler;

	public TokenHandler(TokenValidationParameters tokenValidationParameters)
	{
		_tokenValidationParameters = tokenValidationParameters;
		_tokenHandler = new JwtSecurityTokenHandler();
	}
		
	public ClaimsPrincipal ValidateToken(string token)
	{
		var principal = _tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);

		if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
		{
			throw new SecurityTokenException("JWT validation failed. Invalid token");
		};

		return principal;

	}		
	public ClaimsPrincipal ValidateTokenOnRefreshToken(string token)
	{
		var tokenValidationParameters = _tokenValidationParameters.Clone();

		tokenValidationParameters.ValidateLifetime = false;
		// We do this in order to prevent lifetime check on refresh token
		// because on refresh token has already expired !!!

		var principal = _tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

		if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
		{
			throw new SecurityTokenException("JWT validation failed. Invalid token");
		};

		return principal;

	}
		
	private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
	{
		return
			(validatedToken is JwtSecurityToken jwtSecurityToken) &&
			jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
	}		
}