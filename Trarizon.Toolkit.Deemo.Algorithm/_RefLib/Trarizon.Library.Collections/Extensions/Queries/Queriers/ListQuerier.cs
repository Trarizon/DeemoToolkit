namespace Trarizon.Library.Collections.Extensions.Queries.Queriers;
internal abstract class ListQuerier<T> : EnumerationQuerier<T>, IList<T>, IReadOnlyList<T>
{
	public abstract int Count { get; }

	bool ICollection<T>.IsReadOnly => true;

	public abstract T this[int index] { get; }

	protected ListQuerier() { }

	T IList<T>.this[int index] { get => this[index]; set => ThrowHelper.ThrowNotSupport(); }
	int IList<T>.IndexOf(T item) => ThrowHelper.ThrowNotSupport<int>();
	void IList<T>.Insert(int index, T item) => ThrowHelper.ThrowNotSupport("Cannot change ListQuerier");
	void IList<T>.RemoveAt(int index) => ThrowHelper.ThrowNotSupport("Cannot change ListQuerier");
	void ICollection<T>.Add(T item) => ThrowHelper.ThrowNotSupport("Cannot change ListQuerier");
	void ICollection<T>.Clear() => ThrowHelper.ThrowNotSupport("Cannot change ListQuerier");
	bool ICollection<T>.Contains(T item) => this.Contains(item);
	void ICollection<T>.CopyTo(T[] array, int arrayIndex) => ThrowHelper.ThrowNotSupport("Cannot change ListQuerier");
	bool ICollection<T>.Remove(T item) => ThrowHelper.ThrowNotSupport<bool>("Cannot change ListQuerier");
}
