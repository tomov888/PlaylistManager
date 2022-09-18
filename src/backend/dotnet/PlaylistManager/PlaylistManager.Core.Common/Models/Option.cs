namespace PlaylistManager.Core.Common.Models;

public class Option<T> : IEnumerable<T>
{
	private readonly T[] _value;

	private Option(T[] value)
	{
		_value = value;
	}

	public static Option<T> Of(T element)
	{
		return element is null
			? None
			: new Option<T>(new[] { element });
	}

	public static Option<T> None => new Option<T>(new T[0]);
	public bool HasValue => _value.Any();
	public bool Empty => !_value.Any();
	public T Value => _value.First();

	public IEnumerator<T> GetEnumerator()
	{
		return ((IEnumerable<T>)_value).GetEnumerator();
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public Option<T> OnValue(Action<T> action)
	{
		if(HasValue) action.Invoke(Value);
		
		return this;
	}
	
	public Option<T> OnValue(Action action)
	{
		if(HasValue) action.Invoke();
		
		return this;
	}

	public Option<T> OnEmpty(Action action)
	{
		if (Empty) action.Invoke();

		return this;
	}


	public Option<TOut> Map<TOut>(Func<T, TOut> mapper)
		=> HasValue 
			? Option<TOut>.Of(mapper.Invoke(Value)) 
			: Option<TOut>.None;
	
	public T TryUnwrap()
		=> HasValue 
			? Value 
			: throw new Exception($"Option<{typeof(T)}> holds no value.");

	public T Unwrap(T defaultValue)
		=> HasValue 
			? Value 
			: defaultValue;

}