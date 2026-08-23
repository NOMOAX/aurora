using System.Runtime.CompilerServices;

namespace Aurora
{
    /// <summary>
    /// An invocation that returns a value.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value.</typeparam>
    public abstract class Invocation<TResult>
    {
        /// <summary>
        /// Invokes.
        /// </summary>
        /// <returns>The returned value.</returns>
        public abstract TResult Invoke();

        /// <summary>
        /// Creates an <see cref="Invocation{TResult}"/> instance whose <see cref="Invoke"/> method directly returns the specified value.
        /// </summary>
        /// <param name="result">The value returned when the <see cref="Invoke"/> method of the <see cref="Invocation{TResult}"/> instance is executed.</param>
        /// <returns>An <see cref="Invocation{TResult}"/> instance whose <see cref="Invoke"/> method directly returns the specified value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Invocation<TResult> FromResult(TResult result)
        {
            return new InvocationResult<TResult>(result);
        }
    }
}
