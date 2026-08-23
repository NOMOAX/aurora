namespace Aurora.CompilerServices
{
    /// <summary>
    /// Provides a context to await while switching to the target environment.
    /// </summary>
    /// <typeparam name="TResult">The type of the result available when the await completes.</typeparam>
    public interface IAwaitable<out TResult>
    {
        /// <summary>
        /// Gets the awaiter.
        /// </summary>
        /// <returns>The awaiter.</returns>
        /// <remarks>
        /// According to the C# specification on "awaitable expressions", please implement this interface member explicitly.
        /// </remarks>
        /// <seealso href="https://learn.microsoft.com/dotnet/csharp/language-reference/language-specification/expressions"/>
        IAwaiter<TResult> GetAwaiter();
    }
}
