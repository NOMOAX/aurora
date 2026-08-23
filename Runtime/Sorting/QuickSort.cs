using System;
using System.Collections.Generic;
using Aurora.Pooling;

namespace Aurora.Sorting
{
    /// <summary>
    /// A quick-sort algorithm.
    /// </summary>
    public static class QuickSort
    {
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
