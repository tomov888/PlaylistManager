using System.Security.Authentication;
using Google.Apis.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Domain.Models;
using PlaylistManager.Core.Services.Authentication.JsonWebToken;
using PlaylistManager.Infrastructure.Api.Contracts.Service;

namespace PlaylistManager.Infrastructure.Api.Service;

public class AuthorizeService: IAuthorizeService
{
	private readonly ILogger<AuthorizeService> _logger;
	private readonly ITokenHandler _tokenHandler;

	public AuthorizeService(ILogger<AuthorizeService> logger, ITokenHandler tokenHandler)
	{
		_tokenHandler = tokenHandler;
		_logger = logger;
	}

	public async Task<OperationResult<UserSession>> AuthorizeAsync(HttpRequest httpRequest, List<Permission> permissions)
	{
		var token = httpRequest.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
		
		if(token is null) return OperationResult<UserSession>.Failure(new AuthenticationException($"Request does not contain jwt."));

		try
		{
			var validatedToken = _tokenHandler.ValidateToken(token);
			var userEmail = validatedToken.Claims.First(x => x.Type == AuthorizationClaimTypes.Email).Value;
			var userId = validatedToken.Claims.First(x => x.Type == AuthorizationClaimTypes.UserId).Value;
			var username = validatedToken.Claims.First(x => x.Type == AuthorizationClaimTypes.Username).Value;
			var authProvider = Enum.Parse<AuthProvider>(validatedToken.Claims.First(x => x.Type == AuthorizationClaimTypes.AuthProvider).Value);
			var sessionId = validatedToken.Claims.First(x => x.Type == AuthorizationClaimTypes.SessionId).Value;
			var userRole = Enum.Parse<UserRole>(validatedToken.Claims.First(x => x.Type == AuthorizationClaimTypes.Role).Value);

			var userSession = new UserSession
			{
				UserEmail = userEmail,
				UserId = userId,
				Username = username,
				AuthProvider = authProvider,
				Token = token,
				SessionId = sessionId,
				Role= userRole,
				CorrelationId = Guid.NewGuid().ToString()
			};
			
			return OperationResult<UserSession>.Success(userSession);
		}
		catch (Exception e)
		{
			return OperationResult<UserSession>.Failure(new Exception($"jwt is invalid."));
		}
	}
}