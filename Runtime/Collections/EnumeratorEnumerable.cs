using System;
using System.Collections;

namespace Aurora.Collections
{
    /// <summary>
    /// Represents an enumerable object that uses the specified enumerator.
    /// </summary>
    public readonly struct EnumeratorEnumerable : IEnumerable
    {
        private readonly IEnumerator _enumerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumeratorEnumerable"/> struct.
        /// </summary>
        /// <param name="enumerator">The enumerator.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerator"/> is <see langword="null"/>.</exception>
        public EnumeratorEnumerable(IEnumerator enumerator)
        {
            _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
        }

        /// <inheritdoc />
        public IEnumerator GetEnumerator()
        {
            return _enumerator;
        }
    }
}
