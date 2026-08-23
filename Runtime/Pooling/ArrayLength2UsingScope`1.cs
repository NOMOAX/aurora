using System;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// 使用长度为 2 的数组范围。
    /// </summary>
    /// <typeparam name="T">数组的元素的类型。</typeparam>
    public class ArrayLength2UsingScope<T> : IDisposable
    {
        private T[] _array;

        /// <summary>
        /// 初始化 <see cref="ArrayLength2UsingScope{T}"/> 类的新实例。
        /// </summary>
        /// <param name="array">此输出参数将被赋值为一个各元素都为默认值的长度为 2 的数组。</param>
        public ArrayLength2UsingScope(out T[] array)
        {
            _array = PredefinedPools<T>.ArrayLength2.Get();
            array  = _array;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var array = _array;
            if (array != null && Interlocked.CompareExchange(ref _array, null, array) == array)
            {
                PredefinedPools<T>.ArrayLength2.Return(array);
            }
        }
    }
}
