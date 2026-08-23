using System.IO;
using System.Reflection;

namespace Aurora.Pooling
{
    /// <summary>
    /// A strategy for managing memory streams in the pool.
    /// </summary>
    public class PooledMemoryStreamPolicy : IPooledObjectPolicy<MemoryStream>
    {
        private static readonly FieldInfo MemoryStreamIsExpandableFieldInfo = typeof(MemoryStream).GetField(
            "_expandable",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        /// <summary>
        /// Gets or sets the initial capacity of pooled memory streams.
        /// </summary>
        public int InitialCapacity { get; set; } = 256;

        /// <summary>
        /// Gets or sets the maximum capacity of memory streams allowed into the pool.
        /// </summary>
        public int MaximumRetainedCapacity { get; set; } = 4096;

        private static bool IsExpandable(MemoryStream memoryStream)
        {
            return (bool)MemoryStreamIsExpandableFieldInfo.GetValue(memoryStream);
        }

        /// <inheritdoc />
        public MemoryStream Create()
        {
            return new MemoryStream(InitialCapacity);
        }

        /// <inheritdoc />
        public void Get(MemoryStream obj)
        {
        }

        /// <inheritdoc />
        public bool Return(MemoryStream obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!obj.CanRead)
            {
                return false;
            }
            if (obj.GetType() != typeof(MemoryStream))
            {
                return false;
            }
            if (!IsExpandable(obj))
            {
                return false;
            }
            if (obj.Capacity > MaximumRetainedCapacity)
            {
                return false;
            }
            obj.SetLength(0L);
            return true;
        }

        /// <inheritdoc />
        public void Dispose(MemoryStream obj)
        {
        }
    }
}
