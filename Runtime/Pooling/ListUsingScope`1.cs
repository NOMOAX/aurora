using System;
using System.Collections.Generic;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// 使用列表范围。
    /// </summary>
    /// <typeparam name="T">列表的成员的类型。</typeparam>
    public sealed class ListUsingScope<T> : IDisposable
    {
        private List<T> _list;

        /// <summary>
        /// 初始化 <see cref="ListUsingScope{T}"/> 类的新实例。
        /// </summary>
        /// <param name="list">此输出参数将被赋值为一个空列表。</param>
        public ListUsingScope(out List<T> list)
        {
            _list = PredefinedPools<T>.List.Get();
            list  = _list;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var list = _list;
            if (list != null && Interlocked.CompareExchange(ref _list, null, list) == list)
            {
                PredefinedPools<T>.List.Return(list);
            }
        }
    }
}
