using System;

namespace Aurora.Pooling
{
    /// <summary>
    /// A strategy for managing arrays of length 8 in the pool.
    /// </summary>
    /// <typeparam name="T">The type of the array's elements.</typeparam>
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
