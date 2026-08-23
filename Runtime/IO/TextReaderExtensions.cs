using System;
using System.IO;

namespace Aurora.IO
{
    /// <summary>
    /// Provides extension methods for the <see cref="TextReader"/> class.
    /// </summary>
    public static class TextReaderExtensions
    {
        /// <summary>
        /// Skips consecutive white-space characters in the current <see cref="TextReader"/>.
        /// </summary>
        /// <param name="textReader">The character reader.</param>
        /// <exception cref="ArgumentNullException"><paramref name="textReader"/> is <see langword="null"/>.</exception>
        public static void SkipWhiteSpaces(this TextReader textReader)
        {
            if (textReader == null)
            {
                throw new ArgumentNullException(nameof(textReader));
            }
            int nextChar;
            while ((nextChar = textReader.Peek()) != -1 && char.IsWhiteSpace((char)nextChar))
            {
                textReader.Read();
            }
        }
    }
}
