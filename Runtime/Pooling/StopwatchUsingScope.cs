using System;
using System.Diagnostics;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// 使用秒表范围。
    /// </summary>
    public sealed class StopwatchUsingScope : IDisposable
    {
        private Stopwatch _stopwatch;

        /// <summary>
        /// 初始化 <see cref="StopwatchUsingScope"/> 类的新实例。
        /// </summary>
        /// <param name="stopwatch">此输出参数将被赋值为一个空秒表。</param>
        public StopwatchUsingScope(out Stopwatch stopwatch)
        {
            _stopwatch = PredefinedPools.Stopwatch.Get();
            stopwatch  = _stopwatch;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var stopwatch = _stopwatch;
            if (stopwatch != null && Interlocked.CompareExchange(ref _stopwatch, null, stopwatch) == stopwatch)
            {
                PredefinedPools.Stopwatch.Return(stopwatch);
            }
        }
    }
}
