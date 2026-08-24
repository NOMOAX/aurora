using System;

namespace Aurora
{
    /// <summary>
    /// Provides utilities for parsing hexadecimal characters.
    /// </summary>
    public static class HexCharParseUtility
    {
        private const int LookupLength = 'f' + 1;

        private const byte InvalidValue = byte.MaxValue;

        private static readonly byte[] Lookup;

        static HexCharParseUtility()
        {
            Lookup = new byte[LookupLength];
            Array.Fill(Lookup, InvalidValue);
            for (var i = '0'; i <= '9'; i++)
            {
                Lookup[i] = (byte)(i - '0');
            }
            for (var i = 'A'; i <= 'F'; i++)
            {
                Lookup[i] = (byte)(10 + (i - 'A'));
            }
            for (var i = 'a'; i <= 'f'; i++)
            {
                Lookup[i] = (byte)(10 + (i - 'a'));
            }
        }

        /// <summary>
        /// Parses a hexadecimal character.
        /// </summary>
        /// <param name="c">The hexadecimal character.</param>
        /// <returns>The numeric value (0 to 15) corresponding to the hexadecimal character.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="c"/> is not a valid hexadecimal character (0-9, A-F, a-f).</exception>
        public static byte Parse(char c)
        {
            if (c >= LookupLength)
            {
                throw new ArgumentOutOfRangeException(nameof(c), c, null);
            }
            var value = Lookup[c];
            return value != InvalidValue ? value : throw new ArgumentOutOfRangeException(nameof(c), c, null);
        }

        /// <summary>
        /// Parses a hexadecimal character.
        /// </summary>
        /// <param name="c">The hexadecimal character.</param>
        /// <returns>The numeric value (0 to 15) corresponding to the hexadecimal character. If <paramref name="c"/> is not a valid hexadecimal character (0-9, A-F, a-f), it is <see cref="byte.MaxValue"/>.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="c"/> is greater than `f`.</exception>
        /// <remarks>This method does not perform any checks. You may use it if you are certain that <paramref name="c"/> is a valid hexadecimal character (0-9, A-F, a-f).</remarks>
        public static byte ParseNoCheck(char c)
        {
            return Lookup[c];
        }

        /// <summary>
        /// Tries to parse a hexadecimal character.
        /// </summary>
        /// <param name="c">The hexadecimal character.</param>
        /// <param name="value">The numeric value (0 to 15) corresponding to the hexadecimal character. If <paramref name="c"/> is not a valid hexadecimal character (0-9, A-F, a-f), it is <see cref="byte.MaxValue"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="c"/> is a valid hexadecimal character (0-9, A-F, a-f); otherwise, returns <see langword="false"/>.</returns>
        public static bool TryParse(char c, out byte value)
        {
            if (c >= LookupLength)
            {
                value = InvalidValue;
                return false;
            }
            value = Lookup[c];
            return value != InvalidValue;
        }
    }
}
