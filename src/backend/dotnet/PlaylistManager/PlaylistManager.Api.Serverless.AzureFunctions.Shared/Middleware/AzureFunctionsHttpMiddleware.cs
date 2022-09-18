using System.Security.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PlaylistManager.Api.Serverless.AzureFunctions.Utils;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Api.Serverless.AzureFunctions.Middleware;

	public class AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>
	{
		private UserSession _session;
		private HttpRequest _request;
		private Func<HttpRequest, UserSession, Task> _requestValidationAction;
		private Func<TRequestPayload, UserSession, Task> _requestPayloadValidationAction;
		private Func<HttpRequest, List<Permission>, Task<OperationResult<UserSession>>> _authenticationFunc;
		private Exception _exception;
		private bool _isExceptionThrown;
		private Func<HttpRequest, TRequestPayload, UserSession, Task<TRequestResult>> _action;
		private TRequestResult _actionResult;
		private ILogger<AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>> _logger;
		private TRequestPayload _requestPayload;
		private string _endpoint;
		private List<Permission> _permissions;
		private IActionResult _actionResultResponse;

		public static async Task<AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>> Instance(
			HttpRequest request,
			List<Permission> privileges,
			Func<HttpRequest, List<Permission>, Task<OperationResult<UserSession>>> authenticationFunc,
			ILogger<AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>> logger
			)
		{
			var middleware = new AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>
			{
				_authenticationFunc = authenticationFunc,
				_request = request,
				_endpoint = request.Path,
				_permissions = privileges,
				_logger = logger
			};

			await middleware.AuthenticateAsync();
			await middleware.DeserializePayloadIfAny();

			return middleware;
		}
		
		public static async Task<AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>> Instance(
			HttpRequest request,
			ILogger<AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>> logger
		)
		{
			var middleware = new AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>
			{
				_authenticationFunc = null,
				_request = request,
				_endpoint = request.Path,
				_permissions = new List<Permission>(),
				_logger = logger
			};

			await middleware.DeserializePayloadIfAny();
			return middleware;
		}		

		public AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult> WithRequestPayloadValidation(Func<TRequestPayload, UserSession, Task> requestPayloadValidationAction)
		{
			_requestPayloadValidationAction = requestPayloadValidationAction;
			return this;
		}

		public AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult> WithRequestPayloadValidation(Action<TRequestPayload, UserSession> requestPayloadValidationAction)
		{
			_requestPayloadValidationAction = new Func<TRequestPayload, UserSession, Task>((request, session) =>
			{
				requestPayloadValidationAction.Invoke(request, session);
				return Task.CompletedTask;
			});
			return this;
		}

		public AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult> WithRequestValidation(Func<HttpRequest, UserSession, Task> requestValidationAction)
		{
			_requestValidationAction = requestValidationAction;
			return this;
		}

		public AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult> WithRequestValidation(Action<HttpRequest, UserSession> requestValidationAction)
		{
			_requestValidationAction = new Func<HttpRequest, UserSession, Task>((request, session) =>
			{
				requestValidationAction.Invoke(request, session);
				return Task.CompletedTask;
			});

			return this;
		}

		public AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult> WithExecutingAction(Func<HttpRequest, TRequestPayload, UserSession, Task<TRequestResult>> actionTaskBuilder)
		{
			try
			{
				if (_authenticationFunc is not null && _session is null)
				{
					_logger.LogWarning($"Request is not authenticated");
					throw new AuthenticationException("Request is not authenticated.");
				}

				_action = actionTaskBuilder;
				return this;
			}
			catch (Exception e)
			{
				SaveException(e);
				return this;
			}
		}

		public async Task<IActionResult> ToIActionResultAsync()
		{
			if (_isExceptionThrown) return HandleExceptionAsIActionResult();

			await ValidateRequest();

			if (_isExceptionThrown) return HandleExceptionAsIActionResult();

			await ValidateRequestPayload();

			if (_isExceptionThrown) return HandleExceptionAsIActionResult();

			await ExecuteAction();

			if (_isExceptionThrown) return HandleExceptionAsIActionResult();

			_actionResultResponse = new OkObjectResult(_actionResult);

			return _actionResultResponse;
		}

		public async Task<OperationResult<TRequestResult>> OperationResultAsync()
		{
			if (_isExceptionThrown) return OperationResult<TRequestResult>.Failure(_exception);

			await ValidateRequest();

			if (_isExceptionThrown) return OperationResult<TRequestResult>.Failure(_exception);

			await ValidateRequestPayload();

			if (_isExceptionThrown) return OperationResult<TRequestResult>.Failure(_exception);

			await ExecuteAction();

			if (_isExceptionThrown) return OperationResult<TRequestResult>.Failure(_exception);

			return OperationResult<TRequestResult>.Success(_actionResult);
		}

		private async Task DeserializePayloadIfAny()
		{
			if (
				_request.Method == HttpMethod.Post.ToString() ||
				_request.Method == HttpMethod.Put.ToString()
			)
			{
				try
				{
					string requestBody = await new StreamReader(_request.Body).ReadToEndAsync();
					var deserializedPayload = JsonConvert.DeserializeObject<TRequestPayload>(requestBody);

					if (deserializedPayload is null) throw new Exception($"Request body is required for this request");
					_requestPayload = deserializedPayload;
				}
				catch (Exception e)
				{
					SaveException(e);
				}
			}
		}

		private async Task AuthenticateAsync()
		{
			try
			{
				var authenticationResult = await _authenticationFunc.Invoke(_request, _permissions);
				if (authenticationResult.IsFailure)
				{
					SaveException(authenticationResult.FailureReason);
					return;
				}

				_session = authenticationResult.Payload;
			}
			catch (Exception e)
			{
				SaveException(e);
			}
		}

		private IActionResult HandleExceptionAsIActionResult()
		{
			_actionResultResponse = ActionResultMapper.Map(_exception);
			return _actionResultResponse;
		}

		private void SaveException(Exception e)
		{
			_logger.LogWarning($"[{_endpoint}] => {e.Message}");
			_logger.LogWarning($"[{_endpoint}] => {e.StackTrace}");

			if (!(_exception is null)) return;

			_exception = e;
			_isExceptionThrown = true;
		}

		private async Task ExecuteAction()
		{
			try
			{
				_logger.LogWarning($"[{_endpoint}] => Executing action...");
				_actionResult = await _action.Invoke(_request, _requestPayload, _session);
			}
			catch (Exception e)
			{
				_logger.LogCritical($"[{_endpoint}] => Caught exception while executing action...");
				SaveException(e);
			}
		}

		private async Task ValidateRequest()
		{
			if (_requestValidationAction is null) return;

			try
			{
				_logger.LogWarning($"[{_endpoint}] => Validating request...");
				var requestValidationTask = _requestValidationAction.Invoke(_request, _session);
				await requestValidationTask;
			}
			catch (Exception e)
			{
				_logger.LogWarning($"[{_endpoint}] => Caught exception while validating request...");
				SaveException(e);
			}
		}

		private async Task ValidateRequestPayload()
		{
			if (_requestPayloadValidationAction is null) return;

			try
			{
				_logger.LogCritical($"[{_endpoint}] => Validating request...");
				var requestPayloadValidationTask = _requestPayloadValidationAction.Invoke(_requestPayload, _session);
				await requestPayloadValidationTask;
			}
			catch (Exception e)
			{
				_logger.LogCritical($"[{_endpoint}] => Caught exception while validating request...");
				SaveException(e);
			}
		}
	}