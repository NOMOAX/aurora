using System;

namespace Aurora.Pooling
{
    /// <summary>
    /// 管理池中的长度为 2 的数组的策略。
    /// </summary>
    /// <typeparam name="T">数组的元素的类型。</typeparam>
    public class PooledArrayLength2Policy<T> : IPooledObjectPolicy<T[]>
    {
        private const int Length = 2;

        /// <inheritdoc />
        public T[] Create()
        {
            return new T[Length];
        }

        /// <inheritdoc />
        public void Get(T[] obj)
        {
        }

        /// <inheritdoc />
        public bool Return(T[] obj)
        {
            if (obj is not { Length: Length })
            {
                return false;
            }
            Array.Clear(obj, 0, Length);
            return true;
        }

        /// <inheritdoc />
        public void Dispose(T[] obj)
        {
            if (obj != null)
            {
                Array.Clear(obj, 0, obj.Length);
            }
        }
    }
}
