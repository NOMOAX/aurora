using System;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// A using scope for a byte array of length 4096.
    /// </summary>
    public sealed class ByteArray4096UsingScope : IDisposable
    {
        private byte[] _array;

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteArray4096UsingScope"/> class.
        /// </summary>
        /// <param name="array">This output parameter is assigned a byte array of length 4096 whose elements are all zero.</param>
        public ByteArray4096UsingScope(out byte[] array)
        {
            _array = PredefinedPools.ByteArrayLength4096.Get();
            array  = _array;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var array = _array;
            if (array != null && Interlocked.CompareExchange(ref _array, null, array) == array)
            {
                PredefinedPools.ByteArrayLength4096.Return(array);
            }
        }
    }
}
