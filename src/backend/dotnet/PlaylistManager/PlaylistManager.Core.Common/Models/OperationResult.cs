namespace PlaylistManager.Core.Common.Models;

public class OperationResult<T>
{
	public bool IsSuccess { get; set; }
	public T Payload { get; set; }
	public Exception FailureReason { get; set; }

	public bool IsFailure => !IsSuccess;

	public static OperationResult<T> Success(T result)
	{
		return new OperationResult<T> { IsSuccess = true, Payload = result };
	}

	public static OperationResult<T> Success()
	{
		return new OperationResult<T> { IsSuccess = true, Payload = default };
	}

	public static OperationResult<T> Failure(Exception e)
	{
		return new OperationResult<T> { IsSuccess = false, FailureReason = e };
	}

	public static OperationResult<T> Failure(Exception e, T defaultValue)
	{
		return new OperationResult<T> { IsSuccess = false, FailureReason = e, Payload = defaultValue };
	}

	public OperationResult<T> OnSuccessResult(Action<T> action) 
	{
		if (IsSuccess) action.Invoke(Payload);
		return this;
	}

	public OperationResult<T> OnFailureResult(Action<Exception> action)
	{
		if (IsFailure) action.Invoke(FailureReason);
		return this;
	}
}