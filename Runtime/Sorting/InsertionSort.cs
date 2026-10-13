using System.Collections.Generic;

namespace Aurora.Sorting
{
    internal static class InsertionSort
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
                var elementAtI = collection[i];
                var j          = i - 1;
                if (j >= index && comparer.Compare(collection[j], elementAtI) > 0)
                {
                    do
                    {
                        collection[j + 1] = collection[j];
                        j--;
                    } while (j >= index && comparer.Compare(collection[j], elementAtI) > 0);
                    collection[j + 1] = elementAtI;
                }
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
                var keyAtI   = keys[i];
                var valueAtI = values[i];
                var j        = i - 1;
                if (j >= index && comparer.Compare(keys[j], keyAtI) > 0)
                {
                    do
                    {
                        keys[j + 1]   = keys[j];
                        values[j + 1] = values[j];
                        j--;
                    } while (j >= index && comparer.Compare(keys[j], keyAtI) > 0);
                    keys[j + 1]   = keyAtI;
                    values[j + 1] = valueAtI;
                }
            }
        }
    }
}
