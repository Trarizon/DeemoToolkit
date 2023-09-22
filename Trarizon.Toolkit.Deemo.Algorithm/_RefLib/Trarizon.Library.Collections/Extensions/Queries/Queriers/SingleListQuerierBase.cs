namespace Trarizon.Library.Collections.Extensions.Queries.Queriers;
internal abstract class SingleListQuerierBase<T> : ListQuerier<T>
{
	protected readonly IList<T> _list;

	protected SingleListQuerierBase(IList<T> list)
	{
		_list = list;
	}

	public override T Current => _list[_state];

	public override bool MoveNext() => ++_state < Count;
}
