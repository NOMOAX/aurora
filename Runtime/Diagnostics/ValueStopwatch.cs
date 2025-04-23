using System;
using System.Diagnostics;

namespace Aurora.Diagnostics
{
    /// <summary>
    /// 秒表（值类型）。
    /// </summary>
    public struct ValueStopwatch
    {
        private static readonly double TickFrequency = (double) TimeSpan.TicksPerSecond / Stopwatch.Frequency;

        private long _elapsed;

        private long _startTimeStamp;

        private bool _isRunning;

        /// <summary>
        /// 获取一个值，该值指示 <see cref="ValueStopwatch"/> 是否正在运行。
        /// </summary>
        public readonly bool IsRunning => _isRunning;

        /// <summary>
        /// 获取 <see cref="ValueStopwatch"/> 测量得出的总运行时间。
        /// </summary>
        public readonly TimeSpan Elapsed => new TimeSpan(GetElapsedDateTimeTicks());

        /// <summary>
        /// 获取 <see cref="ValueStopwatch"/> 测量得出的总运行时间（以毫秒为单位）。
        /// </summary>
        public readonly long ElapsedMilliseconds => GetElapsedDateTimeTicks() / TimeSpan.TicksPerMillisecond;

        /// <summary>
        /// 获取 <see cref="ValueStopwatch"/> 测量得出的总运行时间（用计时器刻度表示）。
        /// </summary>
        public readonly long ElapsedTicks => GetRawElapsedTicks();

        /// <summary>
        /// 开始或继续测量某个时间间隔的运行时间。
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
        /// 初始化新的 <see cref="ValueStopwatch"/> 实例，将运行时间属性设置为零，然后开始测量运行时间。
        /// </summary>
        /// <returns>刚刚开始测量运行时间的 <see cref="ValueStopwatch"/>。</returns>
        public static ValueStopwatch StartNew()
        {
            var valueStopwatch = new ValueStopwatch();
            valueStopwatch.Start();
            return valueStopwatch;
        }

        /// <summary>
        /// 停止测量某个时间间隔的运行时间。
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
        /// 停止时间间隔测量，并将运行时间重置为零。
        /// </summary>
        public void Reset()
        {
            _elapsed        = 0L;
            _startTimeStamp = 0L;
            _isRunning      = false;
        }

        /// <summary>
        /// 停止时间间隔测量，将运行时间重置为零，然后开始测量运行时间。
        /// </summary>
        public void Restart()
        {
            _elapsed        = 0L;
            _startTimeStamp = Stopwatch.GetTimestamp();
            _isRunning      = true;
        }

        private readonly long GetElapsedDateTimeTicks()
        {
            return (long) (GetRawElapsedTicks() * TickFrequency);
        }

        private readonly long GetRawElapsedTicks()
        {
            return _isRunning ? _elapsed + (Stopwatch.GetTimestamp() - _startTimeStamp) : _elapsed;
        }
    }
}
