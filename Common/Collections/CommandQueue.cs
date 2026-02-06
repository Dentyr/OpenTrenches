using System.Collections.Generic;

namespace OpenTrenches.Common.Collections;

/// <summary>
/// A queue that can be polled to dump all items from it.
/// </summary>
/// <typeparam name="T"></typeparam>
public class PolledQueue<T>
{
    private List<T> _queue = [];
    private List<T> _swap = [];
    
    public void Enqueue(T value) => _queue.Add(value);

    public IReadOnlyList<T> PollItems()
    {
        (_queue, _swap) = (_swap, _queue);

        _queue.Clear();
        return _swap.AsReadOnly();
    }
}