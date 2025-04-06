using System;
using System.Collections;

namespace Aurora.Collections
{
    /// <summary>
    /// 表示使用指定的枚举器的可枚举对象。
    /// </summary>
    public readonly struct EnumeratorEnumerable : IEnumerable
    {
        private readonly IEnumerator _enumerator;

        /// <summary>
        /// 初始化 <see cref="EnumeratorEnumerable"/> 结构的新实例。
        /// </summary>
        /// <param name="enumerator">枚举器。</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerator"/> 为 <see langword="null"/>。</exception>
        public EnumeratorEnumerable(IEnumerator enumerator)
        {
            _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
        }

        /// <inheritdoc />
        public IEnumerator GetEnumerator()
        {
            return _enumerator;
        }
    }
}
