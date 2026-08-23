namespace Aurora.Diagnostics
{
    /// <summary>
    /// 记录程序。
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 按照指定的记录等级，记录指定对象的字符串表现形式。
        /// </summary>
        /// <param name="value">要记录的对象。</param>
        /// <param name="logLevel">记录等级。</param>
        void Log(object value, LogLevel logLevel);
    }
}
