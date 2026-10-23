using System;
using System.Buffers;
using System.Collections.Generic;
using Aurora.Pooling;

namespace Aurora.Sorting
{
    /// <summary>
    /// The Tim Peters sort algorithm. (Not the original TimSort: it is a port of CPython's list sort, using the powersort merge strategy, CPython's improved run detection and its variable minrun scheme.)
    /// </summary>
    internal static class TimSort
    {
        /// <summary>
        /// The largest minimum run length, mirroring CPython's MAX_MINRUN scheme.
        /// </summary>
        /// <remarks>CPython uses 64 because comparisons are very expensive and element moves cheap; here keys go through <see cref="IComparer{T}"/> while the element moves are plain array copies, so 32 measures better on structured inputs.</remarks>
        private const int MaxMinRun = 32;

        /// <summary>
        /// A short input (shorter than <see cref="MaxMinRun"/>) is finished with the plain <see cref="InsertionSort"/> up to this length and with <see cref="BinaryInsertionSort"/> beyond it.
        /// </summary>
        /// <remarks>The crossover between the two depends on how costly a comparison is: it sits near 140 elements for <see cref="int"/> comparisons and only drops to about 8 when a comparison costs tens of times as much, so this threshold targets relatively expensive comparers. Runs grown inside the main loop always use <see cref="BinaryInsertionSort"/>, because a minrun is at least this long anyway.</remarks>
        private const int InsertionSortThreshold = 16;

        private const int MinGallop = 7;

        /// <summary>
        /// Sorts the requested range. The caller is responsible for validating the arguments.
        /// </summary>
        internal static void InternalSort<T>(IList<T> collection, int index, int count, IComparer<T> comparer)
        {
            if (count < MaxMinRun)
            {
                var runCount = CountRunAndMakeAscending(collection, index, count, comparer);
                if (count <= InsertionSortThreshold)
                {
                    InsertionSort.InternalSort(collection, index, count, index + runCount, comparer);
                }
                else
                {
                    BinaryInsertionSort.InternalSort(collection, index, count, index + runCount, comparer);
                }
                return;
            }

            var runs       = PredefinedPools<Run>.List.Get();
            /*
             * MergeLow and MergeHigh need extra space to merge two runs
             * The space is proportional to the shorter of the two run lengths, so in the worst case (the two runs are of equal length and this is the final merge) half of the count length is enough
             * The rented array may be longer than requested; only the first Min(len1, len2) elements are ever touched
             */
            var tempLength = count >> 1;
            var temp       = ArrayPool<T>.Shared.Rent(tempLength);
            try
            {
                var globalMinGallop = MinGallop;
                var baseIndex       = index;
                var totalCount      = count;
                /*
                 * State used to generate the minimum run length of each run
                 * The lengths vary slightly so that the number of runs is a power of two and merges stay as balanced as possible; see listobject.c's minrun_next
                 */
                var minRunShift     = 0;
                while (totalCount >> minRunShift >= MaxMinRun)
                {
                    minRunShift++;
                }
                var minRunMask    = (1 << minRunShift) - 1;
                var minRunCurrent = 0L;
                do
                {
                    minRunCurrent += totalCount;
                    var minRunCount = (int)(minRunCurrent >> minRunShift);
                    minRunCurrent &= minRunMask;
                    var runCount = CountRunAndMakeAscending(collection, index, count, comparer);
                    // The run length is not enough; grow it to minRunCount, but do not exceed the remaining element count
                    if (runCount < minRunCount)
                    {
                        var forceRunCount = Math.Min(minRunCount, count);
                        BinaryInsertionSort.InternalSort(collection, index, forceRunCount, index + runCount, comparer);
                        runCount = forceRunCount;
                    }
                    // Merge the pending runs according to the powersort strategy, then push the new run
                    FoundNewRun(collection, runs, runCount, baseIndex, totalCount, ref globalMinGallop, temp, comparer);
                    runs.Add(new Run(index, runCount, 0));
                    index += runCount;
                    count -= runCount;
                } while (count > 0);
                MergeForceCollapse(collection, runs, ref globalMinGallop, temp, comparer);
            }
            finally
            {
                PredefinedPools<Run>.List.Return(runs);
                ArrayPool<T>.Shared.Return(temp, true);
            }
        }

