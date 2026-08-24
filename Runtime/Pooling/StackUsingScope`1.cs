using System;
using System.Collections.Generic;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// A using scope for a stack.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the stack.</typeparam>
    public sealed class StackUsingScope<T> : IDisposable
    {
        private Stack<T> _stack;

        /// <summary>
        /// Initializes a new instance of the <see cref="StackUsingScope{T}"/> class.
        /// </summary>
        /// <param name="stack">This output parameter is assigned an empty stack.</param>
        public StackUsingScope(out Stack<T> stack)
        {
            _stack = PredefinedPools<T>.Stack.Get();
            stack  = _stack;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var stack = _stack;
            if (stack != null && Interlocked.CompareExchange(ref _stack, null, stack) == stack)
            {
                PredefinedPools<T>.Stack.Return(stack);
            }
        }
    }
}
