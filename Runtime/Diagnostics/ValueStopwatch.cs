using System;
using System.Diagnostics;

namespace Aurora.Diagnostics
{
    /// <summary>
    /// A stopwatch (value type).
    /// </summary>
    public struct ValueStopwatch
    {
        private static readonly double TickFrequency = (double)TimeSpan.TicksPerSecond / Stopwatch.Frequency;

        private long _elapsed;

        private long _startTimeStamp;

        private bool _isRunning;

        /// <summary>
        /// Gets a value indicating whether the <see cref="ValueStopwatch"/> is running.
        /// </summary>
        public readonly bool IsRunning => _isRunning;

        /// <summary>
        /// Gets the total elapsed time measured by the <see cref="ValueStopwatch"/>.
        /// </summary>
        public readonly TimeSpan Elapsed => new(GetElapsedDateTimeTicks());

        /// <summary>
        /// Gets the total elapsed time measured by the <see cref="ValueStopwatch"/> in milliseconds.
        /// </summary>
        public readonly long ElapsedMilliseconds => GetElapsedDateTimeTicks() / TimeSpan.TicksPerMillisecond;

        /// <summary>
        /// Gets the total elapsed time measured by the <see cref="ValueStopwatch"/> in timer ticks.
        /// </summary>
        public readonly long ElapsedTicks => GetRawElapsedTicks();

        /// <summary>
        /// Starts or resumes measuring elapsed time for an interval.
        /// </summary>
        public void Start()
        {
            if (_isRunning)
            {
                return;
            }
            _startTimeStamp = Stopwatch.GetTimestamp();
            _isRunning      = true;
        }

        /// <summary>
        /// Initializes a new <see cref="ValueStopwatch"/> instance, sets the elapsed time property to zero, and then starts measuring elapsed time.
        /// </summary>
        /// <returns>A <see cref="ValueStopwatch"/> that has just started measuring elapsed time.</returns>
        public static ValueStopwatch StartNew()
        {
            var valueStopwatch = new ValueStopwatch();
            valueStopwatch.Start();
            return valueStopwatch;
        }

        /// <summary>
        /// Stops measuring elapsed time for an interval.
        /// </summary>
        public void Stop()
        {
            if (!_isRunning)
            {
                return;
            }
            _elapsed   += Stopwatch.GetTimestamp() - _startTimeStamp;
            _isRunning =  false;
            if (_elapsed >= 0L)
            {
                return;
            }
            _elapsed = 0L;
        }

        /// <summary>
        /// Stops interval measurement and resets the elapsed time to zero.
        /// </summary>
        public void Reset()
        {
            _elapsed        = 0L;
            _startTimeStamp = 0L;
            _isRunning      = false;
        }

        /// <summary>
        /// Stops interval measurement, resets the elapsed time to zero, and then starts measuring elapsed time.
        /// </summary>
        public void Restart()
        {
            _elapsed        = 0L;
            _startTimeStamp = Stopwatch.GetTimestamp();
            _isRunning      = true;
        }

        private readonly long GetElapsedDateTimeTicks()
        {
            return (long)(GetRawElapsedTicks() * TickFrequency);
        }

        private readonly long GetRawElapsedTicks()
        {
            return _isRunning ? _elapsed + (Stopwatch.GetTimestamp() - _startTimeStamp) : _elapsed;
        }
    }
}
