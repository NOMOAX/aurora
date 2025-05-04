using System;
using System.Collections.Generic;
using Aurora.Pooling;

namespace Aurora.Sorting
{
    /// <summary>
    /// Tim Peters 排序算法。
    /// </summary>
    public static class TimSort
    {
        private const int MinMerge = 32;

        private const int MinGallop = 7;

        /// <summary>
        /// 对集合中的元素进行排序。
        /// </summary>
        /// <param name="collection">集合。</param>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <see langword="null"/>。</exception>
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
        /// 使用指定的比较器，对集合中的元素进行排序。
        /// </summary>
        /// <param name="collection">集合。</param>
        /// <param name="comparer">比较元素时使用的比较器。如果为 <see langword="null"/>，则使用默认的比较器（<see cref="Comparer{T}.Default"/>）。</param>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <see langword="null"/>。</exception>
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
        /// 使用指定的比较器，对集合中的部分元素进行排序。
        /// </summary>
        /// <param name="collection">集合。</param>
        /// <param name="index">排序范围的起始索引。</param>
        /// <param name="count">排序范围内的元素数。</param>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 0，或者 <paramref name="count"/> 小于 0。</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> 加上 <paramref name="count"/> 大于 <paramref name="collection"/> 中包含的元素数。</exception>
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
                throw new ArgumentException($"{nameof(index)} 加上 {nameof(count)} 大于 {nameof(collection)} 中包含的元素数");
            }
            if (count < 2)
            {
                return;
            }
            InternalSort(collection, index, count, Comparer<T>.Default);
        }

        /// <summary>
        /// 使用指定的比较器，对集合中的部分元素进行排序。
        /// </summary>
        /// <param name="collection">集合。</param>
        /// <param name="index">排序范围的起始索引。</param>
        /// <param name="count">排序范围内的元素数。</param>
        /// <param name="comparer">比较元素时使用的比较器。如果为 <see langword="null"/>，则使用默认的比较器（<see cref="Comparer{T}.Default"/>）。</param>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 0，或者 <paramref name="count"/> 小于 0。</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> 加上 <paramref name="count"/> 大于 <paramref name="collection"/> 中包含的元素数。</exception>
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
                throw new ArgumentException($"{nameof(index)} 加上 {nameof(count)} 大于 {nameof(collection)} 中包含的元素数");
            }
            if (count < 2)
            {
                return;
            }
            InternalSort(collection, index, count, comparer ?? Comparer<T>.Default);
        }

        /// <summary>
        /// 使用指定的排序器。对键集合中的元素进行排序，并对应地修改值集合中的元素的顺序。
        /// </summary>
        /// <param name="keys">键集合。</param>
        /// <param name="values">值集合。</param>
        /// <typeparam name="TKey">键集合中元素的类型。</typeparam>
        /// <typeparam name="TValue">值集合中元素的类型。</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentException"><paramref name="values"/> 不为 <see langword="null"/>，并且 <paramref name="values"/> 中包含的元素数小于 <paramref name="keys"/> 中包含的元素数。</exception>
        public static void Sort<TKey, TValue>(IList<TKey> keys, IList<TValue> values)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            var count = keys.Count;
            if (values != null && values.Count < count)
            {
                throw new ArgumentException($"{nameof(values)} 中包含的元素数小于 {nameof(keys)} 中包含的元素数", nameof(values));
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
        /// 使用指定的排序器。对键集合中的元素进行排序，并对应地修改值集合中的元素的顺序。
        /// </summary>
        /// <param name="keys">键集合。</param>
        /// <param name="values">值集合。</param>
        /// <param name="comparer">比较元素时使用的比较器。如果为 <see langword="null"/>，则使用默认的比较器（<see cref="Comparer{T}.Default"/>）。</param>
        /// <typeparam name="TKey">键集合中元素的类型。</typeparam>
        /// <typeparam name="TValue">值集合中元素的类型。</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentException"><paramref name="values"/> 不为 <see langword="null"/>，并且 <paramref name="values"/> 中包含的元素数小于 <paramref name="keys"/> 中包含的元素数。</exception>
        public static void Sort<TKey, TValue>(IList<TKey> keys, IList<TValue> values, IComparer<TKey> comparer)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            var count = keys.Count;
            if (values != null && values.Count < count)
            {
                throw new ArgumentException($"{nameof(values)} 中包含的元素数小于 {nameof(keys)} 中包含的元素数", nameof(values));
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
        /// 使用指定的排序器。对键集合中的部分元素进行排序，并对应地修改值集合中的元素的顺序。
        /// </summary>
        /// <param name="keys">键集合。</param>
        /// <param name="values">值集合。</param>
        /// <param name="index">排序范围的起始索引。</param>
        /// <param name="count">排序范围内的元素数。</param>
        /// <typeparam name="TKey">键集合中元素的类型。</typeparam>
        /// <typeparam name="TValue">值集合中元素的类型。</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 0，或者 <paramref name="count"/> 小于 0。</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> 加上 <paramref name="count"/> 大于 <paramref name="keys"/> 中包含的元素数，或者 <paramref name="values"/> 不为 <see langword="null"/> 并且 <paramref name="index"/> 加上 <paramref name="count"/> 大于 <paramref name="values"/> 中包含的元素数。</exception>
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
                throw new ArgumentException($"{nameof(index)} 加上 {nameof(count)} 大于 {nameof(keys)} 中包含的元素数");
            }
            if (values != null && index + count > values.Count)
            {
                throw new ArgumentException($"{nameof(index)} 加上 {nameof(count)} 大于 {nameof(values)} 中包含的元素数");
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
        /// 使用指定的排序器。对键集合中的部分元素进行排序，并对应地修改值集合中的元素的顺序。
        /// </summary>
        /// <param name="keys">键集合。</param>
        /// <param name="values">值集合。</param>
        /// <param name="index">排序范围的起始索引。</param>
        /// <param name="count">排序范围内的元素数。</param>
        /// <param name="comparer">比较元素时使用的比较器。如果为 <see langword="null"/>，则使用默认的比较器（<see cref="Comparer{T}.Default"/>）。</param>
        /// <typeparam name="TKey">键集合中元素的类型。</typeparam>
        /// <typeparam name="TValue">值集合中元素的类型。</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 0，或者 <paramref name="count"/> 小于 0。</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> 加上 <paramref name="count"/> 大于 <paramref name="keys"/> 中包含的元素数，或者 <paramref name="values"/> 不为 <see langword="null"/> 并且 <paramref name="index"/> 加上 <paramref name="count"/> 大于 <paramref name="values"/> 中包含的元素数。</exception>
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
                throw new ArgumentException($"{nameof(index)} 加上 {nameof(count)} 大于 {nameof(keys)} 中包含的元素数");
            }
            if (values != null && index + count > values.Count)
            {
                throw new ArgumentException($"{nameof(index)} 加上 {nameof(count)} 大于 {nameof(values)} 中包含的元素数");
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
                 * MergeLow 和 MergeHigh 需要使用额外的空间来合并两个 run
                 * 将使用两个 run 的长度的较短者
                 * 最坏情况下，两个 run 的长度相等，并且是最后一轮合并，因此这里直接初始化一个长度为 count 的一半的数组，作为额外空间
                 * 如果使用更小的值，并在合适的时候扩容，也是可以的，
                 * 但是，从栈顶到栈底的 run 的长度变化大概是以斐波那契数列的程度增长的，需要考虑多次扩容带来的性能影响
                 */
                var temp = new T[count >> 1];
                // 最小的 run 长度
                var minRunCount = GetMinRunCount(count);
                do
                {
                    var runCount = CountRunAndMakeAscending(collection, index, count, comparer);
                    // run 的长度不足，扩充到 minRunCount，但不要超过剩余的元素数量
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
                 * MergeLow 和 MergeHigh 需要使用额外的空间来合并两个 run
                 * 将使用两个 run 的长度的较短者
                 * 最坏情况下，两个 run 的长度相等，并且是最后一轮合并，因此这里直接初始化一个长度为 count 的一半的数组，作为额外空间
                 * 如果使用更小的值，并在合适的时候扩容，也是可以的，
                 * 但是，从栈顶到栈底的 run 的长度变化大概是以斐波那契数列的程度增长的，需要考虑多次扩容带来的性能影响
                 */
                var tempKeys   = new TKey[count >> 1];
                var tempValues = new TValue[count >> 1];
                // 最小的 run 长度
                var minRunCount = GetMinRunCount(count);
                do
                {
                    var runCount = CountRunAndMakeAscending(keys, values, index, count, comparer);
                    // run 的长度不足，扩充到 minRunCount，但不要超过剩余的元素数量
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
            // 比较前两个元素，判断是升序序列还是严格降序序列
            var isAscending = comparer.Compare(collection[index], collection[index + 1]) <= 0;
            // 将指针指向第三个元素，此后，每轮比较指针之前一个元素和指针所在的元素
            var i = index + 2;
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
                 * 将严格降序序列反转，使其成为严格升序序列
                 * 由于严格降序序列中不存在相等的元素，因此该操作不会破坏排序的稳定性
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
            // 比较前两个元素，判断是升序序列还是严格降序序列
            var isAscending = comparer.Compare(keys[index], keys[index + 1]) <= 0;
            // 将指针指向第三个元素，此后，每轮比较指针之前一个元素和指针所在的元素
            var i = index + 2;
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
                 * 将严格降序序列反转，使其成为严格升序序列
                 * 由于严格降序序列中不存在相等的元素，因此该操作不会破坏排序的稳定性
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
        /// 合并 <paramref name="runs"/>.[<paramref name="runIndex"/>] 和 <paramref name="runs"/>.[<paramref name="runIndex"/> + 1]。
        /// </summary>
        private static void MergeAt<T>(
            IList<T>     collection,
            IList<Run>   runs,
            int          runIndex,
            ref int      globalMinGallop,
            T[]          temp,
            IComparer<T> comparer)
        {
            // [runIndex] 中需要参与合并的第一个元素的索引
            var index = runs[runIndex].Index;
            // [runIndex] 中需要参与合并的元素数量
            var count = runs[runIndex].Count;
            // [runIndex + 1] 中需要参与合并的第一个元素的索引
            var index1 = runs[runIndex + 1].Index;
            // [runIndex + 1] 中需要参与合并的元素数量
            var count1 = runs[runIndex + 1].Count;

            /*
             * 要合并 [runIndex] 和 [runIndex + 1]
             * 将他们两个作为一个整体存到 [runIndex]
             * 如果 [runIndex + 2] 存在的话，将 [runIndex + 2] 存到 [runIndex + 1]
             * 移除末尾处的 run
             */
            runs[runIndex] = new Run(runs[runIndex].Index, runs[runIndex].Count + runs[runIndex + 1].Count);
            if (runIndex == runs.Count - 3)
            {
                runs[runIndex + 1] = runs[runIndex + 2];
            }
            /*
             * 移除末尾处的 run，因为它的数据已经被存放在新的地方：
             *
             * 如果它是 [runIndex + 1]，那么它的数据已经和 [runIndex] 一起作为整体存放在 [runIndex]
             * 如果它是 [runIndex + 2]，那么它已经存放在 [runIndex + 1]
             */
            runs.RemoveAt(runs.Count - 1);

            // [runIndex + 1] 的第一个元素应该放到 [runIndex] 的什么索引处
            var indexOfRun1FirstElementOfRun = GallopRight(collection[index1], collection, index, count, 0, comparer);
            // [runIndex] 中该索引处之前的元素可以被忽略，因此调整 index 和 count 的值
            index += indexOfRun1FirstElementOfRun;
            count -= indexOfRun1FirstElementOfRun;
            // [runIndex] 与 [runIndex + 1] 已经有序，无需合并
            if (count == 0)
            {
                return;
            }

            // 类似的，寻找 [runIndex] 的最后一个元素应该放到 [runIndex + 1] 的什么索引处，并调整 count1 的值
            count1 = GallopLeft(collection[index + count - 1], collection, index1, count1, count1 - 1, comparer);
            // [runIndex] 与 [runIndex + 1] 已经有序，无需合并
            if (count1 == 0)
            {
                return;
            }

            /*
             * 合并 [runIndex] 和 [runIndex + 1] 中未被忽略的部分
             * 将使用临时内存空间来帮助合并
             * 使用的临时内存空间的大小为 Math.Min(count0, count1)
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
        /// 合并 <paramref name="runs"/>.[<paramref name="runIndex"/>] 和 <paramref name="runs"/>.[<paramref name="runIndex"/> + 1]。
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
            // [runIndex] 中需要参与合并的第一个元素的索引
            var index = runs[runIndex].Index;
            // [runIndex] 中需要参与合并的元素数量
            var count = runs[runIndex].Count;
            // [runIndex + 1] 中需要参与合并的第一个元素的索引
            var index1 = runs[runIndex + 1].Index;
            // [runIndex + 1] 中需要参与合并的元素数量
            var count1 = runs[runIndex + 1].Count;

            /*
             * 要合并 [runIndex] 和 [runIndex + 1]
             * 将他们两个作为一个整体存到 [runIndex]
             * 如果 [runIndex + 2] 存在的话，将 [runIndex + 2] 存到 [runIndex + 1]
             * 移除末尾处的 run
             */
            runs[runIndex] = new Run(runs[runIndex].Index, runs[runIndex].Count + runs[runIndex + 1].Count);
            if (runIndex == runs.Count - 3)
            {
                runs[runIndex + 1] = runs[runIndex + 2];
            }
            /*
             * 移除末尾处的 run，因为它的数据已经被存放在新的地方：
             *
             * 如果它是 [runIndex + 1]，那么它的数据已经和 [runIndex] 一起作为整体存放在 [runIndex]
             * 如果它是 [runIndex + 2]，那么它已经存放在 [runIndex + 1]
             */
            runs.RemoveAt(runs.Count - 1);

            // [runIndex + 1] 的第一个元素应该放到 [runIndex] 的什么索引处
            var indexOfRun1FirstElementOfRun = GallopRight(keys[index1], keys, index, count, 0, comparer);
            // [runIndex] 中该索引处之前的元素可以被忽略，因此调整 index 和 count 的值
            index += indexOfRun1FirstElementOfRun;
            count -= indexOfRun1FirstElementOfRun;
            // [runIndex] 与 [runIndex + 1] 已经有序，无需合并
            if (count == 0)
            {
                return;
            }

            // 类似的，寻找 [runIndex] 的最后一个元素应该放到 [runIndex + 1] 的什么索引处，并调整 count1 的值
            count1 = GallopLeft(keys[index + count - 1], keys, index1, count1, count1 - 1, comparer);
            // [runIndex] 与 [runIndex + 1] 已经有序，无需合并
            if (count1 == 0)
            {
                return;
            }

            /*
             * 合并 [runIndex] 和 [runIndex + 1] 中未被忽略的部分
             * 将使用临时内存空间来帮助合并
             * 使用的临时内存空间的大小为 Math.Min(count0, count1)
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
                    // 溢出
                    if (offset <= 0)
                    {
                        offset = maxOffset;
                    }
                }
                if (offset > maxOffset)
                {
                    offset = maxOffset;
                }
                // 让 offset 变成相对于 index 的值
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
                    // 溢出
                    if (offset <= 0)
                    {
                        offset = maxOffset;
                    }
                }
                if (offset > maxOffset)
                {
                    offset = maxOffset;
                }
                // 让 offset 变成相对于 index 的值
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
                    // 溢出
                    if (offset <= 0)
                    {
                        offset = maxOffset;
                    }
                }
                if (offset > maxOffset)
                {
                    offset = maxOffset;
                }
                // 让 offset 变成相对于 index 的值
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
                    // 溢出
                    if (offset <= 0)
                    {
                        offset = maxOffset;
                    }
                }
                if (offset > maxOffset)
                {
                    offset = maxOffset;
                }
                // 让 offset 变成相对于 index 的值
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
