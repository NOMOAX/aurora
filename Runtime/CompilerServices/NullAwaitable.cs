using System;
using Aurora.Diagnostics;

namespace Aurora.CompilerServices
{
    /// <summary>
    /// Implements <see cref="IAwaitable"/> using the null-object pattern. This awaitable context always completes immediately and has no effect on the asynchronous execution flow.
    /// </summary>
    /// <remarks>Used to eliminate the compiler warning produced when the <see langword="await"/> keyword is not used in an <see langword="async"/> method.</remarks>
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
                Log.I(
                    $"Make sure to use the \"await new {nameof(NullAwaitable)}();\" expression only during development to eliminate the compiler warning"
                );
            }
        }
    }
}
