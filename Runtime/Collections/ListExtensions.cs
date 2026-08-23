using System;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// Provides extension methods for the <see cref="IList{T}"/> interface.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Shuffles the order of elements in the current <see cref="IList{T}"/>.
        /// </summary>
        /// <param name="collection">The collection to shuffle.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        public static void ShuffleInPlace<T>(this IList<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            collection.ShuffleInPlace(0, collection.Count);
        }

        /// <summary>
        /// Shuffles the order of elements in the specified range of the current <see cref="IList{T}"/>.
        /// </summary>
        /// <param name="collection">The collection to shuffle.</param>
        /// <param name="index">The starting index of the range to shuffle.</param>
        /// <param name="count">The number of elements in the range to shuffle.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not specify a valid range within <paramref name="collection"/>.</exception>
        public static void ShuffleInPlace<T>(this IList<T> collection, int index, int count)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, null);
            }
            if (collection.Count - index < count)
            {
                throw new ArgumentException();
            }
            for (var i = index + count - 1; i > index; i--)
            {
                var j = RandomUtility.Shared.Next(index, i + 1);
                if (i == j)
                {
                    continue;
                }
                (collection[i], collection[j]) = (collection[j], collection[i]);
            }
        }

        /// <summary>
        /// Searches for an element matching the specified condition and returns the first matching element in the current <see cref="IList{T}"/>.
        /// </summary>
        /// <param name="collection">The collection to search.</param>
        /// <param name="match">The condition.</param>
        /// <param name="state">The second parameter passed to <paramref name="match"/>.</param>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TState">The type of <paramref name="state"/>.</typeparam>
        /// <returns>If the first element matching the condition defined by <paramref name="match"/> is found, that element; otherwise, the default value of type <typeparamref name="TSource"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="match"/> is <see langword="null"/>。</exception>
        public static TSource Find<TSource, TState>(
            this IList<TSource>                     collection,
            ParameterizedPredicate<TSource, TState> match,
            TState                                  state)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            foreach (var item in collection)
            {
                if (match(item, state))
                {
                    return item;
                }
            }
            return default;
        }

        /// <summary>
        /// Searches for an element matching the specified condition and returns the zero-based index of the first matching element in the current <see cref="IList{T}"/>.
        /// </summary>
        /// <param name="collection">The collection to search.</param>
        /// <param name="match">The condition.</param>
        /// <param name="state">The second parameter passed to <paramref name="match"/>.</param>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TState">The type of <paramref name="state"/>.</typeparam>
        /// <returns>If the first element matching the condition defined by <paramref name="match"/> is found, the zero-based index of that element; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="match"/> is <see langword="null"/>。</exception>
        public static int FindIndex<TSource, TState>(
            this IList<TSource>                     collection,
            ParameterizedPredicate<TSource, TState> match,
            TState                                  state)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            for (var i = 0; i < collection.Count; i++)
            {
                if (match(collection[i], state))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Searches for an element matching the specified condition and returns the last matching element in the current <see cref="IList{T}"/>.
        /// </summary>
        /// <param name="collection">The collection to search.</param>
        /// <param name="match">The condition.</param>
        /// <param name="state">The second parameter passed to <paramref name="match"/>.</param>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TState">The type of <paramref name="state"/>.</typeparam>
        /// <returns>If the last element matching the condition defined by <paramref name="match"/> is found, that element; otherwise, the default value of type <typeparamref name="TSource"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="match"/> is <see langword="null"/>。</exception>
        public static TSource FindLast<TSource, TState>(
            this IList<TSource>                     collection,
            ParameterizedPredicate<TSource, TState> match,
            TState                                  state)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            for (var i = collection.Count - 1; i >= 0; i--)
            {
                var item = collection[i];
                if (match(item, state))
                {
                    return item;
                }
            }
            return default;
        }

        /// <summary>
        /// Searches for an element matching the specified condition and returns the zero-based index of the last matching element in the current <see cref="IList{T}"/>.
        /// </summary>
        /// <param name="collection">The collection to search.</param>
        /// <param name="match">The condition.</param>
        /// <param name="state">The second parameter passed to <paramref name="match"/>.</param>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TState">The type of <paramref name="state"/>.</typeparam>
        /// <returns>If the last element matching the condition defined by <paramref name="match"/> is found, the zero-based index of that element; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="match"/> is <see langword="null"/>。</exception>
        public static int FindLastIndex<TSource, TState>(
            this IList<TSource>                     collection,
            ParameterizedPredicate<TSource, TState> match,
            TState                                  state)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            for (var i = collection.Count - 1; i >= 0; i--)
            {
                if (match(collection[i], state))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Converts the current <see cref="IList{T}"/> to a <see cref="List{T}"/> of another type.
        /// </summary>
        /// <param name="collection">The collection to convert to the target type.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="state">The second argument passed to the converter.</param>
        /// <typeparam name="TInput">The type of the source <see cref="IList{T}"/> elements.</typeparam>
        /// <typeparam name="TOutput">The type of the target <see cref="List{T}"/> elements.</typeparam>
        /// <returns>A <see cref="List{T}"/> of the target type containing the elements converted from the source <see cref="IList{T}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="converter"/> is <see langword="null"/>.</exception>
        public static List<TOutput> ConvertAll<TInput, TOutput>(
            this IList<TInput>            collection,
            Func<TInput, object, TOutput> converter,
            object                        state)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }
            var count      = collection.Count;
            var outputList = new List<TOutput>(count);
            for (var i = 0; i < count; i++)
            {
                outputList.Add(converter(collection[i], state));
            }
            return outputList;
        }

        /// <summary>
        /// Removes all elements matching the specified condition from the current <see cref="IList{T}"/>.
        /// </summary>
        /// <param name="collection">The collection whose elements are to be removed.</param>
        /// <param name="match">The condition.</param>
        /// <param name="state">The second parameter passed to <paramref name="match"/>.</param>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TState">The type of <paramref name="state"/>.</typeparam>
        /// <returns>The number of elements removed from the <see cref="IList{T}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="match"/> is <see langword="null"/>。</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> is read-only.</exception>
        public static int RemoveAll<TSource, TState>(
            this IList<TSource>                     collection,
            ParameterizedPredicate<TSource, TState> match,
            TState                                  state)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (collection.IsReadOnly)
            {
                throw new ArgumentException();
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            var freeIndex = 0;
            var count     = collection.Count;
            while (freeIndex < count && !match(collection[freeIndex], state))
            {
                freeIndex++;
            }
            if (freeIndex == count)
            {
                return 0;
            }
            var current = freeIndex + 1;
            while (current < count)
            {
                while (current < count && match(collection[current], state))
                {
                    current++;
                }
                if (current < count)
                {
                    collection[freeIndex++] = collection[current++];
                }
            }
            if (collection is List<TSource> list)
            {
                list.RemoveRange(freeIndex, count - freeIndex);
            }
            else
            {
                var count1 = count;
                while (count1-- > freeIndex)
                {
                    collection.RemoveAt(count1);
                }
            }
            return count - freeIndex;
        }
    }
}
