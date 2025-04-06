namespace Aurora.CompilerServices
{
    /// <summary>
    /// 提供上下文，用于在切换到目标环境时等待。
    /// </summary>
    public interface IAwaitable
    {
        /// <summary>
        /// 获取等待器。
        /// </summary>
        /// <returns>等待器。</returns>
        /// <remarks>
        /// 根据 C# 规范“12.9.8.2 可等待表达式”的规定，请隐式实现此接口成员。
        /// </remarks>
        /// <seealso href="https://learn.microsoft.com/dotnet/csharp/language-reference/language-specification/expressions#12982-awaitable-expressions"/>
        IAwaiter GetAwaiter();
    }
}
