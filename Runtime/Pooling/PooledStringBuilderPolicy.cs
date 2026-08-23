using System.Text;

namespace Aurora.Pooling
{
    /// <summary>
    /// A strategy for managing mutable strings in the pool.
    /// </summary>
    public class PooledStringBuilderPolicy : IPooledObjectPolicy<StringBuilder>
    {
        /// <summary>
        /// Gets or sets the initial capacity of pooled mutable strings.
        /// </summary>
        public int InitialCapacity { get; set; } = 256;

        /// <summary>
        /// Gets or sets the maximum capacity of mutable strings allowed into the pool.
        /// </summary>
        public int MaximumRetainedCapacity { get; set; } = 4096;

        /// <inheritdoc />
        public StringBuilder Create()
        {
            return new StringBuilder(InitialCapacity);
        }

        /// <inheritdoc />
        public void Get(StringBuilder obj)
        {
        }

        /// <inheritdoc />
        public bool Return(StringBuilder obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.Capacity > MaximumRetainedCapacity)
            {
                return false;
            }
            obj.Length = 0;
            return true;
        }

        /// <inheritdoc />
        public void Dispose(StringBuilder obj)
        {
        }
    }
}
