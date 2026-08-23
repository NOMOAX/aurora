using System.Collections.Generic;

namespace Aurora.Pooling
{
    /// <summary>
    /// 管理池中的哈希集的策略。
    /// </summary>
    /// <typeparam name="T">哈希集的成员的类型。</typeparam>
    public class PooledHashSetPolicy<T> : IPooledObjectPolicy<HashSet<T>>
    {
        /// <summary>
        /// 获取或设置池化的哈希集的初始容量。
        /// </summary>
        /// <remarks>由于目标框架设置为 .NET Framework 4.7.1，因此这个值暂时无法使用。</remarks>
        public int InitialCapacity { get; set; } = 17;

        /// <summary>
        /// 获取或设置允许被放入池的哈希集的最大长度。
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
