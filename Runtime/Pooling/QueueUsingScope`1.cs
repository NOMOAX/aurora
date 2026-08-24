using System;
using System.Collections.Generic;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// A using scope for a queue.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the queue.</typeparam>
    public sealed class QueueUsingScope<T> : IDisposable
    {
        private Queue<T> _queue;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueUsingScope{T}"/> class.
        /// </summary>
        /// <param name="queue">This output parameter is assigned an empty queue.</param>
        public QueueUsingScope(out Queue<T> queue)
        {
            _queue = PredefinedPools<T>.Queue.Get();
            queue  = _queue;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var queue = _queue;
            if (queue != null && Interlocked.CompareExchange(ref _queue, null, queue) == queue)
            {
                PredefinedPools<T>.Queue.Return(queue);
            }
        }
    }
}
