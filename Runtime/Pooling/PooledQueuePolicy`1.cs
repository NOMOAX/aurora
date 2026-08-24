using System.Collections.Generic;

namespace Aurora.Pooling
{
    /// <summary>
    /// A strategy for managing queues in the pool.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the queue.</typeparam>
    public class PooledQueuePolicy<T> : IPooledObjectPolicy<Queue<T>>
    {
        /// <summary>
        /// Gets or sets the initial capacity of pooled queues.
        /// </summary>
        public int InitialCapacity { get; set; } = 16;

        /// <summary>
        /// Gets or sets the maximum length of queues allowed into the pool.
        /// </summary>
        public int MaximumRetainedCount { get; set; } = 256;

        /// <inheritdoc />
        public Queue<T> Create()
        {
            return new Queue<T>(InitialCapacity);
        }

        /// <inheritdoc />
        public void Get(Queue<T> obj)
        {
        }

        /// <inheritdoc />
        public bool Return(Queue<T> obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.Count > MaximumRetainedCount)
            {
                return false;
            }
            obj.Clear();
            return true;
        }

        /// <inheritdoc />
        public void Dispose(Queue<T> obj)
        {
            obj?.Clear();
        }
    }
}
