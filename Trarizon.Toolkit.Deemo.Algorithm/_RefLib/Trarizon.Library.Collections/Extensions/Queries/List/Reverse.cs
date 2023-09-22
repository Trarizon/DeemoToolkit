using Trarizon.Library.Collections.Extensions.Queries.Queriers;

namespace Trarizon.Library.Collections.Extensions.Queries;
public static partial class ListQuery
{
	public static IList<T> ReverseList<T>(this IList<T> list) => new ReverseQuerier<T>(list);

	private sealed class ReverseQuerier<T> : SingleListQuerierBase<T>
	{
		public ReverseQuerier(IList<T> list) : base(list) { }

		public override T this[int index] => _list[_list.Count - index - 1];
		public override int Count => _list.Count;

		protected override EnumerationQuerier<T> Clone() => new ReverseQuerier<T>(_list);
	}
}
