using Aurora.Collections;

namespace Aurora.Pooling
{
    /// <summary>
    /// A strategy for managing deques in the pool.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the deque.</typeparam>
    public class PooledDequePolicy<T> : IPooledObjectPolicy<Deque<T>>
    {
        /// <summary>
        /// Gets or sets the initial capacity of pooled deques.
        /// </summary>
        public int InitialCapacity { get; set; } = 16;

        /// <summary>
        /// Gets or sets the maximum length of deques allowed into the pool.
        /// </summary>
        public int MaximumRetainedCount { get; set; } = 256;

        /// <inheritdoc />
        public Deque<T> Create()
        {
            return new Deque<T>(InitialCapacity);
        }

        /// <inheritdoc />
        public void Get(Deque<T> obj)
        {
        }

        /// <inheritdoc />
        public bool Return(Deque<T> obj)
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
        public void Dispose(Deque<T> obj)
        {
            obj?.Clear();
        }
    }
}
