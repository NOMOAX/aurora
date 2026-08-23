using System;
using Aurora.Diagnostics;

namespace Aurora.CompilerServices
{
    /// <summary>
    /// 使用空对象模式实现 <see cref="IAwaitable"/>。该可等待上下文总是立即完成，不会对异步执行流程造成任何影响。
    /// </summary>
    /// <remarks>用于消除在 <see langword="async"/> 方法中未使用 <see langword="await"/> 关键字时产生的编译器警告。</remarks>
    /// <example>
    /// <code>
    /// public async Task NotImplementedAsyncMethod(CancellationToken cancellationToken)
    /// {
    ///     // disable compiler warning CS1998: Async function without await expression
    ///     await new NullAwaitable();
    /// }
    /// </code>
    /// </example>
    public readonly struct NullAwaitable : IAwaitable
    {
        /// <inheritdoc />
        public IAwaiter GetAwaiter()
        {
            return new Awaiter();
        }

        private readonly struct Awaiter : IAwaiter
        {
            public bool IsCompleted => true;

            public void OnCompleted(Action continuation)
            {
                continuation();
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                continuation();
            }

            public void GetResult()
            {
                Log.I($"请确保仅在开发阶段中使用 \"await new {nameof(NullAwaitable)}();\" 表达式消除编译器警告");
            }
        }
    }
}
