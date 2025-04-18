using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Aurora.Diagnostics
{
    /// <summary>
    /// 记录。
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// 获取或设置记录程序。
        /// </summary>
        public static ILogger Logger { get; set; } = ConsoleLogger.Instance;

        private static LogLevel _level;

        /// <summary>
        /// 获取或设置记录等级。
        /// </summary>
        /// <remarks>小于此值的记录不会执行。</remarks>
        public static LogLevel Level
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _level;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (!EnumUtility<LogLevel>.IsDefined(value))
                {
                    return;
                }
                _level = value;
            }
        }

        /// <summary>
        /// 获取或设置是否记录当前线程 ID。
        /// </summary>
        public static bool WithCurrentThreadId { get; set; } = true;

        private static LogDateTimeOffsetFormat _dateTimeOffsetFormat = LogDateTimeOffsetFormat.O;

        /// <summary>
        /// 获取或设置记录日期和时间的格式。
        /// </summary>
        public static LogDateTimeOffsetFormat DateTimeOffsetFormat
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _dateTimeOffsetFormat;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (!EnumUtility<LogDateTimeOffsetFormat>.IsDefined(value))
                {
                    return;
                }
                _dateTimeOffsetFormat = value;
            }
        }

        /// <summary>
        /// 记录指定对象的字符串表现形式（等级为 <see cref="LogLevel.V"/>）。
        /// </summary>
        /// <param name="value">要记录的对象。</param>
        public static void V(object value)
        {
            Logger?.Log(value, LogLevel.V);
        }

        /// <summary>
        /// 记录指定对象的字符串表现形式（等级为 <see cref="LogLevel.D"/>）。
        /// </summary>
        /// <param name="value">要记录的对象。</param>
        [Conditional("DEBUG")]
        public static void D(object value)
        {
            Logger?.Log(value, LogLevel.D);
        }

        /// <summary>
        /// 记录指定对象的字符串表现形式（等级为 <see cref="LogLevel.I"/>）。
        /// </summary>
        /// <param name="value">要记录的对象。</param>
        public static void I(object value)
        {
            Logger?.Log(value, LogLevel.I);
        }

        /// <summary>
        /// 记录指定对象的字符串表现形式（等级为 <see cref="LogLevel.W"/>）。
        /// </summary>
        /// <param name="value">要记录的对象。</param>
        public static void W(object value)
        {
            Logger?.Log(value, LogLevel.W);
        }

        /// <summary>
        /// 记录指定对象的字符串表现形式（等级为 <see cref="LogLevel.E"/>）。
        /// </summary>
        /// <param name="value">要记录的对象。</param>
        public static void E(object value)
        {
            Logger?.Log(value, LogLevel.E);
        }
    }
}
