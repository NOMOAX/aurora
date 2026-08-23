using System;
using System.Diagnostics;

namespace Aurora.Collections
{
    internal sealed class DequeDebugView<T>
    {
        private readonly Deque<T> _deque;

        public DequeDebugView(Deque<T> deque)
        {
            _deque = deque ?? throw new ArgumentNullException(nameof(deque));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _deque.ToArray();
    }
}
