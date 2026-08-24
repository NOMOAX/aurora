using System;
using System.Threading;
using Aurora.Collections;

namespace Aurora.Pooling
{
    /// <summary>
    /// A using scope for a deque.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the deque.</typeparam>
    public sealed class DequeUsingScope<T> : IDisposable
    {
        private Deque<T> _deque;

        /// <summary>
        /// Initializes a new instance of the <see cref="DequeUsingScope{T}"/> class.
        /// </summary>
        /// <param name="deque">This output parameter is assigned an empty deque.</param>
        public DequeUsingScope(out Deque<T> deque)
        {
            _deque = PredefinedPools<T>.Deque.Get();
            deque  = _deque;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var deque = _deque;
            if (deque != null && Interlocked.CompareExchange(ref _deque, null, deque) == deque)
            {
                PredefinedPools<T>.Deque.Return(deque);
            }
        }
    }
}
