using System;
using System.Collections.Generic;

namespace Aurora.Collections
{
    internal static class EnumerableHelpers
    {
        internal static T[] ToArray<T>(IEnumerable<T> enumerable, out int length)
        {
            if (enumerable is ICollection<T> collection)
            {
                length = collection.Count;
                if (length == 0)
                {
                    return Array.Empty<T>();
                }
                var array = new T[length];
                collection.CopyTo(array, 0);
                return array;
            }

            {
                using var enumerator = enumerable.GetEnumerator();
                if (!enumerator.MoveNext())
                {
                    length = 0;
                    return Array.Empty<T>();
                }

                const int defaultCapacity = 4;
                const int growFactor      = 2;

                var array = new T[defaultCapacity];
                array[0] = enumerator.Current;
                length   = 1;
                while (enumerator.MoveNext())
                {
                    if (length == array.Length)
                    {
                        var newLength = checked(length * growFactor);
                        Array.Resize(ref array, newLength);
                    }
                    array[length++] = enumerator.Current;
                }
                return array;
            }
        }
    }
}
