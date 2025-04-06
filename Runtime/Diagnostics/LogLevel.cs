namespace Aurora.Diagnostics
{
    /// <summary>
    /// 记录等级。
    /// </summary>
    public enum LogLevel : byte
    {
        /// <summary>
        /// 详尽（verbose）。
        /// </summary>
        V,

        /// <summary>
        /// 调试（debug）。
        /// </summary>
        D,

        /// <summary>
        /// 常规信息（info）。
        /// </summary>
        I,

        /// <summary>
        /// 警告（warning）。
        /// </summary>
        W,

        /// <summary>
        /// 错误（error）。
        /// </summary>
        E
    }
}
