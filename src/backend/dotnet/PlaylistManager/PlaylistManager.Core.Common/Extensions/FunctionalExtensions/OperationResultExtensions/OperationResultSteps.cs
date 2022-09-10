using PlaylistManager.Core.Common.Models;

namespace PlaylistManager.Core.Common.Extensions.FunctionalExtensions.OperationResultExtensions;

public interface IOperationResultSwitchExecutionOnSuccessStep<T>
{
	IOperationResultSwitchExecutionOnFailureStep<T> OnSuccess(Action<T> onSuccess);
}

public interface IOperationResultSwitchExecutionOnFailureStep<T>
{
	IOperationResultSwitchExecutionFinallyStep<T> OnFailure(Action<Exception> onFailure);
}

public interface IOperationResultSwitchExecutionFinallyStep<T>
{
	OperationResult<T> Finally();
}

public interface IOperationResultSwitchExecutionOnSuccessAsyncStep<T>
{
	IOperationResultSwitchExecutionOnFailureAsyncStep<T> OnSuccess(Task onSuccess);
}

public interface IOperationResultSwitchExecutionOnFailureAsyncStep<T>
{
	IOperationResultSwitchExecutionFinallyAsyncStep<T> OnFailure(Task onFailure);
}

public interface IOperationResultSwitchExecutionFinallyAsyncStep<T>
{
	Task<OperationResult<T>> FinallyAsync();
}

public interface IOperationResultMapExecutionOnSuccessStep<T, TOutput>
{
	IOperationResultMapExecutionOnFailureStep<T, TOutput> OnSuccess(Func<T, TOutput> onSuccess);
}

public interface IOperationResultMapExecutionOnFailureStep<T, TOutput>
{
	IOperationResultMapExecutionFinallyStep<T, TOutput> OnFailure(Func<Exception, TOutput> onFailure);
}

public interface IOperationResultMapExecutionFinallyStep<T, TOutput>
{
	TOutput Finally();
	TOutput Finally(Action action);
}

public interface IOperationResultMapExecutionOnSuccessAsyncStep<T, TOutput>
{
	IOperationResultMapExecutionOnFailureAsyncStep<T, TOutput> OnSuccess(Func<T, Task<TOutput>> onSuccess);
}

public interface IOperationResultMapExecutionOnFailureAsyncStep<T, TOutput>
{
	IOperationResultMapExecutionFinallyAsyncStep<T, TOutput> OnFailure(Func<Exception, Task<TOutput>> onFailure);
}

public interface IOperationResultMapExecutionFinallyAsyncStep<T, TOutput>
{
	Task<TOutput> FinallyAsync();
	Task<TOutput> FinallyAsync(Task task);
}