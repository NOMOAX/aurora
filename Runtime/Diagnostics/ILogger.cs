namespace Aurora.Diagnostics
{
    /// <summary>
    /// A program logger.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs the string representation of the specified object at the specified log level.
        /// </summary>
        /// <param name="value">The object to log.</param>
        /// <param name="logLevel">The log level.</param>
        void Log(object value, LogLevel logLevel);
    }
}
