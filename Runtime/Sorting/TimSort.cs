using System;
using System.Collections.Generic;
using Aurora.Pooling;

namespace Aurora.Sorting
{
    /// <summary>
    /// The Tim Peters sort algorithm.
    /// </summary>
    public static class TimSort
    {
        private const int MinMerge = 32;

        private const int MinGallop = 7;

        /// <summary>
        /// Sorts the elements in the collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        public static void Sort<T>(IList<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            var count = collection.Count;
            if (count < 2)
            {
                return;
            }
            InternalSort(collection, 0, count, Comparer<T>.Default);
        }

        /// <summary>
        /// Sorts the elements in the collection using the specified comparer.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="comparer">The comparer used to compare elements. If it is <see langword="null"/>, the default comparer (<see cref="Comparer{T}.Default"/>) is used.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        public static void Sort<T>(IList<T> collection, IComparer<T> comparer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            var count = collection.Count;
            if (count < 2)
            {
                return;
            }
            InternalSort(collection, 0, count, comparer ?? Comparer<T>.Default);
        }

        /// <summary>
        /// Sorts part of the elements in the collection using the specified comparer.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="index">The starting index of the sort range.</param>
        /// <param name="count">The number of elements in the sort range.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="collection"/>.</exception>
        public static void Sort<T>(IList<T> collection, int index, int count)
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
            if (index + count > collection.Count)
            {
                throw new ArgumentException(
                    $"{nameof(index)} plus {nameof(count)} is greater than {nameof(collection)}'s number of elements"
                );
            }
            if (count < 2)
            {
                return;
            }
            InternalSort(collection, index, count, Comparer<T>.Default);
        }

        /// <summary>
        /// Sorts part of the elements in the collection using the specified comparer.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="index">The starting index of the sort range.</param>
        /// <param name="count">The number of elements in the sort range.</param>
        /// <param name="comparer">The comparer used to compare elements. If it is <see langword="null"/>, the default comparer (<see cref="Comparer{T}.Default"/>) is used.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="collection"/>.</exception>
        public static void Sort<T>(IList<T> collection, int index, int count, IComparer<T> comparer)
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
            if (index + count > collection.Count)
            {
                throw new ArgumentException(
                    $"{nameof(index)} plus {nameof(count)} is greater than {nameof(collection)}'s number of elements"
                );
            }
            if (count < 2)
            {
                return;
            }
            InternalSort(collection, index, count, comparer ?? Comparer<T>.Default);
        }

        /// <summary>
        /// Uses the specified sorter. Sorts the elements in the key collection and correspondingly changes the order of the elements in the value collection.
        /// </summary>
        /// <param name="keys">The key collection.</param>
        /// <param name="values">The value collection.</param>
        /// <typeparam name="TKey">The type of the elements in the key collection.</typeparam>
        /// <typeparam name="TValue">The type of the elements in the value collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="values"/> is not <see langword="null"/>, and the number of elements in <paramref name="values"/> is less than the number of elements in <paramref name="keys"/>.</exception>
        public static void Sort<TKey, TValue>(IList<TKey> keys, IList<TValue> values)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            var count = keys.Count;
            if (values != null && values.Count < count)
            {
                throw new ArgumentException(
                    $"{nameof(values)}'s number of elements is less than {nameof(keys)}'s number of elements",
                    nameof(values)
                );
            }
            if (count < 2)
            {
                return;
            }
            if (values == null)
            {
                InternalSort(keys, 0, count, Comparer<TKey>.Default);
            }
            else
            {
                InternalSort(keys, values, 0, count, Comparer<TKey>.Default);
            }
        }

        /// <summary>
        /// Uses the specified sorter. Sorts the elements in the key collection and correspondingly changes the order of the elements in the value collection.
        /// </summary>
        /// <param name="keys">The key collection.</param>
        /// <param name="values">The value collection.</param>
        /// <param name="comparer">The comparer used to compare elements. If it is <see langword="null"/>, the default comparer (<see cref="Comparer{T}.Default"/>) is used.</param>
        /// <typeparam name="TKey">The type of the elements in the key collection.</typeparam>
        /// <typeparam name="TValue">The type of the elements in the value collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="values"/> is not <see langword="null"/>, and the number of elements in <paramref name="values"/> is less than the number of elements in <paramref name="keys"/>.</exception>
        public static void Sort<TKey, TValue>(IList<TKey> keys, IList<TValue> values, IComparer<TKey> comparer)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            var count = keys.Count;
            if (values != null && values.Count < count)
            {
                throw new ArgumentException(
                    $"{nameof(values)}'s number of elements is less than {nameof(keys)}'s number of elements",
                    nameof(values)
                );
            }
            if (count < 2)
            {
                return;
            }
            if (values == null)
            {
                InternalSort(keys, 0, count, comparer ?? Comparer<TKey>.Default);
            }
            else
            {
                InternalSort(keys, values, 0, count, comparer ?? Comparer<TKey>.Default);
            }
        }

        /// <summary>
        /// Uses the specified sorter. Sorts part of the elements in the key collection and correspondingly changes the order of the elements in the value collection.
        /// </summary>
        /// <param name="keys">The key collection.</param>
        /// <param name="values">The value collection.</param>
        /// <param name="index">The starting index of the sort range.</param>
        /// <param name="count">The number of elements in the sort range.</param>
        /// <typeparam name="TKey">The type of the elements in the key collection.</typeparam>
        /// <typeparam name="TValue">The type of the elements in the value collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="keys"/>, or <paramref name="values"/> is not <see langword="null"/> and <paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="values"/>.</exception>
        public static void Sort<TKey, TValue>(IList<TKey> keys, IList<TValue> values, int index, int count)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, null);
            }
            if (index + count > keys.Count)
            {
                throw new ArgumentException(
                    $"{nameof(index)} plus {nameof(count)} is greater than {nameof(keys)}'s number of elements"
                );
            }
            if (values != null && index + count > values.Count)
            {
                throw new ArgumentException(
                    $"{nameof(index)} plus {nameof(count)} is greater than {nameof(values)}'s number of elements"
                );
            }
            if (count < 2)
            {
                return;
            }
            if (values == null)
            {
                InternalSort(keys, index, count, Comparer<TKey>.Default);
            }
            else
            {
                InternalSort(keys, values, index, count, Comparer<TKey>.Default);
            }
        }

        /// <summary>
        /// Uses the specified sorter. Sorts part of the elements in the key collection and correspondingly changes the order of the elements in the value collection.
        /// </summary>
        /// <param name="keys">The key collection.</param>
        /// <param name="values">The value collection.</param>
        /// <param name="index">The starting index of the sort range.</param>
        /// <param name="count">The number of elements in the sort range.</param>
        /// <param name="comparer">The comparer used to compare elements. If it is <see langword="null"/>, the default comparer (<see cref="Comparer{T}.Default"/>) is used.</param>
        /// <typeparam name="TKey">The type of the elements in the key collection.</typeparam>
        /// <typeparam name="TValue">The type of the elements in the value collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="keys"/>, or <paramref name="values"/> is not <see langword="null"/> and <paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="values"/>.</exception>
        public static void Sort<TKey, TValue>(
            IList<TKey>     keys,
            IList<TValue>   values,
            int             index,
            int             count,
            IComparer<TKey> comparer)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, null);
            }
            if (index + count > keys.Count)
            {
                throw new ArgumentException(
                    $"{nameof(index)} plus {nameof(count)} is greater than {nameof(keys)}'s number of elements"
                );
            }
            if (values != null && index + count > values.Count)
            {
                throw new ArgumentException(
                    $"{nameof(index)} plus {nameof(count)} is greater than {nameof(values)}'s number of elements"
                );
            }
            if (count < 2)
            {
                return;
            }
            if (values == null)
            {
                InternalSort(keys, index, count, comparer ?? Comparer<TKey>.Default);
            }
            else
            {
                InternalSort(keys, values, index, count, comparer ?? Comparer<TKey>.Default);
            }
        }

        private static void InternalSort<T>(IList<T> collection, int index, int count, IComparer<T> comparer)
        {
            if (count < MinMerge)
            {
                var runCount = CountRunAndMakeAscending(collection, index, count, comparer);
                BinaryInsertionSort.InternalSort(collection, index, count, index + runCount, comparer);
                return;
            }

            var runs = PredefinedPools<Run>.List.Get();
            try
            {
                var globalMinGallop = MinGallop;
                /*
                 * MergeLow and MergeHigh need extra space to merge two runs
                 * Uses the shorter of the two run lengths
                 * In the worst case, the two runs have equal lengths and it is the final merge, so an array of half the count length is directly initialized here as extra space
                 * Using a smaller value and growing it when appropriate is also fine,
                 * However, the run lengths from the top to the bottom of the stack roughly grow at a Fibonacci-sequence rate, so the performance impact of multiple reallocations must be considered
                 */
                var temp            = new T[count >> 1];
                // The minimum run length
                var minRunCount     = GetMinRunCount(count);
                do
                {
                    var runCount = CountRunAndMakeAscending(collection, index, count, comparer);
                    // The run length is not enough; grow it to minRunCount, but do not exceed the remaining element count
                    if (runCount < minRunCount)
                    {
                        var forceRunCount = Math.Min(minRunCount, count);
                        BinaryInsertionSort.InternalSort(collection, index, forceRunCount, index + runCount, comparer);
                        runCount = forceRunCount;
                    }
                    runs.Add(new Run(index, runCount));
                    MergeCollapse(collection, runs, ref globalMinGallop, temp, comparer);
                    index += runCount;
                    count -= runCount;
                } while (count > 0);
                MergeForceCollapse(collection, runs, ref globalMinGallop, temp, comparer);
            }
            finally
            {
                PredefinedPools<Run>.List.Return(runs);
            }
        }

        private static void InternalSort<TKey, TValue>(
            IList<TKey>     keys,
            IList<TValue>   values,
            int             index,
            int             count,
            IComparer<TKey> comparer)
        {
            if (count < MinMerge)
            {
                var runCount = CountRunAndMakeAscending(keys, values, index, count, comparer);
                BinaryInsertionSort.InternalSort(keys, values, index, count, index + runCount, comparer);
                return;
            }

            var runs = PredefinedPools<Run>.List.Get();
            try
            {
                var globalMinGallop = MinGallop;
                /*
                 * MergeLow and MergeHigh need extra space to merge two runs
                 * Uses the shorter of the two run lengths
                 * In the worst case, the two runs have equal lengths and it is the final merge, so an array of half the count length is directly initialized here as extra space
                 * Using a smaller value and growing it when appropriate is also fine,
                 * However, the run lengths from the top to the bottom of the stack roughly grow at a Fibonacci-sequence rate, so the performance impact of multiple reallocations must be considered
                 */
                var tempKeys        = new TKey[count >> 1];
                var tempValues      = new TValue[count >> 1];
                // The minimum run length
                var minRunCount     = GetMinRunCount(count);
                do
                {
                    var runCount = CountRunAndMakeAscending(keys, values, index, count, comparer);
                    // The run length is not enough; grow it to minRunCount, but do not exceed the remaining element count
                    if (runCount < minRunCount)
                    {
                        var forceRunCount = Math.Min(minRunCount, count);
                        BinaryInsertionSort.InternalSort(
                            keys,
                            values,
                            index,
                            forceRunCount,
                            index + runCount,
                            comparer
                        );
                        runCount = forceRunCount;
                    }
                    runs.Add(new Run(index, runCount));
                    MergeCollapse(keys, values, runs, ref globalMinGallop, tempKeys, tempValues, comparer);
                    index += runCount;
                    count -= runCount;
                } while (count > 0);
                MergeForceCollapse(keys, values, runs, ref globalMinGallop, tempKeys, tempValues, comparer);
            }
            finally
            {
                PredefinedPools<Run>.List.Return(runs);
            }
        }

        private static int GetMinRunCount(int count)
        {
            var r = 0;
            while (count >= MinMerge)
            {
                r     |=  count & 1;
                count >>= 1;
            }
            return count + r;
        }

        private static int CountRunAndMakeAscending<T>(IList<T> collection, int index, int count, IComparer<T> comparer)
        {
            if (count < 2)
            {
                return count;
            }
            // Compare the first two elements to determine whether the sequence is ascending or strictly descending
            var isAscending = comparer.Compare(collection[index], collection[index + 1]) <= 0;
            // Move the pointer to the third element; from then on, each round compares the element before the pointer and the element at the pointer
            var i           = index + 2;
            if (isAscending)
            {
                while (i < index + count && comparer.Compare(collection[i - 1], collection[i]) <= 0)
                {
                    i++;
                }
            }
            else
            {
                while (i < index + count && comparer.Compare(collection[i - 1], collection[i]) > 0)
                {
                    i++;
                }
                /*
                 * Reverse the strictly descending sequence to make it strictly ascending
                 * Since a strictly descending sequence has no equal elements, this operation does not break the stability of the sort
                 */
                ReverseRange(collection, index, i - index);
            }
            return i - index;
        }

        private static int CountRunAndMakeAscending<TKey, TValue>(
            IList<TKey>     keys,
            IList<TValue>   values,
            int             index,
            int             count,
            IComparer<TKey> comparer)
        {
            if (count < 2)
            {
                return count;
            }
            // Compare the first two elements to determine whether the sequence is ascending or strictly descending
            var isAscending = comparer.Compare(keys[index], keys[index + 1]) <= 0;
            // Move the pointer to the third element; from then on, each round compares the element before the pointer and the element at the pointer
            var i           = index + 2;
            if (isAscending)
            {
                while (i < index + count && comparer.Compare(keys[i - 1], keys[i]) <= 0)
                {
                    i++;
                }
            }
            else
            {
                while (i < index + count && comparer.Compare(keys[i - 1], keys[i]) > 0)
                {
                    i++;
                }
                /*
                 * Reverse the strictly descending sequence to make it strictly ascending
                 * Since a strictly descending sequence has no equal elements, this operation does not break the stability of the sort
                 */
                ReverseRange(keys, values, index, i - index);
            }
            return i - index;
        }

        private static void ReverseRange<T>(IList<T> collection, int index, int count)
        {
            for (int i = index, j = index + count - 1; i < j; i++, j--)
            {
                (collection[i], collection[j]) = (collection[j], collection[i]);
            }
        }

        private static void ReverseRange<TKey, TValue>(IList<TKey> keys, IList<TValue> values, int index, int count)
        {
            for (int i = index, j = index + count - 1; i < j; i++, j--)
            {
                (keys[i], keys[j])     = (keys[j], keys[i]);
                (values[i], values[j]) = (values[j], values[i]);
            }
        }

        private static void MergeCollapse<T>(
            IList<T>     collection,
            IList<Run>   runs,
            ref int      globalMinGallop,
            T[]          temp,
            IComparer<T> comparer)
        {
            while (runs.Count > 1)
            {
                var runIndex = runs.Count - 2;
                if (runIndex > 0 && runs[runIndex - 1].Count <= runs[runIndex].Count + runs[runIndex + 1].Count ||
                    runIndex > 1 && runs[runIndex - 2].Count <= runs[runIndex].Count + runs[runIndex - 1].Count)
                {
                    if (runs[runIndex - 1].Count < runs[runIndex + 1].Count)
                        runIndex--;
                }
                else if (runIndex < 0 || runs[runIndex].Count > runs[runIndex + 1].Count)
                {
                    break;
                }
                MergeAt(collection, runs, runIndex, ref globalMinGallop, temp, comparer);
            }
        }

        private static void MergeCollapse<TKey, TValue>(
            IList<TKey>     keys,
            IList<TValue>   values,
            IList<Run>      runs,
            ref int         globalMinGallop,
            TKey[]          tempKeys,
            TValue[]        tempValues,
            IComparer<TKey> comparer)
        {
            while (runs.Count > 1)
            {
                var runIndex = runs.Count - 2;
                if (runIndex > 0 && runs[runIndex - 1].Count <= runs[runIndex].Count + runs[runIndex + 1].Count ||
                    runIndex > 1 && runs[runIndex - 2].Count <= runs[runIndex].Count + runs[runIndex - 1].Count)
                {
                    if (runs[runIndex - 1].Count < runs[runIndex + 1].Count)
                        runIndex--;
                }
                else if (runIndex < 0 || runs[runIndex].Count > runs[runIndex + 1].Count)
                {
                    break;
                }
                MergeAt(keys, values, runs, runIndex, ref globalMinGallop, tempKeys, tempValues, comparer);
            }
        }

        private static void MergeForceCollapse<T>(
            IList<T>     collection,
            IList<Run>   runs,
            ref int      globalMinGallop,
            T[]          temp,
            IComparer<T> comparer)
        {
            while (runs.Count > 1)
            {
                var n = runs.Count - 2;
                if (n > 0 && runs[n - 1].Count < runs[n + 1].Count)
                {
                    n--;
                }
                MergeAt(collection, runs, n, ref globalMinGallop, temp, comparer);
            }
        }

        private static void MergeForceCollapse<TKey, TValue>(
            IList<TKey>     keys,
            IList<TValue>   values,
            IList<Run>      runs,
            ref int         globalMinGallop,
            TKey[]          tempKeys,
            TValue[]        tempValues,
            IComparer<TKey> comparer)
        {
            while (runs.Count > 1)
            {
                var n = runs.Count - 2;
                if (n > 0 && runs[n - 1].Count < runs[n + 1].Count)
                {
                    n--;
                }
                MergeAt(keys, values, runs, n, ref globalMinGallop, tempKeys, tempValues, comparer);
            }
        }

        /// <summary>
        /// Merges <paramref name="runs"/>.[<paramref name="runIndex"/>] and <paramref name="runs"/>.[<paramref name="runIndex"/> + 1].
        /// </summary>
        private static void MergeAt<T>(
            IList<T>     collection,
            IList<Run>   runs,
            int          runIndex,
            ref int      globalMinGallop,
            T[]          temp,
            IComparer<T> comparer)
        {
            // The index of the first element in [runIndex] that needs to be merged
            var index  = runs[runIndex].Index;
            // The number of elements in [runIndex] that need to be merged
            var count  = runs[runIndex].Count;
            // The index of the first element in [runIndex + 1] that needs to be merged
            var index1 = runs[runIndex + 1].Index;
            // The number of elements in [runIndex + 1] that need to be merged
            var count1 = runs[runIndex + 1].Count;

            /*
             * To merge [runIndex] and [runIndex + 1]
             * store the two as a whole into [runIndex]
             * if [runIndex + 2] exists, store [runIndex + 2] into [runIndex + 1]
             * remove the run at the end
             */
            runs[runIndex] = new Run(runs[runIndex].Index, runs[runIndex].Count + runs[runIndex + 1].Count);
            if (runIndex == runs.Count - 3)
            {
                runs[runIndex + 1] = runs[runIndex + 2];
            }
            /*
             * Remove the run at the end because its data has already been stored in a new place:
             *
             * if it is [runIndex + 1], its data has been stored together with [runIndex] as a whole into [runIndex]
             * if it is [runIndex + 2], it has already been stored into [runIndex + 1]
             */
            runs.RemoveAt(runs.Count - 1);

            // At which index of [runIndex] the first element of [runIndex + 1] should be placed
            var indexOfRun1FirstElementOfRun = GallopRight(collection[index1], collection, index, count, 0, comparer);
            // The elements before that index in [runIndex] can be ignored, so adjust the index and count values
            index += indexOfRun1FirstElementOfRun;
            count -= indexOfRun1FirstElementOfRun;
            // [runIndex] and [runIndex + 1] are already ordered, so no merge is needed
            if (count == 0)
            {
                return;
            }

            // Similarly, find at which index of [runIndex + 1] the last element of [runIndex] should be placed, and adjust the count1 value
            count1 = GallopLeft(collection[index + count - 1], collection, index1, count1, count1 - 1, comparer);
            // [runIndex] and [runIndex + 1] are already ordered, so no merge is needed
            if (count1 == 0)
            {
                return;
            }

            /*
             * Merge the non-ignored parts of [runIndex] and [runIndex + 1]
             * Temporary memory will be used to help with the merge
             * The size of the temporary memory used is Math.Min(count0, count1)
             */
            if (count <= count1)
            {
                MergeLow(collection, index, count, index1, count1, ref globalMinGallop, temp, comparer);
            }
            else
            {
                MergeHigh(collection, index, count, index1, count1, ref globalMinGallop, temp, comparer);
            }
        }

        /// <summary>
        /// Merges <paramref name="runs"/>.[<paramref name="runIndex"/>] and <paramref name="runs"/>.[<paramref name="runIndex"/> + 1].
        /// </summary>
        private static void MergeAt<TKey, TValue>(
            IList<TKey>     keys,
            IList<TValue>   values,
            IList<Run>      runs,
            int             runIndex,
            ref int         globalMinGallop,
            TKey[]          tempKeys,
            TValue[]        tempValues,
            IComparer<TKey> comparer)
        {
            // The index of the first element in [runIndex] that needs to be merged
            var index  = runs[runIndex].Index;
            // The number of elements in [runIndex] that need to be merged
            var count  = runs[runIndex].Count;
            // The index of the first element in [runIndex + 1] that needs to be merged
            var index1 = runs[runIndex + 1].Index;
            // The number of elements in [runIndex + 1] that need to be merged
            var count1 = runs[runIndex + 1].Count;

            /*
             * To merge [runIndex] and [runIndex + 1]
             * store the two as a whole into [runIndex]
             * if [runIndex + 2] exists, store [runIndex + 2] into [runIndex + 1]
             * remove the run at the end
             */
            runs[runIndex] = new Run(runs[runIndex].Index, runs[runIndex].Count + runs[runIndex + 1].Count);
            if (runIndex == runs.Count - 3)
            {
                runs[runIndex + 1] = runs[runIndex + 2];
            }
            /*
             * Remove the run at the end because its data has already been stored in a new place:
             *
             * if it is [runIndex + 1], its data has been stored together with [runIndex] as a whole into [runIndex]
             * if it is [runIndex + 2], it has already been stored into [runIndex + 1]
             */
            runs.RemoveAt(runs.Count - 1);

            // At which index of [runIndex] the first element of [runIndex + 1] should be placed
            var indexOfRun1FirstElementOfRun = GallopRight(keys[index1], keys, index, count, 0, comparer);
            // The elements before that index in [runIndex] can be ignored, so adjust the index and count values
            index += indexOfRun1FirstElementOfRun;
            count -= indexOfRun1FirstElementOfRun;
            // [runIndex] and [runIndex + 1] are already ordered, so no merge is needed
            if (count == 0)
            {
                return;
            }

            // Similarly, find at which index of [runIndex + 1] the last element of [runIndex] should be placed, and adjust the count1 value
            count1 = GallopLeft(keys[index + count - 1], keys, index1, count1, count1 - 1, comparer);
            // [runIndex] and [runIndex + 1] are already ordered, so no merge is needed
            if (count1 == 0)
            {
                return;
            }

            /*
             * Merge the non-ignored parts of [runIndex] and [runIndex + 1]
             * Temporary memory will be used to help with the merge
             * The size of the temporary memory used is Math.Min(count0, count1)
             */
            if (count <= count1)
            {
                MergeLow(
                    keys,
                    values,
                    index,
                    count,
                    index1,
                    count1,
                    ref globalMinGallop,
                    tempKeys,
                    tempValues,
                    comparer
                );
            }
            else
            {
                MergeHigh(
                    keys,
                    values,
                    index,
                    count,
                    index1,
                    count1,
                    ref globalMinGallop,
                    tempKeys,
                    tempValues,
                    comparer
                );
            }
        }

        private static void MergeLow<T>(
            IList<T>     collection,
            int          base1,
            int          len1,
            int          base2,
            int          len2,
            ref int      globalMinGallop,
            T[]          temp,
            IComparer<T> comparer)
        {
            SortHelper.Copy(collection, base1, temp, 0, len1);
            var cursor1 = 0;
            var cursor2 = base2;
            var dest    = base1;
            collection[dest++] = collection[cursor2++];
            if (--len2 == 0)
            {
                SortHelper.Copy(temp, cursor1, collection, dest, len1);
                return;
            }
            if (len1 == 1)
            {
                SortHelper.Copy(collection, cursor2, collection, dest, len2);
                collection[dest + len2] = temp[cursor1];
                return;
            }
            var minGallop = globalMinGallop;
            while (true)
            {
                var count1 = 0;
                var count2 = 0;
                do
                {
                    if (comparer.Compare(collection[cursor2], temp[cursor1]) < 0)
                    {
                        collection[dest++] = collection[cursor2++];
                        count2++;
                        count1 = 0;
                        if (--len2 == 0)
                        {
                            goto EndOfTheWhileStatement;
                        }
                    }
                    else
                    {
                        collection[dest++] = temp[cursor1++];
                        count1++;
                        count2 = 0;
                        if (--len1 == 1)
                        {
                            goto EndOfTheWhileStatement;
                        }
                    }
                } while ((count1 | count2) < minGallop);
                do
                {
                    count1 = GallopRight(collection[cursor2], temp, cursor1, len1, 0, comparer);
                    if (count1 != 0)
                    {
                        SortHelper.Copy(temp, cursor1, collection, dest, count1);
                        dest    += count1;
                        cursor1 += count1;
                        len1    -= count1;
                        if (len1 <= 1)
                        {
                            goto EndOfTheWhileStatement;
                        }
                    }
                    collection[dest++] = collection[cursor2++];
                    if (--len2 == 0)
                    {
                        goto EndOfTheWhileStatement;
                    }
                    count2 = GallopLeft(temp[cursor1], collection, cursor2, len2, 0, comparer);
                    if (count2 != 0)
                    {
                        SortHelper.Copy(collection, cursor2, collection, dest, count2);
                        dest    += count2;
                        cursor2 += count2;
                        len2    -= count2;
                        if (len2 == 0)
                        {
                            goto EndOfTheWhileStatement;
                        }
                    }
                    collection[dest++] = temp[cursor1++];
                    if (--len1 == 1)
                    {
                        goto EndOfTheWhileStatement;
                    }
                    minGallop--;
                } while (count1 >= MinGallop | count2 >= MinGallop);
                if (minGallop < 0)
                {
                    minGallop = 0;
                }
                minGallop += 2;
            }
            EndOfTheWhileStatement:
            globalMinGallop = minGallop < 1 ? 1 : minGallop;
            switch (len1)
            {
                case 0:
                    throw new ArgumentException("Comparer violates its general contract!", nameof(comparer));
                case 1:
                    SortHelper.Copy(collection, cursor2, collection, dest, len2);
                    collection[dest + len2] = temp[cursor1];
                    break;
                default:
                    SortHelper.Copy(temp, cursor1, collection, dest, len1);
                    break;
            }
        }

        private static void MergeLow<TKey, TValue>(
            IList<TKey>     keys,
            IList<TValue>   values,
            int             base1,
            int             len1,
            int             base2,
            int             len2,
            ref int         globalMinGallop,
            TKey[]          tempKeys,
            TValue[]        tempValues,
            IComparer<TKey> comparer)
        {
            SortHelper.Copy(keys,   base1, tempKeys,   0, len1);
            SortHelper.Copy(values, base1, tempValues, 0, len1);
            var cursor1 = 0;
            var cursor2 = base2;
            var dest    = base1;
            keys[dest]   = keys[cursor2];
            values[dest] = values[cursor2];
            dest++;
            cursor2++;
            if (--len2 == 0)
            {
                SortHelper.Copy(tempKeys,   cursor1, keys,   dest, len1);
                SortHelper.Copy(tempValues, cursor1, values, dest, len1);
                return;
            }
            if (len1 == 1)
            {
                SortHelper.Copy(keys,   cursor2, keys,   dest, len2);
                SortHelper.Copy(values, cursor2, values, dest, len2);
                keys[dest + len2]   = tempKeys[cursor1];
                values[dest + len2] = tempValues[cursor1];
                return;
            }
            var minGallop = globalMinGallop;
            while (true)
            {
                var count1 = 0;
                var count2 = 0;
                do
                {
                    if (comparer.Compare(keys[cursor2], tempKeys[cursor1]) < 0)
                    {
                        keys[dest]   = keys[cursor2];
                        values[dest] = values[cursor2];
                        dest++;
                        cursor2++;
                        count2++;
                        count1 = 0;
                        if (--len2 == 0)
                        {
                            goto EndOfTheWhileStatement;
                        }
                    }
                    else
                    {
                        keys[dest]   = tempKeys[cursor1];
                        values[dest] = tempValues[cursor1];
                        dest++;
                        cursor1++;
                        count1++;
                        count2 = 0;
                        if (--len1 == 1)
                        {
                            goto EndOfTheWhileStatement;
                        }
                    }
                } while ((count1 | count2) < minGallop);
                do
                {
                    count1 = GallopRight(keys[cursor2], tempKeys, cursor1, len1, 0, comparer);
                    if (count1 != 0)
                    {
                        SortHelper.Copy(tempKeys,   cursor1, keys,   dest, count1);
                        SortHelper.Copy(tempValues, cursor1, values, dest, count1);
                        dest    += count1;
                        cursor1 += count1;
                        len1    -= count1;
                        if (len1 <= 1)
                        {
                            goto EndOfTheWhileStatement;
                        }
                    }
                    keys[dest]   = keys[cursor2];
                    values[dest] = values[cursor2];
                    dest++;
                    cursor2++;
                    if (--len2 == 0)
                    {
                        goto EndOfTheWhileStatement;
                    }
                    count2 = GallopLeft(tempKeys[cursor1], keys, cursor2, len2, 0, comparer);
                    if (count2 != 0)
                    {
                        SortHelper.Copy(keys,   cursor2, keys,   dest, count2);
                        SortHelper.Copy(values, cursor2, values, dest, count2);
                        dest    += count2;
                        cursor2 += count2;
                        len2    -= count2;
                        if (len2 == 0)
                        {
                            goto EndOfTheWhileStatement;
                        }
                    }
                    keys[dest]   = tempKeys[cursor1];
                    values[dest] = tempValues[cursor1];
                    dest++;
                    cursor1++;
                    if (--len1 == 1)
                    {
                        goto EndOfTheWhileStatement;
                    }
                    minGallop--;
                } while (count1 >= MinGallop | count2 >= MinGallop);
                if (minGallop < 0)
                {
                    minGallop = 0;
                }
                minGallop += 2;
            }
            EndOfTheWhileStatement:
            globalMinGallop = minGallop < 1 ? 1 : minGallop;
            switch (len1)
            {
                case 0:
                    throw new ArgumentException("Comparer violates its general contract!", nameof(comparer));
                case 1:
                    SortHelper.Copy(keys,   cursor2, keys,   dest, len2);
                    SortHelper.Copy(values, cursor2, values, dest, len2);
                    keys[dest + len2]   = tempKeys[cursor1];
                    values[dest + len2] = tempValues[cursor1];
                    break;
                default:
                    SortHelper.Copy(tempKeys,   cursor1, keys,   dest, len1);
                    SortHelper.Copy(tempValues, cursor1, values, dest, len1);
                    break;
            }
        }

        private static void MergeHigh<T>(
            IList<T>     collection,
            int          base1,
            int          len1,
            int          base2,
            int          len2,
            ref int      globalMinGallop,
            T[]          temp,
            IComparer<T> comparer)
        {
            SortHelper.Copy(collection, base2, temp, 0, len2);
            var cursor1 = base1 + len1 - 1;
            var cursor2 = 0 + len2 - 1;
            var dest    = base2 + len2 - 1;
            collection[dest--] = collection[cursor1--];
            if (--len1 == 0)
            {
                SortHelper.Copy(temp, 0, collection, dest - (len2 - 1), len2);
                return;
            }
            if (len2 == 1)
            {
                dest    -= len1;
                cursor1 -= len1;
                SortHelper.Copy(collection, cursor1 + 1, collection, dest + 1, len1);
                collection[dest] = temp[cursor2];
                return;
            }
            var minGallop = globalMinGallop;
            while (true)
            {
                var count1 = 0;
                var count2 = 0;
                do
                {
                    if (comparer.Compare(temp[cursor2], collection[cursor1]) < 0)
                    {
                        collection[dest--] = collection[cursor1--];
                        count1++;
                        count2 = 0;
                        if (--len1 == 0)
                        {
                            goto EndOfTheWhileStatement;
                        }
                    }
                    else
                    {
                        collection[dest--] = temp[cursor2--];
                        count2++;
                        count1 = 0;
                        if (--len2 == 1)
                        {
                            goto EndOfTheWhileStatement;
                        }
                    }
                } while ((count1 | count2) < minGallop);
                do
                {
                    count1 = len1 - GallopRight(temp[cursor2], collection, base1, len1, len1 - 1, comparer);
                    if (count1 != 0)
                    {
                        dest    -= count1;
                        cursor1 -= count1;
                        len1    -= count1;
                        SortHelper.Copy(collection, cursor1 + 1, collection, dest + 1, count1);
                        if (len1 == 0)
                        {
                            goto EndOfTheWhileStatement;
                        }
                    }
                    collection[dest--] = temp[cursor2--];
                    if (--len2 == 1)
                    {
                        goto EndOfTheWhileStatement;
                    }
                    count2 = len2 - GallopLeft(collection[cursor1], temp, 0, len2, len2 - 1, comparer);
                    if (count2 != 0)
                    {
                        dest    -= count2;
                        cursor2 -= count2;
                        len2    -= count2;
                        SortHelper.Copy(temp, cursor2 + 1, collection, dest + 1, count2);
                        if (len2 <= 1)
                        {
                            goto EndOfTheWhileStatement;
                        }
                    }
                    collection[dest--] = collection[cursor1--];
                    if (--len1 == 0)
                    {
                        goto EndOfTheWhileStatement;
                    }
                    minGallop--;
                } while (count1 >= MinGallop | count2 >= MinGallop);
                if (minGallop < 0)
                {
                    minGallop = 0;
                }
                minGallop += 2;
            }
            EndOfTheWhileStatement:
            globalMinGallop = minGallop < 1 ? 1 : minGallop;
            switch (len2)
            {
                case 0:
                    throw new ArgumentException("Comparer violates its general contract!", nameof(comparer));
                case 1:
                    dest    -= len1;
                    cursor1 -= len1;
                    SortHelper.Copy(collection, cursor1 + 1, collection, dest + 1, len1);
                    collection[dest] = temp[cursor2];
                    break;
                default:
                    SortHelper.Copy(temp, 0, collection, dest - (len2 - 1), len2);
                    break;
            }
        }

        private static void MergeHigh<TKey, TValue>(
            IList<TKey>     keys,
            IList<TValue>   values,
            int             base1,
            int             len1,
            int             base2,
            int             len2,
            ref int         globalMinGallop,
            TKey[]          tempKeys,
            TValue[]        tempValues,
            IComparer<TKey> comparer)
        {
            SortHelper.Copy(keys,   base2, tempKeys,   0, len2);
            SortHelper.Copy(values, base2, tempValues, 0, len2);
            var cursor1 = base1 + len1 - 1;
            var cursor2 = 0 + len2 - 1;
            var dest    = base2 + len2 - 1;
            keys[dest]   = keys[cursor1];
            values[dest] = values[cursor1];
            dest--;
            cursor1--;
            if (--len1 == 0)
            {
                SortHelper.Copy(tempKeys,   0, keys,   dest - (len2 - 1), len2);
                SortHelper.Copy(tempValues, 0, values, dest - (len2 - 1), len2);
                return;
            }
            if (len2 == 1)
            {
                dest    -= len1;
                cursor1 -= len1;
                SortHelper.Copy(keys,   cursor1 + 1, keys,   dest + 1, len1);
                SortHelper.Copy(values, cursor1 + 1, values, dest + 1, len1);
                keys[dest]   = tempKeys[cursor2];
                values[dest] = tempValues[cursor2];
                return;
            }
            var minGallop = globalMinGallop;
            while (true)
            {
                var count1 = 0;
                var count2 = 0;
                do
                {
                    if (comparer.Compare(tempKeys[cursor2], keys[cursor1]) < 0)
                    {
                        keys[dest]   = keys[cursor1];
                        values[dest] = values[cursor1];
                        dest--;
                        cursor1--;
                        count1++;
                        count2 = 0;
                        if (--len1 == 0)
                        {
                            goto EndOfTheWhileStatement;
                        }
                    }
                    else
                    {
                        keys[dest]   = tempKeys[cursor2];
                        values[dest] = tempValues[cursor2];
                        dest--;
                        cursor2--;
                        count2++;
                        count1 = 0;
                        if (--len2 == 1)
                        {
                            goto EndOfTheWhileStatement;
                        }
                    }
                } while ((count1 | count2) < minGallop);
                do
                {
                    count1 = len1 - GallopRight(tempKeys[cursor2], keys, base1, len1, len1 - 1, comparer);
                    if (count1 != 0)
                    {
                        dest    -= count1;
                        cursor1 -= count1;
                        len1    -= count1;
                        SortHelper.Copy(keys,   cursor1 + 1, keys,   dest + 1, count1);
                        SortHelper.Copy(values, cursor1 + 1, values, dest + 1, count1);
                        if (len1 == 0)
                        {
                            goto EndOfTheWhileStatement;
                        }
                    }
                    keys[dest]   = tempKeys[cursor2];
                    values[dest] = tempValues[cursor2];
                    dest--;
                    cursor2--;
                    if (--len2 == 1)
                    {
                        goto EndOfTheWhileStatement;
                    }
                    count2 = len2 - GallopLeft(keys[cursor1], tempKeys, 0, len2, len2 - 1, comparer);
                    if (count2 != 0)
                    {
                        dest    -= count2;
                        cursor2 -= count2;
                        len2    -= count2;
                        SortHelper.Copy(tempKeys,   cursor2 + 1, keys,   dest + 1, count2);
                        SortHelper.Copy(tempValues, cursor2 + 1, values, dest + 1, count2);
                        if (len2 <= 1)
                        {
                            goto EndOfTheWhileStatement;
                        }
                    }
                    keys[dest]   = keys[cursor1];
                    values[dest] = values[cursor1];
                    dest--;
                    cursor1--;
                    if (--len1 == 0)
                    {
                        goto EndOfTheWhileStatement;
                    }
                    minGallop--;
                } while (count1 >= MinGallop | count2 >= MinGallop);
                if (minGallop < 0)
                {
                    minGallop = 0;
                }
                minGallop += 2;
            }
            EndOfTheWhileStatement:
            globalMinGallop = minGallop < 1 ? 1 : minGallop;
            switch (len2)
            {
                case 0:
                    throw new ArgumentException("Comparer violates its general contract!", nameof(comparer));
                case 1:
                    dest    -= len1;
                    cursor1 -= len1;
                    SortHelper.Copy(keys,   cursor1 + 1, keys,   dest + 1, len1);
                    SortHelper.Copy(values, cursor1 + 1, values, dest + 1, len1);
                    keys[dest]   = tempKeys[cursor2];
                    values[dest] = tempValues[cursor2];
                    break;
                default:
                    SortHelper.Copy(tempKeys,   0, keys,   dest - (len2 - 1), len2);
                    SortHelper.Copy(tempValues, 0, values, dest - (len2 - 1), len2);
                    break;
            }
        }

        private static int GallopLeft<T>(
            T            key,
            IList<T>     collection,
            int          index,
            int          count,
            int          hint,
            IComparer<T> comparer)
        {
            var offset     = 1;
            var lastOffset = 0;
            if (comparer.Compare(key, collection[index + hint]) > 0)
            {
                var maxOffset = count - hint;
                while (offset < maxOffset && comparer.Compare(key, collection[index + hint + offset]) > 0)
                {
                    lastOffset = offset;
                    offset     = (offset << 1) + 1;
                    // Overflow
                    if (offset <= 0)
                    {
                        offset = maxOffset;
                    }
                }
                if (offset > maxOffset)
                {
                    offset = maxOffset;
                }
                // Make offset a value relative to index
                lastOffset += hint;
                offset     += hint;
            }
            else
            {
                var maxOffset = hint + 1;
                while (offset < maxOffset && comparer.Compare(key, collection[index + hint - offset]) <= 0)
                {
                    lastOffset = offset;
                    offset     = (offset << 1) + 1;
                    // Overflow
                    if (offset <= 0)
                    {
                        offset = maxOffset;
                    }
                }
                if (offset > maxOffset)
                {
                    offset = maxOffset;
                }
                // Make offset a value relative to index
                var tmp = lastOffset;
                lastOffset = hint - offset;
                offset     = hint - tmp;
            }
            lastOffset++;
            while (lastOffset < offset)
            {
                var m = lastOffset + ((offset - lastOffset) >> 1);
                if (comparer.Compare(key, collection[index + m]) > 0)
                {
                    lastOffset = m + 1;
                }
                else
                {
                    offset = m;
                }
            }
            return offset;
        }

        private static int GallopRight<T>(
            T            key,
            IList<T>     collection,
            int          index,
            int          count,
            int          hint,
            IComparer<T> comparer)
        {
            var offset     = 1;
            var lastOffset = 0;
            if (comparer.Compare(key, collection[index + hint]) < 0)
            {
                var maxOffset = hint + 1;
                while (offset < maxOffset && comparer.Compare(key, collection[index + hint - offset]) < 0)
                {
                    lastOffset = offset;
                    offset     = (offset << 1) + 1;
                    // Overflow
                    if (offset <= 0)
                    {
                        offset = maxOffset;
                    }
                }
                if (offset > maxOffset)
                {
                    offset = maxOffset;
                }
                // Make offset a value relative to index
                var tmp = lastOffset;
                lastOffset = hint - offset;
                offset     = hint - tmp;
            }
            else
            {
                var maxOffset = count - hint;
                while (offset < maxOffset && comparer.Compare(key, collection[index + hint + offset]) >= 0)
                {
                    lastOffset = offset;
                    offset     = (offset << 1) + 1;
                    // Overflow
                    if (offset <= 0)
                    {
                        offset = maxOffset;
                    }
                }
                if (offset > maxOffset)
                {
                    offset = maxOffset;
                }
                // Make offset a value relative to index
                lastOffset += hint;
                offset     += hint;
            }
            lastOffset++;
            while (lastOffset < offset)
            {
                var m = lastOffset + ((offset - lastOffset) >> 1);
                if (comparer.Compare(key, collection[index + m]) < 0)
                {
                    offset = m;
                }
                else
                {
                    lastOffset = m + 1;
                }
            }
            return offset;
        }

        private readonly struct Run
        {
            internal readonly int Index;

            internal readonly int Count;

            internal Run(int index, int count)
            {
                Index = index;
                Count = count;
            }
        }
    }
}
