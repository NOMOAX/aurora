using System.Collections.Generic;

namespace Aurora.Pooling
{
    /// <summary>
    /// 管理池中的列表的策略。
    /// </summary>
    /// <typeparam name="T">列表的成员的类型。</typeparam>
    public class PooledListPolicy<T> : IPooledObjectPolicy<List<T>>
    {
        /// <summary>
        /// 获取或设置池化的列表的初始容量。
        /// </summary>
        public int InitialCapacity { get; set; } = 16;

        /// <summary>
        /// 获取或设置允许被放入池的列表的最大容量。
        /// </summary>
        public int MaximumRetainedCapacity { get; set; } = 256;

        /// <inheritdoc />
        public List<T> Create()
        {
            return new List<T>(InitialCapacity);
        }

        /// <inheritdoc />
        public void OnGet(List<T> obj)
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
