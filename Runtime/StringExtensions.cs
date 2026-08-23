using System;

namespace Aurora
{
    /// <summary>
    /// Provides extension methods for the <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Replaces backslashes ("\\") in the string with slashes ("/").
        /// </summary>
        /// <param name="value">The string.</param>
        /// <returns>A new string that is the result of replacing backslashes ("\\") in <paramref name="value"/> with slashes ("/").</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
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
