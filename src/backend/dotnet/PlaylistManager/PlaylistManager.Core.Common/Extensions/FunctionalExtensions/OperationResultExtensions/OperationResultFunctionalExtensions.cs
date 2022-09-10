using PlaylistManager.Core.Common.Models;

namespace PlaylistManager.Core.Common.Extensions.FunctionalExtensions.OperationResultExtensions;

public static class OperationResultFunctionalExtensions
{
	public static IOperationResultMapExecutionOnFailureStep<T, TOut> OnSuccess<T, TOut>(this OperationResult<T> result, Func<T, TOut> onSuccess)
	{
		var executionStart = OperationResultMapExecution<T, TOut>.Setup(result);
		return executionStart.OnSuccess(onSuccess);
	}

	public static IOperationResultMapExecutionOnFailureAsyncStep<T, TOut> OnSuccess<T, TOut>(this OperationResult<T> result, Func<T, Task<TOut>> onSuccess)
	{
		var executionStart = OperationResultMapExecution<T, TOut>.Setup(result);
		return executionStart.OnSuccess(onSuccess);
	}

	public static IOperationResultSwitchExecutionOnFailureStep<T> OnSuccess<T>(this OperationResult<T> result, Action<T> onSuccess)
	{
		var executionStart = OperationResultSwitchExecution<T>.Setup(result);
		return executionStart.OnSuccess(onSuccess);
	}

	public static IOperationResultSwitchExecutionOnFailureAsyncStep<T> OnSuccess<T>(this OperationResult<T> result, Task onSuccess)
	{
		var executionStart = OperationResultSwitchExecution<T>.Setup(result);
		return executionStart.OnSuccess(onSuccess);
	}

	public static OperationResult<TOut> Map<T, TOut>(this OperationResult<T> result, Func<T, TOut> mapper)
		=> result.IsSuccess
			? OperationResult<TOut>.Success(mapper(result.Payload))
			: OperationResult<TOut>.Failure(result.FailureReason);
}