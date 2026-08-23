using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Aurora.Collections
{
    /// <summary>
    /// Represents an enumerable object that uses the specified enumerator.
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
    public readonly struct EnumeratorEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerator<T> _enumerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumeratorEnumerable{T}"/> struct.
        /// </summary>
        /// <param name="enumerator">The enumerator.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerator"/> is <see langword="null"/>.</exception>
        public EnumeratorEnumerable(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumeratorEnumerable{T}"/> struct.
        /// </summary>
        /// <param name="enumerator">The enumerator.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerator"/> is <see langword="null"/>.</exception>
        /// <remarks>If the enumerated object cannot be converted to type <typeparamref name="T"/>, an <see cref="InvalidCastException"/> is thrown during enumeration.</remarks>
        public EnumeratorEnumerable(IEnumerator enumerator)
        {
            if (enumerator == null)
            {
                throw new ArgumentNullException(nameof(enumerator));
            }
            _enumerator = new EnumeratorEnumerable(enumerator).Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _enumerator;
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return _enumerator;
        }
    }
}
