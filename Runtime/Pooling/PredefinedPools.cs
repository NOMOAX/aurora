using System.Diagnostics;
using System.IO;
using System.Text;

namespace Aurora.Pooling
{
    /// <summary>
    /// 提供一组预定义的公共对象池。
    /// </summary>
    public static class PredefinedPools
    {
        /// <summary>
        /// 长度为 4096 的字节数组池。
        /// </summary>
        public static readonly IPool<byte[]> ByteArrayLength4096 =
            new Pool<byte[]>(new PooledByteArrayLength4096Policy());

        /// <summary>
        /// 内存流池。
        /// </summary>
        public static readonly IPool<MemoryStream> MemoryStream =
            new Pool<MemoryStream>(new PooledMemoryStreamPolicy());

        /// <summary>
        /// 秒表池。
        /// </summary>
        public static readonly IPool<Stopwatch> Stopwatch = new Pool<Stopwatch>(new PooledStopwatchPolicy());

        /// <summary>
        /// 可变字符串池。
        /// </summary>
        public static readonly IPool<StringBuilder> StringBuilder =
            new Pool<StringBuilder>(new PooledStringBuilderPolicy());
    }
}
