using System;

namespace Aurora.Pooling
{
    /// <summary>
    /// 管理池中的长度为 4096 的字节数组的策略。
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
        public void Dispose(byte[] obj)
        {
            if (obj != null)
            {
                Array.Clear(obj, 0, obj.Length);
            }
        }
    }
}
