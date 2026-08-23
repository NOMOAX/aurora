using System.Diagnostics;
using System.IO;
using System.Text;

namespace Aurora.Pooling
{
    /// <summary>
    /// Provides a set of predefined public object pools.
    /// </summary>
    public static class PredefinedPools
    {
        /// <summary>
        /// A pool of byte arrays of length 4096.
        /// </summary>
        public static readonly IPool<byte[]> ByteArrayLength4096 =
            new Pool<byte[]>(new PooledByteArrayLength4096Policy());

        /// <summary>
        /// A pool of memory streams.
        /// </summary>
        public static readonly IPool<MemoryStream> MemoryStream =
            new Pool<MemoryStream>(new PooledMemoryStreamPolicy());

        /// <summary>
        /// A pool of stopwatches.
        /// </summary>
        public static readonly IPool<Stopwatch> Stopwatch = new Pool<Stopwatch>(new PooledStopwatchPolicy());

        /// <summary>
        /// A pool of mutable strings.
        /// </summary>
        public static readonly IPool<StringBuilder> StringBuilder =
            new Pool<StringBuilder>(new PooledStringBuilderPolicy());
    }
}
