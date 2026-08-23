using System;
using System.Collections;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// Implements <see cref="IEnumerator{T}"/> using the null-object pattern.
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
    public sealed class NullEnumerator<T> : IEnumerator<T>
    {
        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static NullEnumerator<T> Instance { get; } = new();

        private NullEnumerator()
        {
        }

        bool IEnumerator.MoveNext()
        {
            return false;
        }

        T IEnumerator<T>.Current => default;

        object IEnumerator.Current => default(T);

        void IEnumerator.Reset()
        {
        }

        void IDisposable.Dispose()
        {
        }
    }
}
