using System.Collections.Concurrent;

namespace Trarizon.Library.CLParsing.Utility;
internal class CacheDictionary<T>
{
    private readonly Func<Type, T> _factory;
    private readonly ConcurrentDictionary<Type, T> _dict;

    public CacheDictionary(Func<Type, T> factory)
    {
        _factory = factory;
        _dict = new();
    }

    public T Get(Type key) => _dict.GetOrAdd(key, _factory);
}
