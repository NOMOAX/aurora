using System.Collections.Generic;

namespace Aurora.Pooling
{
    /// <summary>
    /// 管理池中的队列的策略。
    /// </summary>
    /// <typeparam name="T">队列的成员的类型。</typeparam>
    public class PooledQueuePolicy<T> : IPooledObjectPolicy<Queue<T>>
    {
        /// <summary>
        /// 获取或设置池化的队列的初始容量。
        /// </summary>
        public int InitialCapacity { get; set; } = 16;

        /// <summary>
        /// 获取或设置允许被放入池的队列的最大长度。
        /// </summary>
        public int MaximumRetainedCount { get; set; } = 256;

        /// <inheritdoc />
        public Queue<T> Create()
        {
            return new Queue<T>(InitialCapacity);
        }

        /// <inheritdoc />
        public void Get(Queue<T> obj)
        {
        }

        /// <inheritdoc />
        public bool Return(Queue<T> obj)
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
        public void Dispose(Queue<T> obj)
        {
            obj?.Clear();
        }
    }
}
