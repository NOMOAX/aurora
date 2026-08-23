using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Aurora.Collections
{
    /// <summary>
    /// 表示使用指定的枚举器的可枚举对象。
    /// </summary>
    /// <typeparam name="T">要枚举的对象的类型。</typeparam>
    public readonly struct EnumeratorEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerator<T> _enumerator;

        /// <summary>
        /// 初始化 <see cref="EnumeratorEnumerable{T}"/> 结构的新实例。
        /// </summary>
        /// <param name="enumerator">枚举器。</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerator"/> 为 <see langword="null"/>。</exception>
        public EnumeratorEnumerable(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
        }

        /// <summary>
        /// 初始化 <see cref="EnumeratorEnumerable{T}"/> 结构的新实例。
        /// </summary>
        /// <param name="enumerator">枚举器。</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerator"/> 为 <see langword="null"/>。</exception>
        /// <remarks>如果枚举的对象不能转换为 <typeparamref name="T"/> 类型，将在枚举时抛出 <see cref="InvalidCastException"/> 异常。</remarks>
        public EnumeratorEnumerable(IEnumerator enumerator)
        {
            if (enumerator == null)
            {
                throw new ArgumentNullException(nameof(enumerator));
            }
            _enumerator = new EnumeratorEnumerable(enumerator).Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _enumerator;
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return _enumerator;
        }
    }
}