        /// <summary>
        /// Sorts the requested range of keys and reorders the values to match. The caller is responsible for validating the arguments.
        /// </summary>
        internal static void InternalSort<TKey, TValue>(
            IList<TKey>     keys,
            IList<TValue>   values,
            int             index,
            int             count,
            IComparer<TKey> comparer)
        {
            if (count < MaxMinRun)
            {
                var runCount = CountRunAndMakeAscending(keys, values, index, count, comparer);
                if (count <= InsertionSortThreshold)
                {
                    InsertionSort.InternalSort(keys, values, index, count, index + runCount, comparer);
                }
                else
                {
                    BinaryInsertionSort.InternalSort(keys, values, index, count, index + runCount, comparer);
                }
                return;
            }

            var runs       = PredefinedPools<Run>.List.Get();
            /*
             * MergeLow and MergeHigh need extra space to merge two runs
             * The space is proportional to the shorter of the two run lengths, so in the worst case (the two runs are of equal length and this is the final merge) half of the count length is enough
             * The rented arrays may be longer than requested; only the first Min(len1, len2) elements are ever touched
             */
            var tempLength = count >> 1;
            var tempKeys   = ArrayPool<TKey>.Shared.Rent(tempLength);
            var tempValues = ArrayPool<TValue>.Shared.Rent(tempLength);
            try
            {
                var globalMinGallop = MinGallop;
                var baseIndex       = index;
                var totalCount      = count;
                /*
                 * State used to generate the minimum run length of each run
                 * The lengths vary slightly so that the number of runs is a power of two and merges stay as balanced as possible; see listobject.c's minrun_next
                 */
                var minRunShift     = 0;
                while (totalCount >> minRunShift >= MaxMinRun)
                {
                    minRunShift++;
                }
                var minRunMask    = (1 << minRunShift) - 1;
                var minRunCurrent = 0L;
                do
                {
                    minRunCurrent += totalCount;
                    var minRunCount = (int)(minRunCurrent >> minRunShift);
                    minRunCurrent &= minRunMask;
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
                    // Merge the pending runs according to the powersort strategy, then push the new run
                    FoundNewRun(
                        keys,
                        values,
                        runs,
                        runCount,
                        baseIndex,
                        totalCount,
                        ref globalMinGallop,
                        tempKeys,
                        tempValues,
                        comparer
                    );
                    runs.Add(new Run(index, runCount, 0));
                    index += runCount;
                    count -= runCount;
                } while (count > 0);
                MergeForceCollapse(keys, values, runs, ref globalMinGallop, tempKeys, tempValues, comparer);
            }
            finally
            {
                PredefinedPools<Run>.List.Return(runs);
                ArrayPool<TKey>.Shared.Return(tempKeys, true);
                ArrayPool<TValue>.Shared.Return(tempValues, true);
            }
        }

        /// <summary>
        /// Returns the length of the run beginning at <paramref name="index"/>, reversing it in place if it is descending.
        /// </summary>
        /// <remarks>
        /// Ported from CPython's count_run: a descending run may contain sub-runs of equal elements, which are reversed on the fly so that the final reversal preserves their original order. The reversed run is then extended by a naturally ascending suffix.
        /// </remarks>
        private static int CountRunAndMakeAscending<T>(IList<T> collection, int index, int count, IComparer<T> comparer)
        {
            if (count < 2)
            {
                return count;
            }
            // Find the ascending prefix; it ends at the first strict descent
            var n = 1;
            while (n < count && comparer.Compare(collection[index + n], collection[index + n - 1]) >= 0)
            {
                n++;
            }
            if (n == count)
            {
                return n;
            }
            /*
             * collection[index + n] is strictly less than collection[index + n - 1], so the sequence is descending from here on
             * If the ascending prefix contains a strict increase, it is already a valid run
             */
            if (n > 1)
            {
                if (comparer.Compare(collection[index], collection[index + n - 1]) < 0)
                {
                    return n;
                }
                /*
                 * The ascending prefix consists of equal elements only, so this is a descending run leading with a run of equal elements
                 * Reverse the prefix in place; the final whole-run reversal restores the original order of those equal elements
                 */
                ReverseRange(collection, index, n);
            }
            n++;
            /*
             * Finish the strictly descending run
             * Runs of equal elements are reversed in place as they end, so that the final whole-run reversal restores their original order
             */
            var equalCount = 0;
            while (n < count)
            {
                if (comparer.Compare(collection[index + n], collection[index + n - 1]) < 0)
                {
                    ReverseLastEqualElements(collection, index, n, ref equalCount);
                }
                else if (comparer.Compare(collection[index + n - 1], collection[index + n]) < 0)
                {
                    break;
                }
                else
                {
                    equalCount++;
                }
                n++;
            }
            ReverseLastEqualElements(collection, index, n, ref equalCount);
            // Reverse the strictly descending run to make it ascending; the reversal above keeps equal elements stable
            ReverseRange(collection, index, n);
            /*
             * The reversed run can be extended by a naturally ascending suffix
             * e.g. [3, 2, 3, 4, 1] makes an ascending run from the first 4 elements
             */
            while (n < count && comparer.Compare(collection[index + n], collection[index + n - 1]) >= 0)
            {
                n++;
            }
            return n;
        }

