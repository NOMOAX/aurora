using System;
using System.Collections.Generic;

namespace Aurora
{
    /// <summary>
    /// Used to generate global pseudo-random numbers.
    /// </summary>
    public static class RandomUtility
    {
        /// <summary>
        /// Gets a thread-safe <see cref="Random"/> instance that can be used concurrently from any thread.
        /// </summary>
        public static ThreadSafeRandom Shared => ThreadSafeRandom.Instance;

        /// <summary>
        /// Gets a <see cref="bool"/> value that is <see langword="true"/> with probability <paramref name="probability"/> and <see langword="false"/> with probability 1 - <paramref name="probability"/>.
        /// </summary>
        /// <param name="probability">The probability. It should be greater than or equal to 0 and less than or equal to 1.</param>
        /// <returns>A <see cref="bool"/> value that is <see langword="true"/> with probability <paramref name="probability"/> and <see langword="false"/> with probability 1 - <paramref name="probability"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="probability"/> is not a number or is less than 0 or is greater than 1.</exception>
        public static bool P(double probability)
        {
            if (probability is double.NaN or < 0 or > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(probability));
            }
            return probability > ThreadSafeRandom.Instance.NextDouble();
        }

        /// <summary>
        /// Specifies the weight of each element and randomly selects one element from the collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="weights">The weight of each element in the collection.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <returns>If an element was selected, that element; otherwise, the default value of <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="weights"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> contains fewer elements than <paramref name="weights"/>, or <paramref name="weights"/> contains at least one element that is not a number or is less than 0.</exception>
        /// <exception cref="NotSupportedException">Accumulating <paramref name="weights"/> from back to front reaches positive infinity at some step.</exception>
        public static T Choose<T>(IList<T> collection, IList<double> weights)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            return Choose(collection, weights, 0, collection.Count);
        }

        /// <summary>
        /// Specifies the weight of each element and randomly selects one element from the collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="weights">The weight of each element in the collection.</param>
        /// <param name="index">The starting index of the selection range.</param>
        /// <param name="count">The number of elements in the selection range.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <returns>If an element was selected, that element; otherwise, the default value of <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="weights"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="count"/> is less than 1.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="collection"/>, or <paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="weights"/>, or <paramref name="weights"/> contains at least one element that is not a number or is less than 0.</exception>
        /// <exception cref="NotSupportedException">Accumulating <paramref name="weights"/> from back to front reaches positive infinity at some step.</exception>
        public static T Choose<T>(IList<T> collection, IList<double> weights, int index, int count)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (weights == null)
            {
                throw new ArgumentNullException(nameof(weights));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, null);
            }
            if (index + count > collection.Count)
            {
                throw new ArgumentException();
            }
            if (index + count > weights.Count)
            {
                throw new ArgumentException();
            }
            var chosenIndex = GetChosenIndex(weights, index, count);
            return chosenIndex >= 0 ? collection[chosenIndex] : default;
        }

        /// <summary>
        /// Specifies the weight of each element, tries to randomly select one element from the collection, and returns the selected element through an output parameter.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="weights">The weight of each element in the collection.</param>
        /// <param name="chosen">If an element was selected, that element; otherwise, the default value of <typeparamref name="T"/>.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <returns><see langword="true"/> if an element was selected; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="weights"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> contains fewer elements than <paramref name="weights"/>, or <paramref name="weights"/> contains at least one element that is not a number or is less than 0.</exception>
        /// <exception cref="NotSupportedException">Accumulating <paramref name="weights"/> from back to front reaches positive infinity at some step.</exception>
        public static bool TryChoose<T>(IList<T> collection, IList<double> weights, out T chosen)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            return TryChoose(collection, weights, 0, collection.Count, out chosen);
        }

        /// <summary>
        /// Specifies the weight of each element, tries to randomly select one element from the collection, and returns the selected element through an output parameter.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="weights">The weight of each element in the collection.</param>
        /// <param name="index">The starting index of the selection range.</param>
        /// <param name="count">The number of elements in the selection range.</param>
        /// <param name="chosen">If an element was selected, that element; otherwise, the default value of <typeparamref name="T"/>.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <returns><see langword="true"/> if an element was selected; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="weights"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="count"/> is less than 1.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="collection"/>, or <paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="weights"/>, or <paramref name="weights"/> contains at least one element that is not a number or is less than 0.</exception>
        /// <exception cref="NotSupportedException">Accumulating <paramref name="weights"/> from back to front reaches positive infinity at some step.</exception>
        public static bool TryChoose<T>(IList<T> collection, IList<double> weights, int index, int count, out T chosen)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (weights == null)
            {
                throw new ArgumentNullException(nameof(weights));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, null);
            }
            if (index + count > collection.Count)
            {
                throw new ArgumentException();
            }
            if (index + count > weights.Count)
            {
                throw new ArgumentException();
            }
            var chosenIndex = GetChosenIndex(weights, index, count);
            var result      = chosenIndex >= 0;
            chosen = result ? collection[chosenIndex] : default;
            return result;
        }

        /// <summary>
        /// Randomly selects by weight among a set of specified values and returns the index of the selected weight value.
        /// </summary>
        /// <param name="weights">The collection of weight values.</param>
        /// <returns>If a weight value was selected, the index of that weight value in the collection of weight values; otherwise, a negative number.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="weights"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">Accumulating <paramref name="weights"/> from back to front reaches positive infinity at some step.</exception>
        public static int GetChosenIndex(IList<double> weights)
        {
            if (weights == null)
            {
                throw new ArgumentNullException(nameof(weights));
            }
            return GetChosenIndex(weights, 0, weights.Count);
        }

        /// <summary>
        /// Randomly selects by weight among a set of specified values and returns the index of the selected weight value.
        /// </summary>
        /// <param name="weights">The collection of weight values.</param>
        /// <param name="index">The starting index of the selection range.</param>
        /// <param name="count">The number of elements in the selection range.</param>
        /// <returns>If a weight value was selected, the index of that weight value in the collection of weight values; otherwise, a negative number.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="weights"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="count"/> is less than 1.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="weights"/>, or <paramref name="weights"/> contains at least one element that is not a number or is less than 0.</exception>
        /// <exception cref="NotSupportedException">Accumulating <paramref name="weights"/> from back to front reaches positive infinity at some step.</exception>
        public static int GetChosenIndex(IList<double> weights, int index, int count)
        {
            if (weights == null)
            {
                throw new ArgumentNullException(nameof(weights));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, null);
            }
            if (index + count > weights.Count)
            {
                throw new ArgumentException();
            }
            var last = weights[index + count - 1];
            if (last is double.NaN)
            {
                throw new ArgumentException(
                    $"{nameof(weights)} contains at least one element that is not a number",
                    nameof(weights)
                );
            }
            if (last < 0)
            {
                throw new ArgumentException(
                    $"{nameof(weights)} contains at least one element less than 0",
                    nameof(weights)
                );
            }
            var sum  = last;
            var sums = new double[count - 1];
            for (var sumIndex = count - 2; sumIndex >= 0; sumIndex--)
            {
                var weightIndex = index + sumIndex;
                var weight      = weights[weightIndex];
                if (weight is double.NaN)
                {
                    throw new ArgumentException(
                        $"{nameof(weights)} contains at least one element that is not a number",
                        nameof(weights)
                    );
                }
                if (weight < 0)
                {
                    throw new ArgumentException(
                        $"{nameof(weights)} contains at least one element less than 0",
                        nameof(weights)
                    );
                }
                sum += weight;
                if (sum is double.PositiveInfinity)
                {
                    throw new NotSupportedException(
                        $"{nameof(weights)} would reach positive infinity at some step when accumulated from back to front, so the calculation cannot be performed"
                    );
                }
                sums[sumIndex] = sum;
            }
            for (var sumIndex = 0; sumIndex < count - 1; sumIndex++)
            {
                var weightIndex = index + sumIndex;
                var weight      = weights[weightIndex];
                if (weight == 0)
                {
                    continue;
                }
                if (P(weight / sums[sumIndex]))
                {
                    return sumIndex;
                }
            }
            return last != 0 ? index + count - 1 : -1;
        }
    }
}
