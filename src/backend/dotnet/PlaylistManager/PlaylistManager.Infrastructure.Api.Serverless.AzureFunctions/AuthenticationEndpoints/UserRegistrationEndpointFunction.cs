using System;
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
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Domain.Models;
using PlaylistManager.Core.Services.Authentication;
using PlaylistManager.Infrastructure.Api.Contracts.Dto.Request;

namespace PlaylistManager.Infrastructure.Api.Serverless.AzureFunctions.AuthenticationEndpoints;

public class UserRegistrationEndpointFunction
{
	private readonly ILogger<UserRegistrationEndpointFunction> _logger;
	private readonly IUserRegistrationService _registrationService;
	private readonly AzureFunctionsHttpMiddlewarePipelineFactory _middlewarePipeline;

	public UserRegistrationEndpointFunction(
		ILogger<UserRegistrationEndpointFunction> logger, 
		IUserRegistrationService registrationService, 
		AzureFunctionsHttpMiddlewarePipelineFactory middlewarePipeline)
	{
		_logger = logger;
		_registrationService = registrationService;
		_middlewarePipeline = middlewarePipeline;
	}

	[FunctionName(nameof(UserRegistrationEndpointFunction))]
	public async Task<IActionResult> Register([HttpTrigger(AuthorizationLevel.Function, "post", Route = "register")] HttpRequest req)
	{
		var middleware = await _middlewarePipeline.AnonymousPipeline<RegisterUserRequestDto, User>(req);

		return await middleware
			.WithRequestPayloadValidation((payload, _) =>
			{
				if (payload is null) throw new Exception("HAHAHAH");
				if (payload.Email is null) throw new Exception("HIIHIHIH");
				if (payload.Password is null) throw new Exception("HEHEHEHE");
			})
			.WithExecutingAction((_, payload, _) => _registrationService.TryRegisterUserAsync(payload.ToUserRegistrationInfo(UserRole.FREE_USER, AuthProvider.SELF)))
			.ToIActionResultAsync();		
	}
}