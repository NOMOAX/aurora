namespace Aurora.Diagnostics
{
    /// <summary>
    /// The format used when logging the date and time.
    /// </summary>
    public enum LogDateTimeOffsetFormat
    {
        /// <summary>
        /// Do not log the date and time.
        /// </summary>
        None,

        /// <summary>
        /// Uses the standard date-and-time format string "s".
        /// </summary>
        S,

        /// <summary>
        /// Uses the standard date-and-time format string "O".
        /// </summary>
        O
    }
}
