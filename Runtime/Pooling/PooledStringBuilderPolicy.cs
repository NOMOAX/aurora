using System.Text;

namespace Aurora.Pooling
{
    /// <summary>
    /// 管理池中的可变字符串的策略。
    /// </summary>
    public class PooledStringBuilderPolicy : IPooledObjectPolicy<StringBuilder>
    {
        /// <summary>
        /// 获取或设置池化的可变字符串的初始容量。
        /// </summary>
        public int InitialCapacity { get; set; } = 256;

        /// <summary>
        /// 获取或设置允许被放入池的可变字符串的最大容量。
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
