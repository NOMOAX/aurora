using System;

namespace Aurora
{
    /// <summary>
    /// An invocation that takes one parameter of type <see cref="object"/> and returns a value.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value.</typeparam>
    public sealed class InvocationFuncWithState<TResult> : Invocation<TResult>
    {
        private readonly Func<object, TResult> _funcWithState;

        private readonly object _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationFuncWithState{TResult}"/> class.
        /// </summary>
        /// <param name="funcWithState">A method that takes one parameter of type <see cref="object"/> and returns a value.</param>
        /// <param name="state">The parameter used by <paramref name="funcWithState"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="funcWithState"/> is <see langword="null"/>.</exception>
        public InvocationFuncWithState(Func<object, TResult> funcWithState, object state)
        {
            _funcWithState = funcWithState ?? throw new ArgumentNullException(nameof(funcWithState));
            _state         = state;
        }

        /// <inheritdoc />
        public override TResult Invoke()
        {
            return _funcWithState(_state);
        }
    }
}
