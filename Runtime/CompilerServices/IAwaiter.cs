using System.Runtime.CompilerServices;

namespace Aurora.CompilerServices
{
    /// <summary>
    /// 用于切换到目标环境的等待器。
    /// </summary>
    public interface IAwaiter : ICriticalNotifyCompletion
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
        /// 处理结果。
        /// </summary>
        /// <remarks>
        /// 根据 C# 规范“12.9.8.2 可等待表达式”的规定，请隐式实现此接口成员。
        /// </remarks>
        /// <seealso href="https://learn.microsoft.com/dotnet/csharp/language-reference/language-specification/expressions#12982-awaitable-expressions"/>
        void GetResult();
    }
}
