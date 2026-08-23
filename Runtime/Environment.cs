namespace Aurora
{
    /// <summary>
    /// The runtime environment.
    /// </summary>
    public static class Environment
    {
        /// <summary>
        /// Indicates whether the current environment is a single-threaded environment.
        /// </summary>
        /// <remarks>This value is set by the user (the default is <see langword="false"/>).</remarks>
        public static bool IsSingleThreadEnvironment { get; set; } = false;
    }
}
