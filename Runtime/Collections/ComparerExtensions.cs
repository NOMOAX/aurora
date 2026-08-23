using System;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// Provides extension methods for the <see cref="IComparer{T}"/> interface.
    /// </summary>
    public static class ComparerExtensions
    {
        /// <summary>
        /// Gets a comparer that reverses the comparison result of the current <see cref="IComparer{T}"/>.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <returns>A comparer that reverses the comparison result of the current <see cref="IComparer{T}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
        public static IComparer<T> Reversed<T>(this IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            return new ReversedComparer<T>(comparer);
        }
    }
}
