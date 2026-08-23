using System.Collections.Generic;

namespace Aurora.Pooling
{
    /// <summary>
    /// A strategy for managing hash sets in the pool.
    /// </summary>
    /// <typeparam name="T">The type of the hash set's members.</typeparam>
    public class PooledHashSetPolicy<T> : IPooledObjectPolicy<HashSet<T>>
    {
        /// <summary>
        /// Gets or sets the initial capacity of pooled hash sets.
        /// </summary>
        /// <remarks>Because the target framework is set to .NET Framework 4.7.1, this value is temporarily unusable.</remarks>
        public int InitialCapacity { get; set; } = 17;

        /// <summary>
        /// Gets or sets the maximum length of hash sets allowed into the pool.
        /// </summary>
        public int MaximumRetainedCount { get; set; } = 293;

        /// <inheritdoc />
        public HashSet<T> Create()
        {
            return new HashSet<T>(InitialCapacity, EqualityComparer<T>.Default);
        }

        /// <inheritdoc />
        public void Get(HashSet<T> obj)
        {
        }

        /// <inheritdoc />
        public bool Return(HashSet<T> obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.Count > MaximumRetainedCount)
            {
                return false;
            }
            if (obj.Comparer != EqualityComparer<T>.Default)
            {
                return false;
            }
            obj.Clear();
            return true;
        }

        /// <inheritdoc />
        public void Dispose(HashSet<T> obj)
        {
            obj?.Clear();
        }
    }
}
