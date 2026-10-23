using System;
using System.Collections.Generic;

namespace Aurora.Sorting
{
    /// <summary>
    /// The sorting entry points.
    /// </summary>
    public static class Sorts
    {
        /// <summary>
        /// Sorts the elements in the collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> is read-only.</exception>
        public static void Sort<T>(IList<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (collection.IsReadOnly)
            {
                throw new ArgumentException($"{nameof(collection)} is read-only", nameof(collection));
            }
            var count = collection.Count;
            if (count < 2)
            {
                return;
            }
            InternalSort(collection, 0, count, Comparer<T>.Default);
        }

        /// <summary>
        /// Sorts the elements in the collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> is read-only.</exception>
        public static void StableSort<T>(IList<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (collection.IsReadOnly)
            {
                throw new ArgumentException($"{nameof(collection)} is read-only", nameof(collection));
            }
            var count = collection.Count;
            if (count < 2)
            {
                return;
            }
            InternalStableSort(collection, 0, count, Comparer<T>.Default);
        }

        /// <summary>
        /// Sorts the elements in the collection using the specified comparer.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="comparer">The comparer used to compare elements. If it is <see langword="null"/>, the default comparer (<see cref="Comparer{T}.Default"/>) is used.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> is read-only.</exception>
        public static void Sort<T>(IList<T> collection, IComparer<T> comparer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (collection.IsReadOnly)
            {
                throw new ArgumentException($"{nameof(collection)} is read-only", nameof(collection));
            }
            var count = collection.Count;
            if (count < 2)
            {
                return;
            }
            InternalSort(collection, 0, count, comparer ?? Comparer<T>.Default);
        }

        /// <summary>
        /// Sorts the elements in the collection using the specified comparer.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="comparer">The comparer used to compare elements. If it is <see langword="null"/>, the default comparer (<see cref="Comparer{T}.Default"/>) is used.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> is read-only.</exception>
        public static void StableSort<T>(IList<T> collection, IComparer<T> comparer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (collection.IsReadOnly)
            {
                throw new ArgumentException($"{nameof(collection)} is read-only", nameof(collection));
            }
            var count = collection.Count;
            if (count < 2)
            {
                return;
            }
            InternalStableSort(collection, 0, count, comparer ?? Comparer<T>.Default);
        }

