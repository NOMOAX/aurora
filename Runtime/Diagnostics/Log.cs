using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Aurora.Diagnostics
{
    /// <summary>
    /// Logging.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public static ILogger Logger { get; set; } = ConsoleLogger.Instance;

        private static LogLevel _level = LogLevel.V;

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        /// <remarks>Logs lower than this value are not executed. The default is <see cref="LogLevel.V"/>.</remarks>
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
        /// Gets or sets whether the identifier of the current thread is logged.
        /// </summary>
        public static bool WithCurrentThreadId { get; set; } = true;

        private static LogDateTimeOffsetFormat _dateTimeOffsetFormat = LogDateTimeOffsetFormat.O;

        /// <summary>
        /// Gets or sets the format used for logging the date and time.
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
        /// Logs the string representation of the specified object (level <see cref="LogLevel.V"/>).
        /// </summary>
        /// <param name="value">The object to log.</param>
        public static void V(object value)
        {
            Logger?.Log(value, LogLevel.V);
        }

        /// <summary>
        /// Logs the string representation of the specified object (level <see cref="LogLevel.D"/>).
        /// </summary>
        /// <param name="value">The object to log.</param>
        [Conditional("DEBUG")]
        public static void D(object value)
        {
            Logger?.Log(value, LogLevel.D);
        }

        /// <summary>
        /// Logs the string representation of the specified object (level <see cref="LogLevel.I"/>).
        /// </summary>
        /// <param name="value">The object to log.</param>
        public static void I(object value)
        {
            Logger?.Log(value, LogLevel.I);
        }

        /// <summary>
        /// Logs the string representation of the specified object (level <see cref="LogLevel.W"/>).
        /// </summary>
        /// <param name="value">The object to log.</param>
        public static void W(object value)
        {
            Logger?.Log(value, LogLevel.W);
        }

        /// <summary>
        /// Logs the string representation of the specified object (level <see cref="LogLevel.E"/>).
        /// </summary>
        /// <param name="value">The object to log.</param>
        public static void E(object value)
        {
            Logger?.Log(value, LogLevel.E);
        }
    }
}
