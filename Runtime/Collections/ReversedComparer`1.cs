using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// Represents a comparer whose comparison result is the reverse of the original comparer's.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public sealed class ReversedComparer<T> : IComparer<T>
    {
        /// <summary>
        /// Gets a comparer whose comparison result is the reverse of the default comparer for <typeparamref name="T"/>.
        /// </summary>
        public static ReversedComparer<T> Default { get; } = new(Comparer<T>.Default);

        private readonly IComparer<T> _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReversedComparer{T}"/> class using the default original comparer.
        /// </summary>
        public ReversedComparer()
        {
            _comparer = Comparer<T>.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReversedComparer{T}"/> class using the specified original comparer.
        /// </summary>
        /// <param name="comparer">The original comparer; if it is <see langword="null"/>, the default original comparer is used.</param>
        public ReversedComparer(IComparer<T> comparer)
        {
            _comparer = comparer ?? Comparer<T>.Default;
        }

        /// <inheritdoc />
        public int Compare(T x, T y)
        {
            return _comparer.Compare(y, x);
        }
    }
}
