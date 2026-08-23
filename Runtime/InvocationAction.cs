using System;

namespace Aurora
{
    /// <summary>
    /// An invocation that takes no parameters and returns no value.
    /// </summary>
    public sealed class InvocationAction : Invocation
    {
        private readonly Action _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationAction"/> class.
        /// </summary>
        /// <param name="action">A method that takes no parameters and returns no value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
        public InvocationAction(Action action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        /// <inheritdoc />
        public override void Invoke()
        {
            _action();
        }
    }
}
