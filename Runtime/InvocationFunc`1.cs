using System;

namespace Aurora
{
    /// <summary>
    /// An invocation that takes no parameters and returns a value.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value.</typeparam>
    public sealed class InvocationFunc<TResult> : Invocation<TResult>
    {
        private readonly Func<TResult> _func;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationFunc{TResult}"/> class.
        /// </summary>
        /// <param name="func">A method that takes no parameters and returns a value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
        public InvocationFunc(Func<TResult> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
        }

        /// <inheritdoc />
        public override TResult Invoke()
        {
            return _func();
        }
    }
}
