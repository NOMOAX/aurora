using System;
using System.Collections.Generic;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// A using scope for a hash set.
    /// </summary>
    /// <typeparam name="T">The type of the hash set's members.</typeparam>
    public sealed class HashSetUsingScope<T> : IDisposable
    {
        private HashSet<T> _hashSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSetUsingScope{T}"/> class.
        /// </summary>
        /// <param name="hashSet">This output parameter is assigned an empty hash set.</param>
        public HashSetUsingScope(out HashSet<T> hashSet)
        {
            _hashSet = PredefinedPools<T>.HashSet.Get();
            hashSet  = _hashSet;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var hashSet = _hashSet;
            if (hashSet != null && Interlocked.CompareExchange(ref _hashSet, null, hashSet) == hashSet)
            {
                PredefinedPools<T>.HashSet.Return(hashSet);
            }
        }
    }
}
