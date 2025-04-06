using System;
using System.Collections.Generic;

namespace Aurora.Sorting
{
    internal static class SortHelper
    {
        internal static void Copy<T>(
            IList<T> source,
            int      sourceIndex,
            IList<T> destination,
            int      destinationIndex,
            int      count)
        {
            if (source is T[] sourceArray && destination is T[] destinationArray)
            {
                Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, count);
            }
            else if (!Equals(source, destination))
            {
                CopyFromLeftToRight(source, sourceIndex, destination, destinationIndex, count);
            }
            else if (sourceIndex != destinationIndex)
            {
                if (destinationIndex > sourceIndex && destinationIndex < sourceIndex + count)
                {
                    CopyFromRightToLeft(source, sourceIndex, destination, destinationIndex, count);
                }
                else
                {
                    CopyFromLeftToRight(source, sourceIndex, destination, destinationIndex, count);
                }
            }
        }

        private static void CopyFromLeftToRight<T>(
            IList<T> source,
            int      sourceIndex,
            IList<T> destination,
            int      destinationIndex,
            int      count)
        {
            while (count-- > 0)
            {
                destination[destinationIndex++] = source[sourceIndex++];
            }
        }

        private static void CopyFromRightToLeft<T>(
            IList<T> source,
            int      sourceIndex,
            IList<T> destination,
            int      destinationIndex,
            int      count)
        {
            sourceIndex      += count - 1;
            destinationIndex += count - 1;
            while (count-- > 0)
            {
                destination[destinationIndex--] = source[sourceIndex--];
            }
        }
    }
}
