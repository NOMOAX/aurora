using System;
using System.IO;

namespace Aurora.IO
{
    /// <summary>
    /// 为 <see cref="TextReader"/> 类提供扩展方法。
    /// </summary>
    public static class TextReaderExtensions
    {
        /// <summary>
        /// 跳过当前 <see cref="TextReader"/> 中连续的空白字符。
        /// </summary>
        /// <param name="textReader">字符读取器。</param>
        /// <exception cref="ArgumentNullException"><paramref name="textReader"/> 为 <see langword="null"/>。</exception>
        public static void SkipWhiteSpaces(this TextReader textReader)
        {
            if (textReader == null)
            {
                throw new ArgumentNullException(nameof(textReader));
            }
            int nextChar;
            while ((nextChar = textReader.Peek()) != -1 && char.IsWhiteSpace((char) nextChar))
            {
                textReader.Read();
            }
        }
    }
}
