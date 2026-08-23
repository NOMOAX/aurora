using System;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// 表示可通过指定的方法比较两个对象的比较器。
    /// </summary>
    /// <typeparam name="T">要比较的对象的类型。</typeparam>
    public sealed class FunctorComparer<T> : IComparer<T>
    {
        private readonly Comparison<T> _comparison;

        /// <summary>
        /// 初始化 <see cref="FunctorComparer{T}"/> 类的新实例。
        /// </summary>
        /// <param name="comparison">用于比较 <typeparamref name="T"/> 类型的两个对象的方法。</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/> 为 <see langword="null"/>。</exception>
        public FunctorComparer(Comparison<T> comparison)
        {
            _comparison = comparison ?? throw new ArgumentNullException(nameof(comparison));
        }

        /// <inheritdoc />
        public int Compare(T x, T y)
        {
            return _comparison(x, y);
        }
    }
}
