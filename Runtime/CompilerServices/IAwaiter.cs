using System.Runtime.CompilerServices;

namespace Aurora.CompilerServices
{
    /// <summary>
    /// An awaiter used to switch to the target environment.
    /// </summary>
    public interface IAwaiter : ICriticalNotifyCompletion
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
        /// Processes the result.
        /// </summary>
        /// <remarks>
        /// According to the C# specification on "awaitable expressions", please implement this interface member explicitly.
        /// </remarks>
        /// <seealso href="https://learn.microsoft.com/dotnet/csharp/language-reference/language-specification/expressions"/>
        void GetResult();
    }
}
