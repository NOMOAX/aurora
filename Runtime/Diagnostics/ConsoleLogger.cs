using System;
using Aurora.Pooling;

namespace Aurora.Diagnostics
{
    /// <summary>
    /// Logs to the console standard output stream.
    /// </summary>
    public sealed class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static ConsoleLogger Instance { get; } = new();

        private ConsoleLogger()
        {
        }

        void ILogger.Log(object value, LogLevel logLevel)
        {
            if (!EnumUtility<LogLevel>.IsDefined(logLevel))
            {
                return;
            }
            if (logLevel < Log.Level)
            {
                return;
            }

            var stringBuilder = PredefinedPools.StringBuilder.Get();
            try
            {
                stringBuilder.Append(nameof(ConsoleLogger));
                stringBuilder.Append(' ');
                stringBuilder.Append('(');
                stringBuilder.Append(logLevel);
                LogUtility.AppendCurrentThreadIdString(stringBuilder);
                LogUtility.AppendDateTimeOffsetString(stringBuilder);
                stringBuilder.Append(')');
                stringBuilder.Append(' ');
                stringBuilder.Append(value);
                Console.WriteLine(stringBuilder.ToString());
            }
            finally
            {
                PredefinedPools.StringBuilder.Return(stringBuilder);
            }
        }
    }
}
