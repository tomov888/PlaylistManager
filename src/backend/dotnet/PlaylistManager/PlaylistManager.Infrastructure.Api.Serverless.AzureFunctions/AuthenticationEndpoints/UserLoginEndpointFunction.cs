using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PlaylistManager.Api.Serverless.AzureFunctions.Middleware;
using PlaylistManager.Core.Common.Extensions.FunctionalExtensions.OperationResultExtensions;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Contracts.Models.Authentication;
using PlaylistManager.Core.Domain.Models;
using PlaylistManager.Core.Services.Authentication;
using PlaylistManager.Infrastructure.Api.Contracts.Dto.Request;

namespace PlaylistManager.Infrastructure.Api.Serverless.AzureFunctions.AuthenticationEndpoints;

public class UserLoginEndpointFunction
{
	private readonly ILogger<UserLoginEndpointFunction> _logger;
	private readonly IUserLoginService _loginService;
	private readonly AzureFunctionsHttpMiddlewarePipelineFactory _middlewarePipeline;

	public UserLoginEndpointFunction(
		ILogger<UserLoginEndpointFunction> logger, 
		IUserLoginService loginService, 
		AzureFunctionsHttpMiddlewarePipelineFactory middlewarePipeline)
	{
		_logger = logger;
		_loginService = loginService;
		_middlewarePipeline = middlewarePipeline;
	}

	[FunctionName(nameof(UserLoginEndpointFunction))]
	public async Task<IActionResult> Login([HttpTrigger(AuthorizationLevel.Function, "post", Route = "login")] HttpRequest req)
	{

		var middleware = await _middlewarePipeline.AnonymousPipeline<LoginRequestDto, UserLogin>(req);

		return await middleware
			.WithRequestPayloadValidation((payload, _) =>
			{
				if (payload is null) throw new Exception("HAHAHAH");
				if (payload.Email is null) throw new Exception("HIIHIHIH");
				if (payload.Password is null) throw new Exception("HEHEHEHE");
			})
			.WithExecutingAction((_, payload, _) => _loginService.TryLoginAsync(payload.Email, payload.Password))
			.ToIActionResultAsync();
	}

}