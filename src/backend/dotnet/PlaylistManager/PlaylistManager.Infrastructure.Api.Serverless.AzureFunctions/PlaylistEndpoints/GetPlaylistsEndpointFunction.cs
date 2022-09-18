using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PlaylistManager.Api.Serverless.AzureFunctions.Middleware;
using PlaylistManager.Core.Contracts.UseCase;
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Infrastructure.Api.Serverless.AzureFunctions.PlaylistEndpoints;

public class GetPlaylistsEndpointFunction
{
	private readonly ILogger<GetPlaylistsEndpointFunction> _logger;
	private readonly AzureFunctionsHttpMiddlewarePipelineFactory _middlewarePipeline;
	private readonly IGetPlaylistsUseCase _useCase;

	public GetPlaylistsEndpointFunction(
		ILogger<GetPlaylistsEndpointFunction> logger,
		IGetPlaylistsUseCase useCase,
		AzureFunctionsHttpMiddlewarePipelineFactory middlewarePipeline)
	{
		_logger = logger;
		_useCase = useCase;
		_middlewarePipeline = middlewarePipeline;
	}

	[FunctionName(nameof(GetPlaylistsEndpointFunction))]
	public async Task<IActionResult> GetPlaylists([HttpTrigger(AuthorizationLevel.Function, "get", Route = "playlists")] HttpRequest req)
	{
		var middleware = await _middlewarePipeline.AuthenticatedPipeline<List<Playlist>>(req, new List<Permission> { Permission.VIEW_SONGS });

		return await middleware
			.WithExecutingAction((_, _, session) => _useCase.GetPlaylists(session.UserEmail))
			.ToIActionResultAsync();
	}
}