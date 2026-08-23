using System.Runtime.CompilerServices;

namespace Aurora.CompilerServices
{
    /// <summary>
    /// An awaiter used to switch to the target environment.
    /// </summary>
    /// <typeparam name="TResult">The type of the awaited result.</typeparam>
    public interface IAwaiter<out TResult> : ICriticalNotifyCompletion
    {
        /// <summary>
        /// Gets a value that indicates whether the wait operation has completed.
        /// </summary>
        /// <remarks>
        /// According to the C# specification on "awaitable expressions", please implement this interface member explicitly.
        /// </remarks>
        /// <seealso href="https://learn.microsoft.com/dotnet/csharp/language-reference/language-specification/expressions"/>
        bool IsCompleted { get; }

        /// <summary>
        /// Processes and gets the result.
        /// </summary>
        /// <returns>The awaited result.</returns>
        /// <remarks>
        /// According to the C# specification on "awaitable expressions", please implement this interface member explicitly.
        /// </remarks>
        /// <seealso href="https://learn.microsoft.com/dotnet/csharp/language-reference/language-specification/expressions"/>
        TResult GetResult();
    }
}
