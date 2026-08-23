using System;
using System.Threading;

namespace Aurora
{
    /// <summary>
    /// Wraps another <see cref="Invocation"/> to ensure that only the first call to <see cref="Invoke"/> is actually executed.
    /// </summary>
    public sealed class OneTimeInvocation : Invocation
    {
        private Invocation _invocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneTimeInvocation"/> class.
        /// </summary>
        /// <param name="invocation">An <see cref="Invocation"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="invocation"/> is <see langword="null"/>.</exception>
        public OneTimeInvocation(Invocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            _invocation = invocation;
        }

        /// <inheritdoc />
        public override void Invoke()
        {
            var invocation = _invocation;
            if (invocation != null && Interlocked.CompareExchange(ref _invocation, null, invocation) == invocation)
            {
                invocation.Invoke();
            }
        }
    }
}
