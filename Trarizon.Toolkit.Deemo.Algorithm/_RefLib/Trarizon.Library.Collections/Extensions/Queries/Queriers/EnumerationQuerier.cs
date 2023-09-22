using System.Collections;

namespace Trarizon.Library.Collections.Extensions.Queries.Queriers;
internal abstract class EnumerationQuerier<T> : IEnumerator<T>, IEnumerable<T>
{
	/// <summary>
	/// This value is -1 before first MoveNext(),
	/// -2 is preserved state, do not use it.
	/// </summary>
	protected int _state;
	private readonly int _threadId;

	public abstract T Current { get; }

	protected EnumerationQuerier()
	{
		_state = -2;
		_threadId = Environment.CurrentManagedThreadId;
	}

	public virtual void Dispose() { }
	public IEnumerator<T> GetEnumerator()
	{
		var rtn = _state == -2 && _threadId == Environment.CurrentManagedThreadId ? this : Clone();
		rtn._state = -1;
		return rtn;
	}

	protected abstract EnumerationQuerier<T> Clone();
	public abstract bool MoveNext();
	public virtual void Reset() => _state = -1;

	object? IEnumerator.Current => Current;
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
