using System;

namespace Aurora.Collections
{
    /// <summary>
    /// Provides extension methods for <see cref="Array"/>.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Shuffles the order of elements in the current array.
        /// </summary>
        /// <param name="array">The array to shuffle.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        public static void ShuffleInPlace<T>(this T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            array.ShuffleInPlace(0, array.Length);
        }

        /// <summary>
        /// Shuffles the order of elements in the specified range of the current array.
        /// </summary>
        /// <param name="array">The array to shuffle.</param>
        /// <param name="index">The starting index of the range to shuffle.</param>
        /// <param name="count">The number of elements in the range to shuffle.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not specify a valid range within <paramref name="array"/>.</exception>
        public static void ShuffleInPlace<T>(this T[] array, int index, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, null);
            }
            if (array.Length - index < count)
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
                (array[i], array[j]) = (array[j], array[i]);
            }
        }

        /// <summary>
        /// Searches for an element matching the specified condition and returns the first matching element in the current array.
        /// </summary>
        /// <param name="array">The array to search.</param>
        /// <param name="match">The condition.</param>
        /// <param name="state">The second parameter passed to <paramref name="match"/>.</param>
        /// <typeparam name="TSource">The type of the elements in the array.</typeparam>
        /// <typeparam name="TState">The type of <paramref name="state"/>.</typeparam>
        /// <returns>If the first element matching the condition defined by <paramref name="match"/> is found, that element; otherwise, the default value of type <typeparamref name="TSource"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> or <paramref name="match"/> is <see langword="null"/>.</exception>
        public static TSource Find<TSource, TState>(
            this TSource[]                          array,
            ParameterizedPredicate<TSource, TState> match,
            TState                                  state)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            foreach (var item in array)
            {
                if (match(item, state))
                {
                    return item;
                }
            }
            return default;
        }

        /// <summary>
        /// Searches for an element matching the specified condition and returns the zero-based index of the first matching element in the current array.
        /// </summary>
        /// <param name="array">The array to search.</param>
        /// <param name="match">The condition.</param>
        /// <param name="state">The second parameter passed to <paramref name="match"/>.</param>
        /// <typeparam name="TSource">The type of the elements in the array.</typeparam>
        /// <typeparam name="TState">The type of <paramref name="state"/>.</typeparam>
        /// <returns>If the first element matching the condition defined by <paramref name="match"/> is found, the zero-based index of that element; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> or <paramref name="match"/> is <see langword="null"/>.</exception>
        public static int FindIndex<TSource, TState>(
            this TSource[]                          array,
            ParameterizedPredicate<TSource, TState> match,
            TState                                  state)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            for (var i = 0; i < array.Length; i++)
            {
                if (match(array[i], state))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Searches for an element matching the specified condition and returns the last matching element in the current array.
        /// </summary>
        /// <param name="array">The array to search.</param>
        /// <param name="match">The condition.</param>
        /// <param name="state">The second parameter passed to <paramref name="match"/>.</param>
        /// <typeparam name="TSource">The type of the elements in the array.</typeparam>
        /// <typeparam name="TState">The type of <paramref name="state"/>.</typeparam>
        /// <returns>If the last element matching the condition defined by <paramref name="match"/> is found, that element; otherwise, the default value of type <typeparamref name="TSource"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> or <paramref name="match"/> is <see langword="null"/>.</exception>
        public static TSource FindLast<TSource, TState>(
            this TSource[]                          array,
            ParameterizedPredicate<TSource, TState> match,
            TState                                  state)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            for (var i = array.Length - 1; i >= 0; i--)
            {
                var item = array[i];
                if (match(item, state))
                {
                    return item;
                }
            }
            return default;
        }

        /// <summary>
        /// Searches for an element matching the specified condition and returns the zero-based index of the last matching element in the current array.
        /// </summary>
        /// <param name="array">The array to search.</param>
        /// <param name="match">The condition.</param>
        /// <param name="state">The second parameter passed to <paramref name="match"/>.</param>
        /// <typeparam name="TSource">The type of the elements in the array.</typeparam>
        /// <typeparam name="TState">The type of <paramref name="state"/>.</typeparam>
        /// <returns>If the last element matching the condition defined by <paramref name="match"/> is found, the zero-based index of that element; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> or <paramref name="match"/> is <see langword="null"/>.</exception>
        public static int FindLastIndex<TSource, TState>(
            this TSource[]                          array,
            ParameterizedPredicate<TSource, TState> match,
            TState                                  state)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            for (var i = array.Length - 1; i >= 0; i--)
            {
                if (match(array[i], state))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Converts an array of one type to an array of another type.
        /// </summary>
        /// <param name="array">The array to convert to the target type.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="state">The second argument passed to the converter.</param>
        /// <typeparam name="TInput">The type of the source array's elements.</typeparam>
        /// <typeparam name="TOutput">The type of the target array's elements.</typeparam>
        /// <returns>An array of the target type containing the elements converted from the source array.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> or <paramref name="converter"/> is <see langword="null"/>.</exception>
        /// <remarks>This method extends <see cref="Array.ConvertAll{TInput,TOutput}"/> by allowing user-defined state to be passed in, avoiding the use of closures.</remarks>
        public static TOutput[] ConvertAll<TInput, TOutput>(
            this TInput[]                 array,
            Func<TInput, object, TOutput> converter,
            object                        state)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }
            var length      = array.Length;
            var outputArray = new TOutput[length];
            for (var i = 0; i < length; ++i)
            {
                outputArray[i] = converter(array[i], state);
            }
            return outputArray;
        }
    }
}
