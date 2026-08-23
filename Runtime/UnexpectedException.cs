using System;

namespace Aurora
{
    /// <summary>
    /// The exception thrown when an unexpected situation is encountered.
    /// </summary>
    public class UnexpectedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedException"/> class.
        /// </summary>
        public UnexpectedException() : base("An unexpected situation was encountered")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedException"/> class with the specified error message.
        /// </summary>
        /// <inheritdoc />
        public UnexpectedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedException"/> class with the specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <inheritdoc />
        public UnexpectedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
