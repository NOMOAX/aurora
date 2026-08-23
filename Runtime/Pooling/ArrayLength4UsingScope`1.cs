using System;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// A using scope for an array of length 4.
    /// </summary>
    /// <typeparam name="T">The type of the array's elements.</typeparam>
    public class ArrayLength4UsingScope<T> : IDisposable
    {
        private T[] _array;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayLength4UsingScope{T}"/> class.
        /// </summary>
        /// <param name="array">This output parameter is assigned an array of length 4 whose elements are all default values.</param>
        public ArrayLength4UsingScope(out T[] array)
        {
            _array = PredefinedPools<T>.ArrayLength4.Get();
            array  = _array;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var array = _array;
            if (array != null && Interlocked.CompareExchange(ref _array, null, array) == array)
            {
                PredefinedPools<T>.ArrayLength4.Return(array);
            }
        }
    }
}
