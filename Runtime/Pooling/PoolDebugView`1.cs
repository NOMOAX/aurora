using System;
using System.Diagnostics;

namespace Aurora.Pooling
{
    internal sealed class PoolDebugView<T> where T : class
    {
        private readonly Pool<T> _pool;

        public PoolDebugView(Pool<T> pool)
        {
            _pool = pool ?? throw new ArgumentNullException(nameof(pool));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _pool.GetItems();
    }
}
