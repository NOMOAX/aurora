using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// 表示比较结果与原比较器的比较结果相反的比较器。
    /// </summary>
    /// <typeparam name="T">比较的对象的类型。</typeparam>
    public sealed class ReversedComparer<T> : IComparer<T>
    {
        /// <summary>
        /// 获取比较结果与 <typeparamref name="T"/> 的默认比较器相反的比较器。
        /// </summary>
        public static ReversedComparer<T> Default { get; } = new ReversedComparer<T>(Comparer<T>.Default);

        private readonly IComparer<T> _comparer;

        /// <summary>
        /// 使用默认的原比较器初始化 <see cref="ReversedComparer{T}"/> 类的新实例。
        /// </summary>
        public ReversedComparer()
        {
            _comparer = Comparer<T>.Default;
        }

        /// <summary>
        /// 使用指定的原比较器初始化 <see cref="ReversedComparer{T}"/> 类的新实例。
        /// </summary>
        /// <param name="comparer">原比较器，如果为 <see langword="null"/>，则使用默认的原比较器。</param>
        public ReversedComparer(IComparer<T> comparer)
        {
            _comparer = comparer ?? Comparer<T>.Default;
        }

        /// <inheritdoc />
        public int Compare(T x, T y)
        {
            return _comparer.Compare(y, x);
        }
    }
}
