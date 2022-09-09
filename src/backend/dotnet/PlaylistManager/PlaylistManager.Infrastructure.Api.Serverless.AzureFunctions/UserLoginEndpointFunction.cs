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

public class UserLoginEndpointFunction
{
	private readonly ILogger<UserLoginEndpointFunction> _logger;
	private readonly IUserLoginService _loginService;

	public UserLoginEndpointFunction(ILogger<UserLoginEndpointFunction> logger, IUserLoginService loginService)
	{
		_logger = logger;
		_loginService = loginService;
	}

	[FunctionName(nameof(UserLoginEndpointFunction))]
	public async Task<IActionResult> Login([HttpTrigger(AuthorizationLevel.Function, "post", Route = "login")] HttpRequest req)
	{
		try
		{
			_logger.LogInformation("LOGIN ENDPOINT");

			var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			var payload = JsonConvert.DeserializeObject<LoginRequestDto>(requestBody);
			var loginResult = await _loginService.TryLoginAsync(payload.Email,  payload.Password);
			return new OkObjectResult(loginResult);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return new BadRequestObjectResult(e.Message);
		}
	}

}