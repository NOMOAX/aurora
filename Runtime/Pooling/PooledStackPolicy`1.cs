using System.Collections.Generic;

namespace Aurora.Pooling
{
    /// <summary>
    /// 管理池中的栈的策略。
    /// </summary>
    /// <typeparam name="T">栈的成员的类型。</typeparam>
    public class PooledStackPolicy<T> : IPooledObjectPolicy<Stack<T>>
    {
        /// <summary>
        /// 获取或设置池化的栈的初始容量。
        /// </summary>
        public int InitialCapacity { get; set; } = 16;

        /// <summary>
        /// 获取或设置允许被放入池的栈的最大长度。
        /// </summary>
        public int MaximumRetainedCount { get; set; } = 256;

        /// <inheritdoc />
        public Stack<T> Create()
        {
            return new Stack<T>(InitialCapacity);
        }

        /// <inheritdoc />
        public void OnGet(Stack<T> obj)
        {
        }

        /// <inheritdoc />
        public bool Return(Stack<T> obj)
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
        public void Dispose(Stack<T> obj)
        {
            obj?.Clear();
        }
    }
}
