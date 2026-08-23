using System;
using System.Collections.Generic;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// A using scope for a list.
    /// </summary>
    /// <typeparam name="T">The type of the list's members.</typeparam>
    public sealed class ListUsingScope<T> : IDisposable
    {
        private List<T> _list;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListUsingScope{T}"/> class.
        /// </summary>
        /// <param name="list">This output parameter is assigned an empty list.</param>
        public ListUsingScope(out List<T> list)
        {
            _list = PredefinedPools<T>.List.Get();
            list  = _list;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var list = _list;
            if (list != null && Interlocked.CompareExchange(ref _list, null, list) == list)
            {
                PredefinedPools<T>.List.Return(list);
            }
        }
    }
}