        /// <summary>
        /// Returns the length of the run beginning at <paramref name="index"/>, reversing it in place if it is descending.
        /// </summary>
        /// <remarks>
        /// Ported from CPython's count_run: a descending run may contain sub-runs of equal elements, which are reversed on the fly so that the final reversal preserves their original order. The reversed run is then extended by a naturally ascending suffix.
        /// </remarks>
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
            // Find the ascending prefix; it ends at the first strict descent
            var n = 1;
            while (n < count && comparer.Compare(keys[index + n], keys[index + n - 1]) >= 0)
            {
                n++;
            }
            if (n == count)
            {
                return n;
            }
            /*
             * keys[index + n] is strictly less than keys[index + n - 1], so the sequence is descending from here on
             * If the ascending prefix contains a strict increase, it is already a valid run
             */
            if (n > 1)
            {
                if (comparer.Compare(keys[index], keys[index + n - 1]) < 0)
                {
                    return n;
                }
                /*
                 * The ascending prefix consists of equal elements only, so this is a descending run leading with a run of equal elements
                 * Reverse the prefix in place; the final whole-run reversal restores the original order of those equal elements
                 */
                ReverseRange(keys, values, index, n);
            }
            n++;
            /*
             * Finish the strictly descending run
             * Runs of equal elements are reversed in place as they end, so that the final whole-run reversal restores their original order
             */
            var equalCount = 0;
            while (n < count)
            {
                if (comparer.Compare(keys[index + n], keys[index + n - 1]) < 0)
                {
                    ReverseLastEqualElements(keys, values, index, n, ref equalCount);
                }
                else if (comparer.Compare(keys[index + n - 1], keys[index + n]) < 0)
                {
                    break;
                }
                else
                {
                    equalCount++;
                }
                n++;
            }
            ReverseLastEqualElements(keys, values, index, n, ref equalCount);
            // Reverse the strictly descending run to make it ascending; the reversal above keeps equal elements stable
            ReverseRange(keys, values, index, n);
            /*
             * The reversed run can be extended by a naturally ascending suffix
             * e.g. [3, 2, 3, 4, 1] makes an ascending run from the first 4 elements
             */
            while (n < count && comparer.Compare(keys[index + n], keys[index + n - 1]) >= 0)
            {
                n++;
            }
            return n;
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

        /// <summary>
        /// Reverses the last <paramref name="equalCount"/> + 1 equal elements and resets <paramref name="equalCount"/>.
        /// </summary>
        private static void ReverseLastEqualElements<T>(IList<T> collection, int index, int n, ref int equalCount)
        {
            if (equalCount == 0)
            {
                return;
            }
            ReverseRange(collection, index + n - (equalCount + 1), equalCount + 1);
            equalCount = 0;
        }

        /// <summary>
        /// Reverses the last <paramref name="equalCount"/> + 1 equal elements and resets <paramref name="equalCount"/>.
        /// </summary>
        private static void ReverseLastEqualElements<TKey, TValue>(
            IList<TKey>   keys,
            IList<TValue> values,
            int           index,
            int           n,
            ref int       equalCount)
        {
            if (equalCount == 0)
            {
                return;
            }
            ReverseRange(keys, values, index + n - (equalCount + 1), equalCount + 1);
            equalCount = 0;
        }

        /// <summary>
        /// Applies the powersort merge strategy: merges the runs on the stack whose power is greater than the power of the boundary between the top run and <paramref name="newRunCount"/>.
        /// </summary>
        private static void FoundNewRun<T>(
            IList<T>     collection,
            IList<Run>   runs,
            int          newRunCount,
            int          baseIndex,
            int          totalCount,
            ref int      globalMinGallop,
            T[]          temp,
            IComparer<T> comparer)
        {
            if (runs.Count == 0)
            {
                return;
            }
            var topRun = runs[^1];
            var power  = GetPower(topRun.Index - baseIndex, topRun.Count, newRunCount, totalCount);
            while (runs.Count > 1 && runs[^2].Power > power)
            {
                MergeAt(collection, runs, runs.Count - 2, ref globalMinGallop, temp, comparer);
            }
            topRun   = runs[^1];
            runs[^1] = new Run(topRun.Index, topRun.Count, power);
        }

