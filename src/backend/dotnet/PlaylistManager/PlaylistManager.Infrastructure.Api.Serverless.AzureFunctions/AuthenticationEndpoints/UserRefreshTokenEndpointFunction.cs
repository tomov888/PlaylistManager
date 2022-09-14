using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PlaylistManager.Api.Serverless.AzureFunctions.Middleware;
using PlaylistManager.Core.Contracts.Models.Authentication;
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Services.Authentication;
using PlaylistManager.Infrastructure.Api.Contracts.Dto.Request;

namespace PlaylistManager.Infrastructure.Api.Serverless.AzureFunctions.AuthenticationEndpoints;

public class UserRefreshTokenEndpointFunction
{
	private readonly ILogger<UserRefreshTokenEndpointFunction> _logger;
	private readonly IUserLoginService _loginService;
	private readonly AzureFunctionsHttpMiddlewarePipelineFactory _middlewarePipeline;

	public UserRefreshTokenEndpointFunction(
		ILogger<UserRefreshTokenEndpointFunction> logger, 
		IUserLoginService loginService, 
		AzureFunctionsHttpMiddlewarePipelineFactory middlewarePipeline)
	{
		_logger = logger;
		_loginService = loginService;
		_middlewarePipeline = middlewarePipeline;
	}
	
	[FunctionName(nameof(UserRefreshTokenEndpointFunction))]
	public async Task<IActionResult> RefreshToken([HttpTrigger(AuthorizationLevel.Function, "post", Route = "refresh-token")] HttpRequest req)
	{
		var middleware = await _middlewarePipeline.AuthenticatedPipeline<RefreshTokenRequestDto, UserLogin>(req, new List<Permission>());

		return await middleware
			.WithRequestPayloadValidation((payload, _) =>
			{
				if (payload is null) throw new Exception("Payload is null");
				if (payload.Token is null) throw new Exception("Token value is invalid");
				if (payload.RefreshToken is null) throw new Exception("Refresh Token value is invalid");
			})
			.WithExecutingAction((_, payload, _) => _loginService.TryRefreshTokenAsync(payload.Token, payload.RefreshToken))
			.ToIActionResultAsync();
	}
	
}