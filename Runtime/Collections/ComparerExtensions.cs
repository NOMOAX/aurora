using System;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// 为 <see cref="IComparer{T}"/> 接口提供扩展方法。
    /// </summary>
    public static class ComparerExtensions
    {
        /// <summary>
        /// 获取与当前 <see cref="IComparer{T}"/> 的比较结果相反的比较器。
        /// </summary>
        /// <param name="comparer">比较器。</param>
        /// <typeparam name="T">比较的对象的类型。</typeparam>
        /// <returns>与当前 <see cref="IComparer{T}"/> 的比较结果相反的比较器。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> 为 <see langword="null"/>。</exception>
        public static IComparer<T> Reversed<T>(this IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            return new ReversedComparer<T>(comparer);
        }
    }
}
