using System;
using System.Text;

namespace Aurora.Diagnostics
{
    /// <summary>
    /// 提供一组与记录有关的工具方法。
    /// </summary>
    public static class LogUtility
    {
        /// <summary>
        /// 向可变字符串末尾添加当前线程 Id 的信息。
        /// </summary>
        /// <param name="stringBuilder">可变字符串。</param>
        /// <exception cref="ArgumentNullException"><paramref name="stringBuilder"/> 为 <see langword="null"/>。</exception>
        /// <remarks>此方法仅被设计用于 <see cref="ILogger"/> 的实现类在其内部调用。</remarks>
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
        /// 向可变字符串末尾添加当前计算机的当前日期和时间的信息。
        /// </summary>
        /// <param name="stringBuilder">可变字符串。</param>
        /// <exception cref="ArgumentNullException"><paramref name="stringBuilder"/> 为 <see langword="null"/>。</exception>
        /// <remarks>此方法仅被设计用于 <see cref="ILogger"/> 的实现类在其内部调用。</remarks>
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
