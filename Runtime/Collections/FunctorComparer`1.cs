using System;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// Represents a comparer that compares two objects using a specified method.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public sealed class FunctorComparer<T> : IComparer<T>
    {
        private readonly Comparison<T> _comparison;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctorComparer{T}"/> class.
        /// </summary>
        /// <param name="comparison">A method used to compare two objects of type <typeparamref name="T"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <see langword="null"/>.</exception>
        public FunctorComparer(Comparison<T> comparison)
        {
            _comparison = comparison ?? throw new ArgumentNullException(nameof(comparison));
        }

        /// <inheritdoc />
        public int Compare(T x, T y)
        {
            return _comparison(x, y);
        }
    }
}
