namespace Aurora.Diagnostics
{
    /// <summary>
    /// 记录日期和时间时使用的格式。
    /// </summary>
    public enum LogDateTimeOffsetFormat
    {
        /// <summary>
        /// 不记录日期时间。
        /// </summary>
        None,

        /// <summary>
        /// 使用标准日期和时间格式字符串“s”。
        /// </summary>
        S,

        /// <summary>
        /// 使用标准日期和时间格式字符串“O”。
        /// </summary>
        O
    }
}
