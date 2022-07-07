using System.Collections.Concurrent;

namespace ConsoleLogFilter;

/// <summary>
/// Awaitable queue
/// </summary>
internal class AwaitableQueue<T>
{
    private SemaphoreSlim _queuing = new(0);
    private ConcurrentQueue<T> _queue = new();

    /// <summary>
    /// Enqueue
    /// </summary>
    public void Enqueue(in T item)
    {
        _queue.Enqueue(item);
        _queuing.Release();
    }

    /// <summary>
    /// Await queuing and Dequeue
    /// </summary>
    public async Task<T> DequeueAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            await _queuing.WaitAsync(cancellationToken);

            if (_queue.TryDequeue(out var item)) return item;
        }
    }
}
