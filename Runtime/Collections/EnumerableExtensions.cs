using System;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// Provides extension methods for the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Searches the current <see cref="IEnumerable{T}"/> for the specified object using the default equality comparer, and returns the index of its first match.
        /// </summary>
        /// <param name="source">The sequence to search.</param>
        /// <param name="value">The object to search for in <paramref name="source"/>.</param>
        /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
        /// <returns>The zero-based index of the first match of <paramref name="value"/> in the whole <paramref name="source"/>, if found; otherwise, a negative number.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        public static int IndexOf<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case TSource[] array:
                    return Array.IndexOf(array, value);
                case IList<TSource> list:
                    return list.IndexOf(value);
                default:
                {
                    using var enumerator = source.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        var equalityComparer = EqualityComparer<TSource>.Default;
                        var currentIndex     = -1;
                        var foundIndex       = -1;
                        do
                        {
                            checked
                            {
                                ++currentIndex;
                            }
                            var current = enumerator.Current;
                            if (equalityComparer.Equals(current, value))
                            {
                                foundIndex = currentIndex;
                                break;
                            }
                        } while (enumerator.MoveNext());
                        return foundIndex;
                    }
                    return -1;
                }
            }
        }

        /// <summary>
        /// Searches the current <see cref="IEnumerable{T}"/> for the specified object using the specified equality comparer, and returns the index of its first match.
        /// </summary>
        /// <param name="source">The sequence to search.</param>
        /// <param name="value">The object to search for in <paramref name="source"/>.</param>
        /// <param name="equalityComparer">A comparer used to determine whether two <typeparamref name="TSource"/> objects are equal.</param>
        /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
        /// <returns>The index of the first match of <paramref name="value"/> in the whole <paramref name="source"/>, if found; otherwise, a negative number.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        public static int IndexOf<TSource>(
            this IEnumerable<TSource>  source,
            TSource                    value,
            IEqualityComparer<TSource> equalityComparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            using var enumerator = source.GetEnumerator();
            if (enumerator.MoveNext())
            {
                equalityComparer ??= EqualityComparer<TSource>.Default;
                var currentIndex = -1;
                var foundIndex   = -1;
                do
                {
                    checked
                    {
                        ++currentIndex;
                    }
                    var current = enumerator.Current;
                    if (equalityComparer.Equals(current, value))
                    {
                        foundIndex = currentIndex;
                        break;
                    }
                } while (enumerator.MoveNext());
                return foundIndex;
            }
            return -1;
        }

        /// <summary>
        /// Searches the current <see cref="IEnumerable{T}"/> for the specified object using the default equality comparer, and returns the index of its last match.
        /// </summary>
        /// <param name="source">The sequence to search.</param>
        /// <param name="value">The object to search for in <paramref name="source"/>.</param>
        /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
        /// <returns>The zero-based index of the last match of <paramref name="value"/> in the whole <paramref name="source"/>, if found; otherwise, a negative number.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        public static int LastIndexOf<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case TSource[] array:
                    return Array.LastIndexOf(array, value);
                case List<TSource> list:
                    return list.LastIndexOf(value);
                default:
                {
                    using var enumerator = source.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        var equalityComparer = EqualityComparer<TSource>.Default;
                        var currentIndex     = -1;
                        var foundIndex       = -1;
                        do
                        {
                            checked
                            {
                                ++currentIndex;
                            }
                            var current = enumerator.Current;
                            if (equalityComparer.Equals(current, value))
                            {
                                foundIndex = currentIndex;
                            }
                        } while (enumerator.MoveNext());
                        return foundIndex;
                    }
                    return -1;
                }
            }
        }

        /// <summary>
        /// Searches the current <see cref="IEnumerable{T}"/> for the specified object using the specified equality comparer, and returns the index of its last match.
        /// </summary>
        /// <param name="source">The sequence to search.</param>
        /// <param name="value">The object to search for in <paramref name="source"/>.</param>
        /// <param name="equalityComparer">A comparer used to determine whether two <typeparamref name="TSource"/> objects are equal.</param>
        /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
        /// <returns>The zero-based index of the last match of <paramref name="value"/> in the whole <paramref name="source"/>, if found; otherwise, a negative number.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        public static int LastIndexOf<TSource>(
            this IEnumerable<TSource>  source,
            TSource                    value,
            IEqualityComparer<TSource> equalityComparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            using var enumerator = source.GetEnumerator();
            if (enumerator.MoveNext())
            {
                equalityComparer ??= EqualityComparer<TSource>.Default;
                var currentIndex = -1;
                var foundIndex   = -1;
                do
                {
                    checked
                    {
                        ++currentIndex;
                    }
                    var current = enumerator.Current;
                    if (equalityComparer.Equals(current, value))
                    {
                        foundIndex = currentIndex;
                    }
                } while (enumerator.MoveNext());
                return foundIndex;
            }
            return -1;
        }

        /// <summary>
        /// Searches the current <see cref="IEnumerable{T}"/> for matching elements using the specified condition, and returns the index of its first match.
        /// </summary>
        /// <param name="source">The sequence to search.</param>
        /// <param name="match">The condition used to search the elements of <paramref name="source"/>.</param>
        /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
        /// <returns>The index of the first match satisfying <paramref name="match"/> in the whole <paramref name="source"/>, if found; otherwise, a negative number.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="match"/> is <see langword="null"/>.</exception>
        public static int FindIndex<TSource>(this IEnumerable<TSource> source, Predicate<TSource> match)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            using var enumerator = source.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var currentIndex = -1;
                var foundIndex   = -1;
                do
                {
                    checked
                    {
                        ++currentIndex;
                    }
                    var current = enumerator.Current;
                    if (match(current))
                    {
                        foundIndex = currentIndex;
                        break;
                    }
                } while (enumerator.MoveNext());
                return foundIndex;
            }
            return -1;
        }

        /// <summary>
        /// Searches the current <see cref="IEnumerable{T}"/> for matching elements using the specified condition, and returns the index of its last match.
        /// </summary>
        /// <param name="source">The sequence to search.</param>
        /// <param name="match">The condition used to search the objects of <paramref name="source"/>.</param>
        /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
        /// <returns>The index of the last match satisfying <paramref name="match"/> in the whole <paramref name="source"/>, if found; otherwise, a negative number.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="match"/> is <see langword="null"/>.</exception>
        public static int FindLastIndex<TSource>(this IEnumerable<TSource> source, Predicate<TSource> match)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            using var enumerator = source.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var currentIndex = -1;
                var foundIndex   = -1;
                do
                {
                    checked
                    {
                        ++currentIndex;
                    }
                    var current = enumerator.Current;
                    if (match(current))
                    {
                        foundIndex = currentIndex;
                    }
                } while (enumerator.MoveNext());
                return foundIndex;
            }
            return -1;
        }
    }
}
