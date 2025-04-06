using System;
using System.IO;
using System.Reflection;

namespace Aurora.Pooling
{
    /// <summary>
    /// 管理池中的内存流的策略。
    /// </summary>
    public class PooledMemoryStreamPolicy : IPooledObjectPolicy<MemoryStream>
    {
        private static readonly Type MemoryStreamType = typeof(MemoryStream);

        private static readonly FieldInfo MemoryStreamIsExpandableFieldInfo = MemoryStreamType.GetField(
            "_expandable",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        /// <summary>
        /// 获取或设置池化的内存流的初始容量。
        /// </summary>
        public int InitialCapacity { get; set; } = 256;

        /// <summary>
        /// 获取或设置允许被放入池的内存流的最大容量。
        /// </summary>
        public int MaximumRetainedCapacity { get; set; } = 4096;

        private static bool IsExpandable(MemoryStream memoryStream)
        {
            return (bool) MemoryStreamIsExpandableFieldInfo.GetValue(memoryStream);
        }

        /// <inheritdoc />
        public MemoryStream Create()
        {
            return new MemoryStream(InitialCapacity);
        }

        /// <inheritdoc />
        public void OnGet(MemoryStream obj)
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
            if (obj.GetType() != MemoryStreamType)
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
            if (obj == null)
            {
                return;
            }
            if (!obj.CanRead)
            {
                return;
            }
            if (obj.GetType() != MemoryStreamType)
            {
                return;
            }
            if (!IsExpandable(obj))
            {
                return;
            }
            obj.SetLength(0L);
        }
    }
}
