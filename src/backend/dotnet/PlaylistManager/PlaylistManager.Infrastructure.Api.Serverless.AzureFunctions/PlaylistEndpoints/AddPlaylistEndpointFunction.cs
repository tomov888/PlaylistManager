using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PlaylistManager.Api.Serverless.AzureFunctions.Middleware;
using PlaylistManager.Core.Contracts.Models.UseCases;
using PlaylistManager.Core.Contracts.UseCase;
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Domain.Models;
using PlaylistManager.Infrastructure.Api.Contracts.Dto.Request;

namespace PlaylistManager.Infrastructure.Api.Serverless.AzureFunctions.PlaylistEndpoints;

public class AddPlaylistEndpointFunction
{
	private readonly ILogger<AddPlaylistEndpointFunction> _logger;
	private readonly IAddPlaylistUseCase _useCase;
	private readonly AzureFunctionsHttpMiddlewarePipelineFactory _middlewarePipeline;

	public AddPlaylistEndpointFunction(
		ILogger<AddPlaylistEndpointFunction> logger, 
		IAddPlaylistUseCase useCase, 
		AzureFunctionsHttpMiddlewarePipelineFactory middlewarePipeline)
	{
		_logger = logger;
		_useCase = useCase;
		_middlewarePipeline = middlewarePipeline;
	}
	
	[FunctionName(nameof(AddPlaylistEndpointFunction))]
	public async Task<IActionResult> AddPlaylist([HttpTrigger(AuthorizationLevel.Function, "post", Route = "playlists")] HttpRequest req)
	{
		var middleware = await _middlewarePipeline.AuthenticatedPipeline<AddPlaylistRequestDto, Playlist>(req, new List<Permission> { Permission.VIEW_SONGS });

		return await middleware
			.WithRequestPayloadValidation((payload, session) => { })
			.WithExecutingAction((_, payload, session) => _useCase.AddPlaylistAsync(new AddPlaylistRequest
			{
				Name = payload.Name, 
				Tags = payload.Tags, 
				UserEmail = session.UserEmail
			}))
			.ToIActionResultAsync();	
	}
}