using System;
using System.Threading;

namespace Aurora
{
    /// <summary>
    /// Wraps another <see cref="Invocation{TResult}"/> to ensure that only the first call to <see cref="Invoke"/> is actually executed.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value.</typeparam>
    public sealed class OneTimeInvocation<TResult> : Invocation<TResult>
    {
        private Invocation<TResult> _invocation;

        private TResult _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneTimeInvocation{TResult}"/> class.
        /// </summary>
        /// <param name="invocation">An <see cref="Invocation{TResult}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="invocation"/> is <see langword="null"/>.</exception>
        public OneTimeInvocation(Invocation<TResult> invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            _invocation = invocation;
        }

        /// <inheritdoc />
        public override TResult Invoke()
        {
            var invocation = _invocation;
            if (invocation != null && Interlocked.CompareExchange(ref _invocation, null, invocation) == invocation)
            {
                _result = invocation.Invoke();
            }
            return _result;
        }
    }
}
