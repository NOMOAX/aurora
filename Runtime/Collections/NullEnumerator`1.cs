using System;
using System.Collections;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// 使用空对象模式实现 <see cref="IEnumerator{T}"/>。
    /// </summary>
    /// <typeparam name="T">要枚举的对象的类型。</typeparam>
    public sealed class NullEnumerator<T> : IEnumerator<T>
    {
        /// <summary>
        /// 获取单一实例。
        /// </summary>
        public static NullEnumerator<T> Instance { get; } = new NullEnumerator<T>();

        private NullEnumerator()
        {
        }

        bool IEnumerator.MoveNext()
        {
            return false;
        }

        T IEnumerator<T>.Current => default;

        object IEnumerator.Current => default(T);

        void IEnumerator.Reset()
        {
        }

        void IDisposable.Dispose()
        {
        }
    }
}
