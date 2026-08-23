using System;

namespace Aurora.Pooling
{
    /// <summary>
    /// A strategy for managing arrays of length 2 in the pool.
    /// </summary>
    /// <typeparam name="T">The type of the array's elements.</typeparam>
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
