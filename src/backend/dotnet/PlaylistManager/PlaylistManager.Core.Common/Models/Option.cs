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

	public Option<TOut> Map<TOut>(Func<T, TOut> mapper)
		=> HasValue 
			? Option<TOut>.Of(mapper.Invoke(Value)) 
			: Option<TOut>.None;
}