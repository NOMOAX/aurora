using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Aurora.Collections
{
    /// <summary>
    /// Represents a comparer that compares the hash codes of two objects.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public sealed class HashCodeComparer<T> : IComparer<T> where T : class
    {
        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static HashCodeComparer<T> Instance { get; } = new();

        private HashCodeComparer()
        {
        }

        /// <inheritdoc />
        public int Compare(T x, T y)
        {
            return Comparer<int>.Default.Compare(GetHashCode(x), GetHashCode(y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetHashCode(T obj)
        {
            return obj != null ? obj.GetHashCode() : 0;
        }
    }
}
