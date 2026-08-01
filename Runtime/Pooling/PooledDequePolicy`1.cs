using Aurora.Collections;

namespace Aurora.Pooling
{
    /// <summary>
    /// 管理池中的双端队列的策略。
    /// </summary>
    /// <typeparam name="T">双端队列的成员的类型。</typeparam>
    public class PooledDequePolicy<T> : IPooledObjectPolicy<Deque<T>>
    {
        /// <summary>
        /// 获取或设置池化的双端队列的初始容量。
        /// </summary>
        public int InitialCapacity { get; set; } = 16;

        /// <summary>
        /// 获取或设置允许被放入池的双端队列的最大长度。
        /// </summary>
        public int MaximumRetainedCount { get; set; } = 256;

        /// <inheritdoc />
        public Deque<T> Create()
        {
            return new Deque<T>(InitialCapacity);
        }

        /// <inheritdoc />
        public void Get(Deque<T> obj)
        {
        }

        /// <inheritdoc />
        public bool Return(Deque<T> obj)
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
        public void Dispose(Deque<T> obj)
        {
            obj?.Clear();
        }
    }
}
