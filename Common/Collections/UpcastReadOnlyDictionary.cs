using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OpenTrenches.Common.Collections;

/// <summary>
/// Upcasts a dictionary of <typeparamref name="TOrig"/> values into <typeparamref name="TCast"/>
/// </summary>
/// <typeparam name="TKey">Key</typeparam>
/// <typeparam name="TOrig">Original value of dictionary</typeparam>
/// <typeparam name="TCast">New value of this dictionary</typeparam>
public sealed class UpcastReadOnlyDictionary<TKey, TOrig, TCast>(
    IReadOnlyDictionary<TKey, TOrig> source
) : IReadOnlyDictionary<TKey, TCast> where TOrig : TCast
{
    private readonly IReadOnlyDictionary<TKey, TOrig> _source = source ?? throw new ArgumentNullException(nameof(source));

    public int Count => _source.Count;

    public IEnumerable<TKey> Keys => _source.Keys;

    public IEnumerable<TCast> Values => _source.Values.Cast<TCast>();

    public TCast this[TKey key] => _source[key];

    public bool ContainsKey(TKey key) => _source.ContainsKey(key);

    public bool TryGetValue(TKey key, out TCast value)
    {
        if (_source.TryGetValue(key, out var orig))
        {
            value = orig;
            return true;
        }

        value = default!;
        return false;
    }

    public IEnumerator<KeyValuePair<TKey, TCast>> GetEnumerator()
    {
        foreach (var kv in _source)
            yield return new KeyValuePair<TKey, TCast>(kv.Key, kv.Value);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}