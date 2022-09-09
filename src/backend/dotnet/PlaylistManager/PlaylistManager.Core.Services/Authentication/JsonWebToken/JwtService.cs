using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PlaylistManager.Core.Contracts.Models.Authentication;
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Services.Authentication.JsonWebToken;

public class JwtService : IJwtService
{
		private readonly AuthenticationConfiguration _authenticationConfiguration;
		
		public JwtService(AuthenticationConfiguration authenticationConfiguration)
		{
			_authenticationConfiguration = authenticationConfiguration;
		}

		public async Task<AuthenticationTokenPair> GenerateTokenPairAsync(User user, List<string> permissions)
		{
			var sessionId = Guid.NewGuid().ToString();
			
			var tokenHandler = new JwtSecurityTokenHandler();

			SecurityTokenDescriptor tokenDescriptor = GenerateSecurityTokenDescriptor(user, sessionId, permissions);

			var token = tokenHandler.CreateToken(tokenDescriptor);

			RefreshToken refreshToken = await GenerateRefreshTokenAsync(user, token);

			string tokenString = tokenHandler.WriteToken(token);
			
			return new AuthenticationTokenPair { Token = tokenString, RefreshToken = refreshToken.Token };
		}
		
		private SecurityTokenDescriptor GenerateSecurityTokenDescriptor(User user, string sessionId, List<string> permissions)
		{
			var key = Encoding.ASCII.GetBytes(_authenticationConfiguration.Secret);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Issuer = _authenticationConfiguration.Issuer,
				Subject = new ClaimsIdentity(GetClaimsFor(user, sessionId, permissions)),
				Expires = DateTime.UtcNow.Add(_authenticationConfiguration.TokenLifetime),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			return tokenDescriptor;
		}
		
		
		private async Task<RefreshToken> GenerateRefreshTokenAsync(User user, SecurityToken token)
		{
			await Task.CompletedTask;
			
			var refreshToken = new RefreshToken
			{
				JwtId = token.Id,
				UserId = user.Id,
				UserEmail = user.Email,
				Token = Guid.NewGuid().ToString(),
				CreatedAtUtc = DateTime.UtcNow,
				ExpiresAtUtc = DateTime.UtcNow.AddDays(1)
			};
			
			return refreshToken;
		}	
		
		
		private List<Claim> GetClaimsFor(User user, string sessionId, List<string> permissions)
		{
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Sub, user.Email),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(AuthorizationClaimTypes.UserId, user.Id),
				new Claim(AuthorizationClaimTypes.Username, user.Username),
				new Claim(AuthorizationClaimTypes.AuthProvider, user.AuthProvider.ToString()),
				new Claim(ClaimTypes.Role, user.Role.ToString()),
				new Claim(AuthorizationClaimTypes.Permissions, string.Join(",", permissions)),
				new Claim(AuthorizationClaimTypes.Role, user.Role.ToString()),
				new Claim(AuthorizationClaimTypes.Email, user.Email),
				new Claim(AuthorizationClaimTypes.SessionId, sessionId),
				new Claim(UserInfoClaimTypes.PhotoUrl, user.PhotoUrl ?? ""),
			};
			return claims;
		}
		
	}