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
using PlaylistManager.Api.Serverless.AzureFunctions.Utils;
using PlaylistManager.Core.Common.Extensions.FunctionalExtensions.OperationResultExtensions;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Contracts.Models.UseCases;
using PlaylistManager.Core.Contracts.UseCase;
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Domain.Models;
using PlaylistManager.Infrastructure.Api.Contracts.Dto.Request;
using PlaylistManager.Infrastructure.Api.Contracts.Service;

namespace PlaylistManager.Infrastructure.Api.Serverless.AzureFunctions.TrackEndpoints;

public class AddTrackFromYoutubeEndpointFunction
{
	private readonly ILogger<GetTracksEndpointFunction> _logger;
	private readonly AzureFunctionsHttpMiddlewarePipelineFactory _middlewarePipeline;
	private readonly IYoutubeTrackDownloadQueueProducer _queueProducer;

	public AddTrackFromYoutubeEndpointFunction(
		ILogger<GetTracksEndpointFunction> logger,
		AzureFunctionsHttpMiddlewarePipelineFactory middlewarePipeline,
		IYoutubeTrackDownloadQueueProducer queueProducer)
	{
		_logger = logger;
		_middlewarePipeline = middlewarePipeline;
		_queueProducer = queueProducer;
	}

	[FunctionName(nameof(AddTrackFromYoutubeEndpointFunction))]
	public async Task<IActionResult> AddTrackFromYoutubeVideo(
		[HttpTrigger(AuthorizationLevel.Function, "post", Route = "tracks/add-youtube-track")]
		HttpRequest req,
		ExecutionContext context)
	{
		var middleware = await _middlewarePipeline.AuthenticatedPipeline<AddTrackFromYoutubeRequestDto, OperationResult<AddTrackFromYoutubeRequest>>(req, new List<Permission> { Permission.VIEW_SONGS });

		var middlewareResult = await middleware
			.WithExecutingAction((_, payload, session) => _queueProducer.EnqueueYoutubeTrackDownloadRequestAsync(
				new AddTrackFromYoutubeRequest
				{
					Artist = payload.Artist,
					Tags = payload.Tags,
					TrackName = payload.TrackName,
					UserEmail = session.UserEmail,
					YoutubeUrl = payload.YoutubeUrl
				}
			))
			.OperationResultAsync();

		if (middlewareResult.IsFailure) return ActionResultMapper.Map(middlewareResult.FailureReason);

		return middlewareResult.Payload
			.OnSuccess(x => (IActionResult) new OkObjectResult(x))
			.OnFailure(x => (IActionResult) ActionResultMapper.Map(x))
			.Finally();
	}
}
