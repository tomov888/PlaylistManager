using System;
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

namespace PlaylistManager.Infrastructure.Api.Serverless.AzureFunctions.TrackEndpoints;

public class GetTracksEndpointFunction
{
	private readonly ILogger<GetTracksEndpointFunction> _logger;
	private readonly AzureFunctionsHttpMiddlewarePipelineFactory _middlewarePipeline;
	private readonly IGetTracksUseCase _useCase;

	public GetTracksEndpointFunction(ILogger<GetTracksEndpointFunction> logger, AzureFunctionsHttpMiddlewarePipelineFactory middlewarePipeline, IGetTracksUseCase useCase)
	{
		_logger = logger;
		_middlewarePipeline = middlewarePipeline;
		_useCase = useCase;
	}
	
	[FunctionName(nameof(GetTracksEndpointFunction))]
	public async Task<IActionResult> GetTracks([HttpTrigger(AuthorizationLevel.Function, "get", Route = "tracks")] HttpRequest req)
	{
		var middleware = await _middlewarePipeline.AuthenticatedPipeline<List<Track>>(req, new List<Permission> { Permission.VIEW_SONGS });

		return await middleware
			.WithExecutingAction((_, payload, session) => _useCase.GetTracksAsync(session.UserEmail))
			.ToIActionResultAsync();	
		
		// var middleware = await _middlewarePipeline.AnonymousPipeline<List<Track>>(req);
		//
		// return await middleware
		// 	.WithRequestValidation((req, _) => { if (userId is null) throw new Exception("HEHEHEHE"); })
		// 	.WithExecutingAction((_, payload, _) => _useCase.GetTracksAsync(userId))
		// 	.ToIActionResultAsync();		
	}
}