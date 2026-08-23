using System;
using System.Diagnostics;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// A using scope for a stopwatch.
    /// </summary>
    public sealed class StopwatchUsingScope : IDisposable
    {
        private Stopwatch _stopwatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="StopwatchUsingScope"/> class.
        /// </summary>
        /// <param name="stopwatch">This output parameter is assigned an empty stopwatch.</param>
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
