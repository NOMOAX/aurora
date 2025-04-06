using System.Runtime.CompilerServices;

namespace Aurora.CompilerServices
{
    /// <summary>
    /// 用于切换到目标环境的等待器。
    /// </summary>
    /// <typeparam name="TResult">等待的结果的类型。</typeparam>
    public interface IAwaiter<out TResult> : ICriticalNotifyCompletion
    {
        /// <summary>
        /// 获取一个值，这个值指示等待操作是否已完成。
        /// </summary>
        /// <remarks>
        /// 根据 C# 规范“12.9.8.2 可等待表达式”的规定，请隐式实现此接口成员。
        /// </remarks>
        /// <seealso href="https://learn.microsoft.com/dotnet/csharp/language-reference/language-specification/expressions#12982-awaitable-expressions"/>
        bool IsCompleted { get; }

        /// <summary>
        /// 处理并获取结果。
        /// </summary>
        /// <returns>等待的结果。</returns>
        /// <remarks>
        /// 根据 C# 规范“12.9.8.2 可等待表达式”的规定，请隐式实现此接口成员。
        /// </remarks>
        /// <seealso href="https://learn.microsoft.com/dotnet/csharp/language-reference/language-specification/expressions#12982-awaitable-expressions"/>
        TResult GetResult();
    }
}
