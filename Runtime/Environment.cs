namespace Aurora
{
    /// <summary>
    /// 运行环境。
    /// </summary>
    public static class Environment
    {
        /// <summary>
        /// 指示当前环境是否为单线程环境。
        /// </summary>
        /// <remarks>这个值由用户自行设置（默认值为 <see langword="false"/>）。</remarks>
        public static bool IsSingleThreadEnvironment { get; set; } = false;
    }
}
