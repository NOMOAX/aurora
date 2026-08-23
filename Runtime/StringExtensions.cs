using System;

namespace Aurora
{
    /// <summary>
    /// 为 <see cref="string"/> 类提供扩展方法。
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 将字符串中的反斜线号（“\”）替换为斜线号（“/”）。
        /// </summary>
        /// <param name="value">字符串。</param>
        /// <returns>一个新的字符串，它是将 <paramref name="value"/> 中的反斜线号（“\”）替换为斜线号（“/”）后的结果。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <see langword="null"/>。</exception>
        public static string ReplaceBackslashWithSlash(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            return value.Replace('\\', '/');
        }
    }
}
