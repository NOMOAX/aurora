using System;

namespace Aurora.Pooling
{
    /// <summary>
    /// A strategy for managing byte arrays of length 4096 in the pool.
    /// </summary>
    public class PooledByteArrayLength4096Policy : IPooledObjectPolicy<byte[]>
    {
        private const int Length = 4096;

        /// <inheritdoc />
        public byte[] Create()
        {
            return new byte[Length];
        }

        /// <inheritdoc />
        public void Get(byte[] obj)
        {
        }

        /// <inheritdoc />
        public bool Return(byte[] obj)
        {
            if (obj is not { Length: Length })
            {
                return false;
            }
            Array.Clear(obj, 0, Length);
            return true;
        }

        /// <inheritdoc />
        public void Dispose(byte[] obj)
        {
            if (obj != null)
            {
                Array.Clear(obj, 0, obj.Length);
            }
        }
    }
}
