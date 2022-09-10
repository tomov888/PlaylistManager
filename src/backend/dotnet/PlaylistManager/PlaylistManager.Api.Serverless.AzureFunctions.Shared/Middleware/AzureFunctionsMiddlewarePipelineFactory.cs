using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Infrastructure.Api.Service;

namespace PlaylistManager.Api.Serverless.AzureFunctions.Middleware;

public class AzureFunctionsHttpMiddlewarePipelineFactory
{
	private readonly ILoggerFactory _loggerFactory;
	private readonly IAuthorizeService _authorizer;

	public AzureFunctionsHttpMiddlewarePipelineFactory(
		ILoggerFactory loggerFactory,
		IAuthorizeService authorizer)
	{
		_loggerFactory = loggerFactory;
		_authorizer = authorizer;
	}

	public async Task<AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>> AuthenticatedPipeline<TRequestPayload, TRequestResult>(
		HttpRequest request,
		List<Permission> permissions)
	{
		ILogger<AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>> logger = _loggerFactory.CreateLogger<AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>>();

		var middleware = await AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>.Instance(request, permissions, _authorizer.AuthorizeAsync, logger);

		return middleware;
	}
	
	public async Task<AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>> AnonymousPipeline<TRequestPayload, TRequestResult>(HttpRequest request)
	{
		ILogger<AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>> logger = _loggerFactory.CreateLogger<AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>>();

		var middleware = await AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>.Instance(request, logger);

		return middleware;
	}	

	public async Task<AzureFunctionsHttpMiddleware<object, TRequestResult>> AuthenticatedPipeline<TRequestResult>(
		HttpRequest request,
		List<Permission> permissions)
	{
		ILogger<AzureFunctionsHttpMiddleware<object, TRequestResult>> logger = _loggerFactory.CreateLogger<AzureFunctionsHttpMiddleware<object, TRequestResult>>();

		var middleware = await AzureFunctionsHttpMiddleware<object, TRequestResult>.Instance(request, permissions, _authorizer.AuthorizeAsync, logger);

		return middleware;
	}
	
	public async Task<AzureFunctionsHttpMiddleware<object, TRequestResult>> AnonymousPipeline<TRequestResult>(HttpRequest request)
	{
		ILogger<AzureFunctionsHttpMiddleware<object, TRequestResult>> logger = _loggerFactory.CreateLogger<AzureFunctionsHttpMiddleware<object, TRequestResult>>();

		var middleware = await AzureFunctionsHttpMiddleware<object, TRequestResult>.Instance(request, logger);

		return middleware;
	}
}