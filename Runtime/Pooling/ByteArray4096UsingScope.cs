using System;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// 使用长度为 4096 的字节数组范围。
    /// </summary>
    public sealed class ByteArray4096UsingScope : IDisposable
    {
        private byte[] _array;

        /// <summary>
        /// 初始化 <see cref="ByteArray4096UsingScope"/> 类的新实例。
        /// </summary>
        /// <param name="array">此输出参数将被赋值为一个长度为 4096 且所有元素都为 0 的字节数组。</param>
        public ByteArray4096UsingScope(out byte[] array)
        {
            _array = PredefinedPools.ByteArray4096.Get();
            array  = _array;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var array = _array;
            if (array != null && Interlocked.CompareExchange(ref _array, null, array) == array)
            {
                PredefinedPools.ByteArray4096.Return(array);
            }
        }
    }
}
