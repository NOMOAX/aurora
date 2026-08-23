using System;
using System.IO;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// A using scope for a memory stream.
    /// </summary>
    public sealed class MemoryStreamUsingScope : IDisposable
    {
        private MemoryStream _memoryStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryStreamUsingScope"/> class.
        /// </summary>
        /// <param name="memoryStream">This output parameter is assigned an empty memory stream.</param>
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