        /// <summary>
        /// Sorts part of the elements in the collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="index">The starting index of the sort range.</param>
        /// <param name="count">The number of elements in the sort range.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> is read-only, or <paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="collection"/>.</exception>
        public static void Sort<T>(IList<T> collection, int index, int count)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (collection.IsReadOnly)
            {
                throw new ArgumentException($"{nameof(collection)} is read-only", nameof(collection));
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
        /// Sorts part of the elements in the collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="index">The starting index of the sort range.</param>
        /// <param name="count">The number of elements in the sort range.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> is read-only, or <paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="collection"/>.</exception>
        public static void StableSort<T>(IList<T> collection, int index, int count)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (collection.IsReadOnly)
            {
                throw new ArgumentException($"{nameof(collection)} is read-only", nameof(collection));
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
            InternalStableSort(collection, index, count, Comparer<T>.Default);
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
        /// <exception cref="ArgumentException"><paramref name="collection"/> is read-only, or <paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="collection"/>.</exception>
        public static void Sort<T>(IList<T> collection, int index, int count, IComparer<T> comparer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (collection.IsReadOnly)
            {
                throw new ArgumentException($"{nameof(collection)} is read-only", nameof(collection));
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
        /// Sorts part of the elements in the collection using the specified comparer.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="index">The starting index of the sort range.</param>
        /// <param name="count">The number of elements in the sort range.</param>
        /// <param name="comparer">The comparer used to compare elements. If it is <see langword="null"/>, the default comparer (<see cref="Comparer{T}.Default"/>) is used.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> is read-only, or <paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="collection"/>.</exception>
        public static void StableSort<T>(IList<T> collection, int index, int count, IComparer<T> comparer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (collection.IsReadOnly)
            {
                throw new ArgumentException($"{nameof(collection)} is read-only", nameof(collection));
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
            InternalStableSort(collection, index, count, comparer ?? Comparer<T>.Default);
        }

        /// <summary>
        /// Sorts the elements in the key collection and correspondingly changes the order of the elements in the value collection.
        /// </summary>
        /// <param name="keys">The key collection.</param>
        /// <param name="values">The value collection. If it is <see langword="null"/>, only the key collection is sorted.</param>
        /// <typeparam name="TKey">The type of the elements in the key collection.</typeparam>
        /// <typeparam name="TValue">The type of the elements in the value collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="keys"/> is read-only, or <paramref name="values"/> is not <see langword="null"/> and is read-only, or <paramref name="values"/> is not <see langword="null"/>, and the number of elements in <paramref name="values"/> is less than the number of elements in <paramref name="keys"/>.</exception>
        public static void Sort<TKey, TValue>(IList<TKey> keys, IList<TValue> values)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            if (keys.IsReadOnly)
            {
                throw new ArgumentException($"{nameof(keys)} is read-only", nameof(keys));
            }
            var count = keys.Count;
            if (values != null)
            {
                if (values.IsReadOnly)
                {
                    throw new ArgumentException($"{nameof(values)} is read-only", nameof(values));
                }
                if (values.Count < count)
                {
                    throw new ArgumentException(
                        $"{nameof(values)}'s number of elements is less than {nameof(keys)}'s number of elements",
                        nameof(values)
                    );
                }
            }
            if (count < 2)
            {
                return;
            }
            InternalSort(keys, values, 0, count, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Sorts the elements in the key collection and correspondingly changes the order of the elements in the value collection.
        /// </summary>
        /// <param name="keys">The key collection.</param>
        /// <param name="values">The value collection. If it is <see langword="null"/>, only the key collection is sorted.</param>
        /// <typeparam name="TKey">The type of the elements in the key collection.</typeparam>
        /// <typeparam name="TValue">The type of the elements in the value collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="keys"/> is read-only, or <paramref name="values"/> is not <see langword="null"/> and is read-only, or <paramref name="values"/> is not <see langword="null"/>, and the number of elements in <paramref name="values"/> is less than the number of elements in <paramref name="keys"/>.</exception>
        public static void StableSort<TKey, TValue>(IList<TKey> keys, IList<TValue> values)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            if (keys.IsReadOnly)
            {
                throw new ArgumentException($"{nameof(keys)} is read-only", nameof(keys));
            }
            var count = keys.Count;
            if (values != null)
            {
                if (values.IsReadOnly)
                {
                    throw new ArgumentException($"{nameof(values)} is read-only", nameof(values));
                }
                if (values.Count < count)
                {
                    throw new ArgumentException(
                        $"{nameof(values)}'s number of elements is less than {nameof(keys)}'s number of elements",
                        nameof(values)
                    );
                }
            }
            if (count < 2)
            {
                return;
            }
            InternalStableSort(keys, values, 0, count, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Sorts the elements in the key collection and correspondingly changes the order of the elements in the value collection, using the specified comparer.
        /// </summary>
        /// <param name="keys">The key collection.</param>
        /// <param name="values">The value collection. If it is <see langword="null"/>, only the key collection is sorted.</param>
        /// <param name="comparer">The comparer used to compare keys. If it is <see langword="null"/>, the default comparer (<see cref="Comparer{T}.Default"/>) is used.</param>
        /// <typeparam name="TKey">The type of the elements in the key collection.</typeparam>
        /// <typeparam name="TValue">The type of the elements in the value collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="keys"/> is read-only, or <paramref name="values"/> is not <see langword="null"/> and is read-only, or <paramref name="values"/> is not <see langword="null"/>, and the number of elements in <paramref name="values"/> is less than the number of elements in <paramref name="keys"/>.</exception>
        public static void Sort<TKey, TValue>(IList<TKey> keys, IList<TValue> values, IComparer<TKey> comparer)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            if (keys.IsReadOnly)
            {
                throw new ArgumentException($"{nameof(keys)} is read-only", nameof(keys));
            }
            var count = keys.Count;
            if (values != null)
            {
                if (values.IsReadOnly)
                {
                    throw new ArgumentException($"{nameof(values)} is read-only", nameof(values));
                }
                if (values.Count < count)
                {
                    throw new ArgumentException(
                        $"{nameof(values)}'s number of elements is less than {nameof(keys)}'s number of elements",
                        nameof(values)
                    );
                }
            }
            if (count < 2)
            {
                return;
            }
            InternalSort(keys, values, 0, count, comparer ?? Comparer<TKey>.Default);
        }

        /// <summary>
        /// Sorts the elements in the key collection and correspondingly changes the order of the elements in the value collection, using the specified comparer.
        /// </summary>
        /// <param name="keys">The key collection.</param>
        /// <param name="values">The value collection. If it is <see langword="null"/>, only the key collection is sorted.</param>
        /// <param name="comparer">The comparer used to compare keys. If it is <see langword="null"/>, the default comparer (<see cref="Comparer{T}.Default"/>) is used.</param>
        /// <typeparam name="TKey">The type of the elements in the key collection.</typeparam>
        /// <typeparam name="TValue">The type of the elements in the value collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="keys"/> is read-only, or <paramref name="values"/> is not <see langword="null"/> and is read-only, or <paramref name="values"/> is not <see langword="null"/>, and the number of elements in <paramref name="values"/> is less than the number of elements in <paramref name="keys"/>.</exception>
        public static void StableSort<TKey, TValue>(IList<TKey> keys, IList<TValue> values, IComparer<TKey> comparer)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            if (keys.IsReadOnly)
            {
                throw new ArgumentException($"{nameof(keys)} is read-only", nameof(keys));
            }
            var count = keys.Count;
            if (values != null)
            {
                if (values.IsReadOnly)
                {
                    throw new ArgumentException($"{nameof(values)} is read-only", nameof(values));
                }
                if (values.Count < count)
                {
                    throw new ArgumentException(
                        $"{nameof(values)}'s number of elements is less than {nameof(keys)}'s number of elements",
                        nameof(values)
                    );
                }
            }
            if (count < 2)
            {
                return;
            }
            InternalStableSort(keys, values, 0, count, comparer ?? Comparer<TKey>.Default);
        }

        /// <summary>
        /// Sorts part of the elements in the key collection and correspondingly changes the order of the elements in the value collection.
        /// </summary>
        /// <param name="keys">The key collection.</param>
        /// <param name="values">The value collection. If it is <see langword="null"/>, only the key collection is sorted.</param>
        /// <param name="index">The starting index of the sort range.</param>
        /// <param name="count">The number of elements in the sort range.</param>
        /// <typeparam name="TKey">The type of the elements in the key collection.</typeparam>
        /// <typeparam name="TValue">The type of the elements in the value collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="keys"/> is read-only, or <paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="keys"/>, or <paramref name="values"/> is not <see langword="null"/> and is read-only, or <paramref name="values"/> is not <see langword="null"/> and <paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="values"/>.</exception>
        public static void Sort<TKey, TValue>(IList<TKey> keys, IList<TValue> values, int index, int count)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            if (keys.IsReadOnly)
            {
                throw new ArgumentException($"{nameof(keys)} is read-only", nameof(keys));
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
            if (values != null)
            {
                if (values.IsReadOnly)
                {
                    throw new ArgumentException($"{nameof(values)} is read-only", nameof(values));
                }
                if (index + count > values.Count)
                {
                    throw new ArgumentException(
                        $"{nameof(index)} plus {nameof(count)} is greater than {nameof(values)}'s number of elements"
                    );
                }
            }
            if (count < 2)
            {
                return;
            }
            InternalSort(keys, values, index, count, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Sorts part of the elements in the key collection and correspondingly changes the order of the elements in the value collection.
        /// </summary>
        /// <param name="keys">The key collection.</param>
        /// <param name="values">The value collection. If it is <see langword="null"/>, only the key collection is sorted.</param>
        /// <param name="index">The starting index of the sort range.</param>
        /// <param name="count">The number of elements in the sort range.</param>
        /// <typeparam name="TKey">The type of the elements in the key collection.</typeparam>
        /// <typeparam name="TValue">The type of the elements in the value collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="keys"/> is read-only, or <paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="keys"/>, or <paramref name="values"/> is not <see langword="null"/> and is read-only, or <paramref name="values"/> is not <see langword="null"/> and <paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="values"/>.</exception>
        public static void StableSort<TKey, TValue>(IList<TKey> keys, IList<TValue> values, int index, int count)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            if (keys.IsReadOnly)
            {
                throw new ArgumentException($"{nameof(keys)} is read-only", nameof(keys));
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
            if (values != null)
            {
                if (values.IsReadOnly)
                {
                    throw new ArgumentException($"{nameof(values)} is read-only", nameof(values));
                }
                if (index + count > values.Count)
                {
                    throw new ArgumentException(
                        $"{nameof(index)} plus {nameof(count)} is greater than {nameof(values)}'s number of elements"
                    );
                }
            }
            if (count < 2)
            {
                return;
            }
            InternalStableSort(keys, values, index, count, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Sorts part of the elements in the key collection and correspondingly changes the order of the elements in the value collection, using the specified comparer.
        /// </summary>
        /// <param name="keys">The key collection.</param>
        /// <param name="values">The value collection. If it is <see langword="null"/>, only the key collection is sorted.</param>
        /// <param name="index">The starting index of the sort range.</param>
        /// <param name="count">The number of elements in the sort range.</param>
        /// <param name="comparer">The comparer used to compare keys. If it is <see langword="null"/>, the default comparer (<see cref="Comparer{T}.Default"/>) is used.</param>
        /// <typeparam name="TKey">The type of the elements in the key collection.</typeparam>
        /// <typeparam name="TValue">The type of the elements in the value collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="keys"/> is read-only, or <paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="keys"/>, or <paramref name="values"/> is not <see langword="null"/> and is read-only, or <paramref name="values"/> is not <see langword="null"/> and <paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="values"/>.</exception>
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
            if (keys.IsReadOnly)
            {
                throw new ArgumentException($"{nameof(keys)} is read-only", nameof(keys));
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
            if (values != null)
            {
                if (values.IsReadOnly)
                {
                    throw new ArgumentException($"{nameof(values)} is read-only", nameof(values));
                }
                if (index + count > values.Count)
                {
                    throw new ArgumentException(
                        $"{nameof(index)} plus {nameof(count)} is greater than {nameof(values)}'s number of elements"
                    );
                }
            }
            if (count < 2)
            {
                return;
            }
            InternalSort(keys, values, index, count, comparer ?? Comparer<TKey>.Default);
        }

        /// <summary>
        /// Sorts part of the elements in the key collection and correspondingly changes the order of the elements in the value collection, using the specified comparer.
        /// </summary>
        /// <param name="keys">The key collection.</param>
        /// <param name="values">The value collection. If it is <see langword="null"/>, only the key collection is sorted.</param>
        /// <param name="index">The starting index of the sort range.</param>
        /// <param name="count">The number of elements in the sort range.</param>
        /// <param name="comparer">The comparer used to compare keys. If it is <see langword="null"/>, the default comparer (<see cref="Comparer{T}.Default"/>) is used.</param>
        /// <typeparam name="TKey">The type of the elements in the key collection.</typeparam>
        /// <typeparam name="TValue">The type of the elements in the value collection.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="count"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="keys"/> is read-only, or <paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="keys"/>, or <paramref name="values"/> is not <see langword="null"/> and is read-only, or <paramref name="values"/> is not <see langword="null"/> and <paramref name="index"/> plus <paramref name="count"/> is greater than the number of elements in <paramref name="values"/>.</exception>
        public static void StableSort<TKey, TValue>(
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
            if (keys.IsReadOnly)
            {
                throw new ArgumentException($"{nameof(keys)} is read-only", nameof(keys));
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
            if (values != null)
            {
                if (values.IsReadOnly)
                {
                    throw new ArgumentException($"{nameof(values)} is read-only", nameof(values));
                }
                if (index + count > values.Count)
                {
                    throw new ArgumentException(
                        $"{nameof(index)} plus {nameof(count)} is greater than {nameof(values)}'s number of elements"
                    );
                }
            }
            if (count < 2)
            {
                return;
            }
            InternalStableSort(keys, values, index, count, comparer ?? Comparer<TKey>.Default);
        }

        private static void InternalSort<T>(IList<T> collection, int index, int count, IComparer<T> comparer)
        {
            QuickSort.InternalSort(collection, index, count, comparer);
        }

        private static void InternalSort<TKey, TValue>(
            IList<TKey>     keys,
            IList<TValue>   values,
            int             index,
            int             count,
            IComparer<TKey> comparer)
        {
            if (values == null)
            {
                QuickSort.InternalSort(keys, index, count, comparer);
            }
            else
            {
                QuickSort.InternalSort(keys, values, index, count, comparer);
            }
        }

        private static void InternalStableSort<T>(IList<T> collection, int index, int count, IComparer<T> comparer)
        {
            TimSort.InternalSort(collection, index, count, comparer);
        }

        private static void InternalStableSort<TKey, TValue>(
            IList<TKey>     keys,
            IList<TValue>   values,
            int             index,
            int             count,
            IComparer<TKey> comparer)
        {
            if (values == null)
            {
                TimSort.InternalSort(keys, index, count, comparer);
            }
            else
            {
                TimSort.InternalSort(keys, values, index, count, comparer);
            }
        }
    }
}
