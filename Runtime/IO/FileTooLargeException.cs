using System.IO;

namespace Aurora.IO
{
    /// <summary>
    /// The exception thrown when a file is too large.
    /// </summary>
    public sealed class FileTooLargeException : IOException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileTooLargeException"/> class.
        /// </summary>
        public FileTooLargeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTooLargeException"/> class with the specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public FileTooLargeException(string message) : base(message)
        {
        }
    }
}
