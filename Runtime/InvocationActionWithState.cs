using System;

namespace Aurora
{
    /// <summary>
    /// An invocation that takes one parameter of type <see cref="object"/> and returns no value.
    /// </summary>
    public sealed class InvocationActionWithState : Invocation
    {
        private readonly Action<object> _actionWithState;

        private readonly object _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationActionWithState"/> class.
        /// </summary>
        /// <param name="actionWithState">A method that takes one parameter of type <see cref="object"/> and returns no value.</param>
        /// <param name="state">The parameter used by <paramref name="actionWithState"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="actionWithState"/> is <see langword="null"/>.</exception>
        public InvocationActionWithState(Action<object> actionWithState, object state)
        {
            _actionWithState = actionWithState ?? throw new ArgumentNullException(nameof(actionWithState));
            _state           = state;
        }

        /// <inheritdoc />
        public override void Invoke()
        {
            _actionWithState(_state);
        }
    }
}
