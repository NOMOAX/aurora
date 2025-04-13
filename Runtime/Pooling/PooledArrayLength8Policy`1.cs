using System;

namespace Aurora.Pooling
{
    /// <summary>
    /// 管理池中的长度为 8 的数组的策略。
    /// </summary>
    /// <typeparam name="T">数组的元素的类型。</typeparam>
    public class PooledArrayLength8Policy<T> : IPooledObjectPolicy<T[]>
    {
        private const int Length = 8;

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
            if (obj == null)
            {
                return false;
            }
            if (obj.Length != Length)
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
