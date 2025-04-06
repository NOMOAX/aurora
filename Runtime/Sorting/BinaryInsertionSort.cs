using System.Collections.Generic;

namespace Aurora.Sorting
{
    internal static class BinaryInsertionSort
    {
        internal static void InternalSort<T>(
            IList<T>     collection,
            int          index,
            int          count,
            int          startIndex,
            IComparer<T> comparer)
        {
            if (startIndex == index)
            {
                startIndex++;
            }
            for (var i = startIndex; i < index + count; i++)
            {
                var elementAtI  = collection[i];
                var insertIndex = BinarySearchInsertIndex(collection, index, i - index, elementAtI, comparer);
                if (insertIndex == i)
                {
                    continue;
                }
                SortHelper.Copy(collection, insertIndex, collection, insertIndex + 1, i - insertIndex);
                collection[insertIndex] = elementAtI;
            }
        }

        internal static void InternalSort<TKey, TValue>(
            IList<TKey>     keys,
            IList<TValue>   values,
            int             index,
            int             count,
            int             startIndex,
            IComparer<TKey> comparer)
        {
            if (startIndex == index)
            {
                startIndex++;
            }
            for (var i = startIndex; i < index + count; i++)
            {
                var keyAtI      = keys[i];
                var insertIndex = BinarySearchInsertIndex(keys, index, i - index, keyAtI, comparer);
                if (insertIndex == i)
                {
                    continue;
                }
                SortHelper.Copy(keys, insertIndex, keys, insertIndex + 1, i - insertIndex);
                keys[insertIndex] = keyAtI;
                var valueAtI = values[i];
                SortHelper.Copy(values, insertIndex, values, insertIndex + 1, i - insertIndex);
                values[insertIndex] = valueAtI;
            }
        }

        private static int BinarySearchInsertIndex<T>(
            IList<T>     collection,
            int          index,
            int          count,
            T            target,
            IComparer<T> comparer)
        {
            var left  = index;
            var right = index + count - 1;
            while (left <= right)
            {
                var median = left + ((right - left) >> 1);
                if (comparer.Compare(collection[median], target) <= 0)
                {
                    left = median + 1;
                }
                else
                {
                    right = median - 1;
                }
            }
            return left;
        }
    }
}