        /// <summary>
        /// Computes the powersort power of the boundary between two adjacent runs, that is its depth in the conceptual binary merge tree.
        /// </summary>
        private static int GetPower(int startIndex, int count1, int count2, int count)
        {
            var result = 0;
            var a      = 2L * startIndex + count1;
            var b      = a + count1 + count2;
            while (true)
            {
                result++;
                if (a >= count)
                {
                    a -= count;
                    b -= count;
                }
                else if (b >= count)
                {
                    break;
                }
                a <<= 1;
                b <<= 1;
            }
            return result;
        }

        /// <summary>
        /// Applies the powersort merge strategy: merges the runs on the stack whose power is greater than the power of the boundary between the top run and <paramref name="newRunCount"/>.
        /// </summary>
        private static void FoundNewRun<TKey, TValue>(
            IList<TKey>     keys,
            IList<TValue>   values,
            IList<Run>      runs,
            int             newRunCount,
            int             baseIndex,
            int             totalCount,
            ref int         globalMinGallop,
            TKey[]          tempKeys,
            TValue[]        tempValues,
            IComparer<TKey> comparer)
        {
            if (runs.Count == 0)
            {
                return;
            }
            var topRun = runs[^1];
            var power  = GetPower(topRun.Index - baseIndex, topRun.Count, newRunCount, totalCount);
            while (runs.Count > 1 && runs[^2].Power > power)
            {
                MergeAt(keys, values, runs, runs.Count - 2, ref globalMinGallop, tempKeys, tempValues, comparer);
            }
            topRun   = runs[^1];
            runs[^1] = new Run(topRun.Index, topRun.Count, power);
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
            runs[runIndex] = new Run(
                runs[runIndex].Index,
                runs[runIndex].Count + runs[runIndex + 1].Count,
                runs[runIndex].Power
            );
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
            runs[runIndex] = new Run(
                runs[runIndex].Index,
                runs[runIndex].Count + runs[runIndex + 1].Count,
                runs[runIndex].Power
            );
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
                minGallop++;
                do
                {
                    if (minGallop > 1)
                    {
                        minGallop--;
                    }
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
                } while (count1 >= MinGallop | count2 >= MinGallop);
                minGallop++;
            }
            EndOfTheWhileStatement:
            globalMinGallop = minGallop;
            switch (len1)
            {
                case 0:
                    throw new ArgumentException(
                        $"Unable to sort because the IComparer.Compare() method returns inconsistent results. Either a value does not compare equal to itself, or one value repeatedly compared to another value yields different results. IComparer: '{comparer}'."
                    );
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
                minGallop++;
                do
                {
                    if (minGallop > 1)
                    {
                        minGallop--;
                    }
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
                } while (count1 >= MinGallop | count2 >= MinGallop);
                minGallop++;
            }
            EndOfTheWhileStatement:
            globalMinGallop = minGallop;
            switch (len1)
            {
                case 0:
                    throw new ArgumentException(
                        $"Unable to sort because the IComparer.Compare() method returns inconsistent results. Either a value does not compare equal to itself, or one value repeatedly compared to another value yields different results. IComparer: '{comparer}'."
                    );
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
                minGallop++;
                do
                {
                    if (minGallop > 1)
                    {
                        minGallop--;
                    }
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
                } while (count1 >= MinGallop | count2 >= MinGallop);
                minGallop++;
            }
            EndOfTheWhileStatement:
            globalMinGallop = minGallop;
            switch (len2)
            {
                case 0:
                    throw new ArgumentException(
                        $"Unable to sort because the IComparer.Compare() method returns inconsistent results. Either a value does not compare equal to itself, or one value repeatedly compared to another value yields different results. IComparer: '{comparer}'."
                    );
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
                minGallop++;
                do
                {
                    if (minGallop > 1)
                    {
                        minGallop--;
                    }
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
                } while (count1 >= MinGallop | count2 >= MinGallop);
                minGallop++;
            }
            EndOfTheWhileStatement:
            globalMinGallop = minGallop;
            switch (len2)
            {
                case 0:
                    throw new ArgumentException(
                        $"Unable to sort because the IComparer.Compare() method returns inconsistent results. Either a value does not compare equal to itself, or one value repeatedly compared to another value yields different results. IComparer: '{comparer}'."
                    );
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

            /// <summary>
            /// The powersort power of the boundary between this run and the run above it on the run stack.
            /// </summary>
            internal readonly int Power;

            internal Run(int index, int count, int power)
            {
                Index = index;
                Count = count;
                Power = power;
            }
        }
    }
}
