using System.Collections;

namespace Trarizon.Library.Collections.CollectionQuery;
static partial class ReadOnlyListQuery
{
    private sealed class ReverseIterator<T> : IEnumerator<T>, IEnumerable<T>
    {
        // Conflict validation
        private bool _useSelf;
        private readonly int _threadId;
        // Values
        private int _index;
        private readonly IReadOnlyList<T> _list;

        public T Current => _list[_index];

        public ReverseIterator(IReadOnlyList<T> list) : this(list, true) { }

        private ReverseIterator(IReadOnlyList<T> list, bool createByLinq)
        {
            _useSelf = createByLinq;
            _threadId = Environment.CurrentManagedThreadId;

            _list = list;
            _index = _list.Count;
        }

        public void Dispose() { }
        public IEnumerator<T> GetEnumerator()
        {
            if (_useSelf && _threadId == Environment.CurrentManagedThreadId) {
                _useSelf = false;
                return this;
            }
            else
                return new ReverseIterator<T>(_list, false);
        }

        public bool MoveNext() => --_index >= 0;
        public void Reset() => _index = _list.Count;

        object? IEnumerator.Current => Current;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
