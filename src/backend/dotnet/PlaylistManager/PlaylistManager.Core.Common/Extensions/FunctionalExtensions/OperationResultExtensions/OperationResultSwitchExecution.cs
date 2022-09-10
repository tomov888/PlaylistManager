using PlaylistManager.Core.Common.Models;

namespace PlaylistManager.Core.Common.Extensions.FunctionalExtensions.OperationResultExtensions;

public class OperationResultSwitchExecution<T> :
	IOperationResultSwitchExecutionFinallyStep<T>,
	IOperationResultSwitchExecutionOnSuccessStep<T>,
	IOperationResultSwitchExecutionOnFailureStep<T>,
	IOperationResultSwitchExecutionOnSuccessAsyncStep<T>,
	IOperationResultSwitchExecutionOnFailureAsyncStep<T>,
	IOperationResultSwitchExecutionFinallyAsyncStep<T>
{
	private OperationResultSwitchExecution(OperationResult<T> result)
	{
		_operationResult = result;
	}

	private Action<T> _onSuccess { get; set; }
	private Action<Exception> _onFailure { get; set; }
	private Task _onSuccessTask { get; set; }
	private Task _onFailureTask { get; set; }
	private OperationResult<T> _operationResult { get; }

	public static OperationResultSwitchExecution<T> Setup(OperationResult<T> result)
	{
		return new OperationResultSwitchExecution<T>(result);
	}

	public IOperationResultSwitchExecutionOnFailureStep<T> OnSuccess(Action<T> onSuccess)
	{
		_onSuccess = onSuccess;
		return this;
	}

	public IOperationResultSwitchExecutionFinallyStep<T> OnFailure(Action<Exception> onFailure)
	{
		_onFailure = onFailure;
		return this;
	}

	public IOperationResultSwitchExecutionOnFailureAsyncStep<T> OnSuccess(Task onSuccess)
	{
		_onSuccessTask = onSuccess;
		return this;
	}

	public IOperationResultSwitchExecutionFinallyAsyncStep<T> OnFailure(Task onFailure)
	{
		_onFailureTask = onFailure;
		return this;
	}

	public OperationResult<T> Finally()
	{
		if (!_operationResult.IsSuccess)
		{
			_onFailure(_operationResult.FailureReason);
			return _operationResult;
		}

		_onSuccess(_operationResult.Payload);
		return _operationResult;
	}

	public async Task<OperationResult<T>> FinallyAsync()
	{
		if (!_operationResult.IsSuccess)
		{
			await _onFailureTask;
			return _operationResult;
		}

		await _onSuccessTask;
		return _operationResult;
	}
}