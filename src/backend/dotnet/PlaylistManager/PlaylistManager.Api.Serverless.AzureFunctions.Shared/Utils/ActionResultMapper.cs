using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace PlaylistManager.Api.Serverless.AzureFunctions.Utils;

public static class ActionResultMapper
{
	public static IActionResult Map(Exception ex) =>
		ex switch
		{
			AuthenticationException => new UnauthorizedResult(),
			UnauthorizedAccessException => new ForbidResult(),
			_ => new BadRequestObjectResult(new { Message = ex.Message })
		};
}