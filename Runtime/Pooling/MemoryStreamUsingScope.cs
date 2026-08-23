using System;
using System.IO;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// 使用内存流范围。
    /// </summary>
    public sealed class MemoryStreamUsingScope : IDisposable
    {
        private MemoryStream _memoryStream;

        /// <summary>
        /// 初始化 <see cref="MemoryStreamUsingScope"/> 类的新实例。
        /// </summary>
        /// <param name="memoryStream">此输出参数将被赋值为一个空内存流。</param>
        public MemoryStreamUsingScope(out MemoryStream memoryStream)
        {
            _memoryStream = PredefinedPools.MemoryStream.Get();
            memoryStream  = _memoryStream;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var memoryStream = _memoryStream;
            if (memoryStream != null &&
                Interlocked.CompareExchange(ref _memoryStream, null, memoryStream) == memoryStream)
            {
                PredefinedPools.MemoryStream.Return(memoryStream);
            }
        }
    }
}
