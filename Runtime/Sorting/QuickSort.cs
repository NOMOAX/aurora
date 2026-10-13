using System.Collections.Generic;
using Aurora.Pooling;

namespace Aurora.Sorting
{
    /// <summary>
    /// A quick-sort algorithm, with the usual practical optimizations applied to the plain textbook version.
    /// </summary>
    internal static class QuickSort
    {
        /// <summary>
        /// Sections of at most this many elements are finished off with an insertion sort.
        /// </summary>
        private const int InsertionSortThreshold = 16;

        /// <summary>
        /// Sections of at most this many elements use a median of three as the pivot; larger ones use the median of the medians of three evenly spread triples.
        /// </summary>
        private const int MedianOfMediansThreshold = 40;

        /// <summary>
        /// Sorts the requested range. The caller is responsible for validating the arguments.
        /// </summary>
        internal static void InternalSort<T>(IList<T> collection, int index, int count, IComparer<T> comparer)
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

            if (right - left + 1 <= InsertionSortThreshold)
            {
                InsertionSort.InternalSort(collection, left, right - left + 1, left, comparer);
                return;
            }
            /*
             * Partition around the median of three, kept in a hole at the left end
             * The scans compare strictly, so keys equal to the pivot stop them and end up on both sides; that keeps
             * all-equal and duplicate-heavy input at O(n log n), while each element is still written at most once
             */
            var pivotIndex = ChoosePivotIndex(collection, left, right, comparer);
            if (pivotIndex != left)
            {
                (collection[left], collection[pivotIndex]) = (collection[pivotIndex], collection[left]);
            }

            var key = collection[left];
            var l   = left;
            var r   = right;
            while (l < r)
            {
                while (l < r && comparer.Compare(collection[r], key) > 0)
                {
                    r--;
                }
                if (l < r)
                {
                    collection[l++] = collection[r];
                }
                while (l < r && comparer.Compare(collection[l], key) < 0)
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

            if (right - left + 1 <= InsertionSortThreshold)
            {
                InsertionSort.InternalSort(keys, values, left, right - left + 1, left, comparer);
                return;
            }
            /*
             * Partition around the median of three, kept in a hole at the left end
             * The scans compare strictly, so keys equal to the pivot stop them and end up on both sides; that keeps
             * all-equal and duplicate-heavy input at O(n log n), while each element is still written at most once
             */
            var pivotIndex = ChoosePivotIndex(keys, left, right, comparer);
            if (pivotIndex != left)
            {
                (keys[left], keys[pivotIndex])     = (keys[pivotIndex], keys[left]);
                (values[left], values[pivotIndex]) = (values[pivotIndex], values[left]);
            }

            var key   = keys[left];
            var value = values[left];
            var l     = left;
            var r     = right;
            while (l < r)
            {
                while (l < r && comparer.Compare(keys[r], key) > 0)
                {
                    r--;
                }
                if (l < r)
                {
                    keys[l]   = keys[r];
                    values[l] = values[r];
                    l++;
                }
                while (l < r && comparer.Compare(keys[l], key) < 0)
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

        private static int ChoosePivotIndex<T>(IList<T> collection, int left, int right, IComparer<T> comparer)
        {
            var count = right - left + 1;
            if (count <= MedianOfMediansThreshold)
            {
                return MedianOfThreeIndex(collection, left, left + (count >> 1), right, comparer);
            }
            /*
             * The median of the medians of three evenly spread triples
             * It stays balanced on distributions that defeat a plain median of three, such as an organ pipe
             */
            var step = (right - left) / 8;
            var m1   = MedianOfThreeIndex(collection, left,            left + step,     left + step * 2, comparer);
            var m2   = MedianOfThreeIndex(collection, left + step * 3, left + step * 4, left + step * 5, comparer);
            var m3   = MedianOfThreeIndex(collection, left + step * 6, left + step * 7, right,           comparer);
            return MedianOfThreeIndex(collection, m1, m2, m3, comparer);
        }

        private static int MedianOfThreeIndex<T>(IList<T> collection, int i, int j, int k, IComparer<T> comparer)
        {
            return i == j || comparer.Compare(collection[i], collection[j]) <= 0
                       ? j == k || comparer.Compare(collection[j], collection[k]) <= 0
                             ? j /* i <= j <= k */
                             : i == k || comparer.Compare(collection[i], collection[k]) <= 0
                                 ? k /* i <= k < j */
                                 : i /* k < i < j */
                       : j == k || comparer.Compare(collection[j], collection[k]) <= 0
                           ? i == k || comparer.Compare(collection[i], collection[k]) <= 0
                                 ? i /* j < i <= k */
                                 : k /* j < k < i */
                           : j /* k < j < i */;
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
