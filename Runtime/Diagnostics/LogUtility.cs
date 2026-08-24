using System;
using System.Text;

namespace Aurora.Diagnostics
{
    /// <summary>
    /// Provides a set of utility methods related to logging.
    /// </summary>
    public static class LogUtility
    {
        /// <summary>
        /// Appends information about the identifier of the current thread to the end of the mutable string.
        /// </summary>
        /// <param name="stringBuilder">The mutable string.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stringBuilder"/> is <see langword="null"/>.</exception>
        /// <remarks>This method is designed to be called only from within implementations of <see cref="ILogger"/>.</remarks>
        public static void AppendCurrentThreadIdString(StringBuilder stringBuilder)
        {
            if (stringBuilder == null)
            {
                throw new ArgumentNullException(nameof(stringBuilder));
            }
            if (!Log.WithCurrentThreadId)
            {
                return;
            }
            stringBuilder.Append(',');
            stringBuilder.Append(' ');
            stringBuilder.Append('@');
            stringBuilder.Append(System.Environment.CurrentManagedThreadId);
        }

        /// <summary>
        /// Appends information about the current date and time of the current machine to the end of the mutable string.
        /// </summary>
        /// <param name="stringBuilder">The mutable string.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stringBuilder"/> is <see langword="null"/>.</exception>
        /// <remarks>This method is designed to be called only from within implementations of <see cref="ILogger"/>.</remarks>
        public static void AppendDateTimeOffsetString(StringBuilder stringBuilder)
        {
            if (stringBuilder == null)
            {
                throw new ArgumentNullException(nameof(stringBuilder));
            }
            switch (Log.DateTimeOffsetFormat)
            {
                case LogDateTimeOffsetFormat.None:
                    break;
                case LogDateTimeOffsetFormat.S:
                    stringBuilder.Append(',');
                    stringBuilder.Append(' ');
                    stringBuilder.Append(DateTimeOffset.Now.ToString("s"));
                    break;
                case LogDateTimeOffsetFormat.O:
                    stringBuilder.Append(',');
                    stringBuilder.Append(' ');
                    stringBuilder.Append(DateTimeOffset.Now.ToString("O"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
