using System;
using System.Runtime.CompilerServices;

namespace Aurora
{
    /// <summary>
    /// Provides English-language utility methods.
    /// </summary>
    public static class EnglishUtility
    {
        /// <summary>
        /// Returns the singular form if <paramref name="count"/> is 1 or -1; otherwise, returns the plural form.
        /// </summary>
        /// <param name="singular">The singular form.</param>
        /// <param name="plural">The plural form.</param>
        /// <param name="count">The count that determines which form is returned.</param>
        /// <returns>The <paramref name="singular"/> form if <paramref name="count"/> is 1 or -1; otherwise, the <paramref name="plural"/> form.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Pluralize(string singular, string plural, int count)
        {
            return count switch
            {
                1 or -1 => singular,
                _       => plural
            };
        }

        /// <summary>
        /// Returns the ordinal suffix for the specified number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>The ordinal suffix: "st", "nd", "rd", or "th".</returns>
        /// <example>
        /// <code>
        /// $"{number}{EnglishUtility.Th(number)}" // 1st, 2nd, 3rd, 11th, 21st
        /// </code>
        /// </example>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Th(int number)
        {
            return Math.Abs(number % 100) switch
            {
                11 or 12 or 13 => "th",
                var lastTwoDigits => (lastTwoDigits % 10) switch
                {
                    1 => "st",
                    2 => "nd",
                    3 => "rd",
                    _ => "th"
                }
            };
        }
    }
}
