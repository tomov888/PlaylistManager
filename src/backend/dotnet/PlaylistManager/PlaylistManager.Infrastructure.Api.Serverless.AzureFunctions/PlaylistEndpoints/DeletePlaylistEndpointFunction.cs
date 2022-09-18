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

public class DeletePlaylistEndpointFunction
{
	private readonly ILogger<DeletePlaylistEndpointFunction> _logger;
	private readonly AzureFunctionsHttpMiddlewarePipelineFactory _middlewarePipeline;
	private readonly IDeletePlaylistUseCase _useCase;

	public DeletePlaylistEndpointFunction(
		ILogger<DeletePlaylistEndpointFunction> logger,
		IDeletePlaylistUseCase useCase,
		AzureFunctionsHttpMiddlewarePipelineFactory middlewarePipeline)
	{
		_logger = logger;
		_useCase = useCase;
		_middlewarePipeline = middlewarePipeline;
	}

	[FunctionName(nameof(DeletePlaylistEndpointFunction))]
	public async Task<IActionResult> GetPlaylists([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "playlists/{playlistId}")] HttpRequest req, string playlistId)
	{
		var middleware = await _middlewarePipeline.AuthenticatedPipeline<Playlist>(req, new List<Permission> { Permission.VIEW_SONGS });

		return await middleware
			.WithExecutingAction((_, _, session) => _useCase.DeletePlaylistAsync(session.UserEmail, playlistId))
			.ToIActionResultAsync();
	}
}