using PlaylistManager.Core.Common.Models;

namespace PlaylistManager.Core.Common.Extensions.FunctionalExtensions.OperationResultExtensions;

	public class OperationResultMapExecution<T, TOutput> :
	IOperationResultMapExecutionOnSuccessStep<T, TOutput>,
	IOperationResultMapExecutionOnFailureStep<T, TOutput>,
	IOperationResultMapExecutionFinallyStep<T, TOutput>,
	IOperationResultMapExecutionOnSuccessAsyncStep<T, TOutput>,
	IOperationResultMapExecutionOnFailureAsyncStep<T, TOutput>,
	IOperationResultMapExecutionFinallyAsyncStep<T, TOutput>
	{
		private OperationResultMapExecution(OperationResult<T> result)
		{
			_operationResult = result;
		}

		private Func<T, TOutput> _onSuccess { get; set; }
		private Func<Exception, TOutput> _onFailure { get; set; }
		private Func<T, Task<TOutput>> _onSuccessTask { get; set; }
		private Func<Exception, Task<TOutput>> _onFailureTask { get; set; }
		private OperationResult<T> _operationResult { get; }

		public static OperationResultMapExecution<T, TOutput> Setup(OperationResult<T> result)
		{
			return new OperationResultMapExecution<T, TOutput>(result);
		}

		public IOperationResultMapExecutionOnFailureStep<T, TOutput> OnSuccess(Func<T, TOutput> onSuccess)
		{
			_onSuccess = onSuccess;
			return this;
		}

		public IOperationResultMapExecutionFinallyStep<T, TOutput> OnFailure(Func<Exception, TOutput> onFailure)
		{
			_onFailure = onFailure;
			return this;
		}

		public IOperationResultMapExecutionOnFailureAsyncStep<T, TOutput> OnSuccess(Func<T, Task<TOutput>> onSuccess)
		{
			_onSuccessTask = onSuccess;
			return this;
		}

		public IOperationResultMapExecutionFinallyAsyncStep<T, TOutput> OnFailure(Func<Exception, Task<TOutput>> onFailure)
		{
			_onFailureTask = onFailure;
			return this;
		}

		public TOutput Finally()
		{
			return _operationResult.IsSuccess
				? _onSuccess(_operationResult.Payload)
				: _onFailure(_operationResult.FailureReason);
		}

		public TOutput Finally(Action action)
		{
			action.Invoke();

			return _operationResult.IsSuccess
				? _onSuccess(_operationResult.Payload)
				: _onFailure(_operationResult.FailureReason);
		}

		public async Task<TOutput> FinallyAsync(Task task)
		{
			await task;	

			return _operationResult.IsSuccess
				? await _onSuccessTask(_operationResult.Payload)
				: await _onFailureTask(_operationResult.FailureReason);
		}

		public async Task<TOutput> FinallyAsync()
		{
			return _operationResult.IsSuccess
				? await _onSuccessTask(_operationResult.Payload)
				: await _onFailureTask(_operationResult.FailureReason);
		}
	}