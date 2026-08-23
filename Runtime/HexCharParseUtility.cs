using System;

namespace Aurora
{
    /// <summary>
    /// 提供解析 16 进制字符的工具。
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
        /// 解析 16 进制字符。
        /// </summary>
        /// <param name="c">16 进制字符。</param>
        /// <returns>16 进制字符对应的数值（0到15）。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="c"/> 不是有效的 16 进制字符（0到9、A到F、a到f）。</exception>
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
        /// 解析 16 进制字符。
        /// </summary>
        /// <param name="c">16 进制字符。</param>
        /// <returns>16 进制字符对应的数值（0到15）。如果 <paramref name="c"/> 不是有效的 16 进制字符（0到9、A到F、a到f），则为 <see cref="byte.MaxValue"/>。</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="c"/> 大于 `f`。</exception>
        /// <remarks>此方法不会进行检查。如果你确信 <paramref name="c"/> 是有效的 16 进制字符（0到9、A到F、a到f），则可以使用此方法。</remarks>
        public static byte ParseNoCheck(char c)
        {
            return Lookup[c];
        }

        /// <summary>
        /// 尝试解析 16 进制字符。
        /// </summary>
        /// <param name="c">16 进制字符。</param>
        /// <param name="value">16 进制字符对应的数值（0到15）。如果 <paramref name="c"/> 不是有效的 16 进制字符（0到9、A到F、a到f），则为 <see cref="byte.MaxValue"/>。</param>
        /// <returns>如果 <paramref name="c"/> 是有效的 16 进制字符（0到9、A到F、a到f），则返回 <see langword="true"/>，否则返回 <see langword="false"/>。</returns>
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
