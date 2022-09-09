using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PlaylistManager.Core.Services.Authentication;
using PlaylistManager.Infrastructure.Api.Contracts.Dto.Request;

namespace PlaylistManager.Infrastructure.Api.Serverless.AzureFunctions;

public class UserRegistrationEndpointFunction
{
	private readonly ILogger<UserRegistrationEndpointFunction> _logger;
	private readonly IUserRegistrationService _registrationService;

	public UserRegistrationEndpointFunction(ILogger<UserRegistrationEndpointFunction> logger, IUserRegistrationService registrationService)
	{
		_logger = logger;
		_registrationService = registrationService;
	}

	[FunctionName(nameof(UserRegistrationEndpointFunction))]
	public async Task<IActionResult> Register([HttpTrigger(AuthorizationLevel.Function, "post", Route = "register")] HttpRequest req)
	{
		try
		{
			_logger.LogInformation("REGISTER ENDPOINT");

			var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			var payload = JsonConvert.DeserializeObject<RegisterUserRequestDto>(requestBody);
			var registeredUser = await _registrationService.RegisterUserAsync(payload.Email, payload.Username,
				payload.Password, payload.DateOfBirth, payload.PhotoUrl);
			return new OkObjectResult(registeredUser);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return new BadRequestObjectResult(e.Message);
		}
	}
}