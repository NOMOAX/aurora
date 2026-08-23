using System.Collections.Generic;

namespace Aurora.Pooling
{
    /// <summary>
    /// A strategy for managing lists in the pool.
    /// </summary>
    /// <typeparam name="T">The type of the list's members.</typeparam>
    public class PooledListPolicy<T> : IPooledObjectPolicy<List<T>>
    {
        /// <summary>
        /// Gets or sets the initial capacity of pooled lists.
        /// </summary>
        public int InitialCapacity { get; set; } = 16;

        /// <summary>
        /// Gets or sets the maximum capacity of lists allowed into the pool.
        /// </summary>
        public int MaximumRetainedCapacity { get; set; } = 256;

        /// <inheritdoc />
        public List<T> Create()
        {
            return new List<T>(InitialCapacity);
        }

        /// <inheritdoc />
        public void Get(List<T> obj)
        {
        }

        /// <inheritdoc />
        public bool Return(List<T> obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.Capacity > MaximumRetainedCapacity)
            {
                return false;
            }
            obj.Clear();
            return true;
        }

        /// <inheritdoc />
        public void Dispose(List<T> obj)
        {
            obj?.Clear();
        }
    }
}
