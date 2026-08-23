using System;
using System.Collections.Generic;
using Aurora.Pooling;

namespace Aurora.Sorting
{
    /// <summary>
    /// 快速排序算法。
    /// </summary>
    public static class QuickSort
    {
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
            var sections = PredefinedPools<Section>.Stack.Get();
            try
            {
                sections.Push(new Section(index, index + count - 1));
                do
                {
                    var section = sections.Pop();
                    InternalSort(sections, collection, section, comparer);
                } while (sections.Count > 0);
            }
            finally
            {
                PredefinedPools<Section>.Stack.Return(sections);
            }
        }

        private static void InternalSort<TKey, TValue>(
            IList<TKey>     keys,
            IList<TValue>   values,
            int             index,
            int             count,
            IComparer<TKey> comparer)
        {
            var sections = PredefinedPools<Section>.Stack.Get();
            try
            {
                sections.Push(new Section(index, index + count - 1));
                do
                {
                    var section = sections.Pop();
                    InternalSort(sections, keys, values, section, comparer);
                } while (sections.Count > 0);
            }
            finally
            {
                PredefinedPools<Section>.Stack.Return(sections);
            }
        }

        private static void InternalSort<T>(
            Stack<Section> sections,
            IList<T>       collection,
            Section        section,
            IComparer<T>   comparer)
        {
            var left  = section.Left;
            var right = section.Right;
            var key   = collection[left];
            var l     = left;
            var r     = right;
            while (l < r)
            {
                while (l < r && comparer.Compare(collection[r], key) >= 0)
                {
                    r--;
                }
                if (l < r)
                {
                    collection[l++] = collection[r];
                }
                while (l < r && comparer.Compare(collection[l], key) <= 0)
                {
                    l++;
                }
                if (l < r)
                {
                    collection[r--] = collection[l];
                }
            }
            collection[l] = key;
            if (left < l - 1)
            {
                sections.Push(new Section(left, l - 1));
            }
            if (l + 1 < right)
            {
                sections.Push(new Section(l + 1, right));
            }
        }

        private static void InternalSort<TKey, TValue>(
            Stack<Section>  sections,
            IList<TKey>     keys,
            IList<TValue>   values,
            Section         section,
            IComparer<TKey> comparer)
        {
            var left  = section.Left;
            var right = section.Right;
            var key   = keys[left];
            var value = values[left];
            var l     = left;
            var r     = right;
            while (l < r)
            {
                while (l < r && comparer.Compare(keys[r], key) >= 0)
                {
                    r--;
                }
                if (l < r)
                {
                    keys[l]   = keys[r];
                    values[l] = values[r];
                    l++;
                }
                while (l < r && comparer.Compare(keys[l], key) <= 0)
                {
                    l++;
                }
                if (l < r)
                {
                    keys[r]   = keys[l];
                    values[r] = values[l];
                    r--;
                }
            }
            keys[l]   = key;
            values[l] = value;
            if (left < l - 1)
            {
                sections.Push(new Section(left, l - 1));
            }
            if (l + 1 < right)
            {
                sections.Push(new Section(l + 1, right));
            }
        }

        private readonly struct Section
        {
            internal readonly int Left;

            internal readonly int Right;

            internal Section(int left, int right)
            {
                Left  = left;
                Right = right;
            }
        }
    }
}
