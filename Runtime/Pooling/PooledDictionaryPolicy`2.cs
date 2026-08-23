using System.Collections.Generic;

namespace Aurora.Pooling
{
    /// <summary>
    /// A strategy for managing dictionaries in the pool.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public class PooledDictionaryPolicy<TKey, TValue> : IPooledObjectPolicy<Dictionary<TKey, TValue>>
    {
        /// <summary>
        /// Gets or sets the initial capacity of pooled dictionaries.
        /// </summary>
        public int InitialCapacity { get; set; } = 17;

        /// <summary>
        /// Gets or sets the maximum length of dictionaries allowed into the pool.
        /// </summary>
        public int MaximumRetainedCount { get; set; } = 293;

        /// <inheritdoc />
        public Dictionary<TKey, TValue> Create()
        {
            return new Dictionary<TKey, TValue>(InitialCapacity, EqualityComparer<TKey>.Default);
        }

        /// <inheritdoc />
        public void Get(Dictionary<TKey, TValue> obj)
        {
        }

        /// <inheritdoc />
        public bool Return(Dictionary<TKey, TValue> obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.Count > MaximumRetainedCount)
            {
                return false;
            }
            if (obj.Comparer != EqualityComparer<TKey>.Default)
            {
                return false;
            }
            obj.Clear();
            return true;
        }

        /// <inheritdoc />
        public void Dispose(Dictionary<TKey, TValue> obj)
        {
            obj?.Clear();
        }
    }
}
