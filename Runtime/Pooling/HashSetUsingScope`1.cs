using System;
using System.Collections.Generic;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// 使用哈希集范围。
    /// </summary>
    /// <typeparam name="T">哈希集的成员的类型。</typeparam>
    public sealed class HashSetUsingScope<T> : IDisposable
    {
        private HashSet<T> _hashSet;

        /// <summary>
        /// 初始化 <see cref="HashSetUsingScope{T}"/> 类的新实例。
        /// </summary>
        /// <param name="hashSet">此输出参数将被赋值为一个空哈希集。</param>
        public HashSetUsingScope(out HashSet<T> hashSet)
        {
            _hashSet = PredefinedPools<T>.HashSet.Get();
            hashSet  = _hashSet;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var hashSet = _hashSet;
            if (hashSet != null && Interlocked.CompareExchange(ref _hashSet, null, hashSet) == hashSet)
            {
                PredefinedPools<T>.HashSet.Return(hashSet);
            }
        }
    }
}
