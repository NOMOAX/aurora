using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Aurora.Collections
{
    /// <summary>
    /// 表示比较两个对象的哈希码的比较器。
    /// </summary>
    /// <typeparam name="T">比较的对象的类型。</typeparam>
    public sealed class HashCodeComparer<T> : IComparer<T> where T : class
    {
        /// <summary>
        /// 获取单一实例。
        /// </summary>
        public static HashCodeComparer<T> Instance { get; } = new HashCodeComparer<T>();

        private HashCodeComparer()
        {
        }

        /// <inheritdoc />
        public int Compare(T x, T y)
        {
            return Comparer<int>.Default.Compare(GetHashCode(x), GetHashCode(y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetHashCode(T obj)
        {
            return obj != null ? obj.GetHashCode() : 0;
        }
    }
}
