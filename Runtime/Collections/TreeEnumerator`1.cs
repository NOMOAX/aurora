using System;
using System.Collections;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// 枚举树的结点。
    /// </summary>
    /// <typeparam name="T">树的结点的类型。</typeparam>
    public abstract class TreeEnumerator<T> : IEnumerator<T> where T : class
    {
        /// <summary>
        /// 根结点。
        /// </summary>
        protected T RootNode;

        /// <summary>
        /// 获取指定结点的所有子结点的集合的方法。
        /// <br/>
        /// （特别地，如果该方法的返回值为 <see cref="IReadOnlyList{T}"/> 或 <see cref="IList{T}"/>，有助于提升此枚举器的效率。）
        /// </summary>
        protected readonly Func<T, IEnumerable<T>> FuncGetChildren;

        private readonly Func<T, object, bool> _funcValidate;

        private readonly object _validateState;

        /// <summary>
        /// 初始化 <see cref="TreeEnumerator{T}"/> 类的新实例。
        /// </summary>
        /// <param name="rootNode">
        /// 根结点。
        /// <br/>
        /// （它将传递给 <paramref name="funcValidate"/> 的第 1 个参数。）
        /// </param>
        /// <param name="funcGetChildren">
        /// 获取指定结点的所有子结点的集合的方法。
        /// <br/>
        /// （特别地，如果该方法的返回值为 <see cref="IReadOnlyList{T}"/> 或 <see cref="IList{T}"/>，有助于提升此枚举器的效率。）
        /// </param>
        /// <param name="funcValidate">
        /// 用于校验的方法。
        /// <br/>
        /// 如果返回值为 <see langword="true"/>，则表示校验通过，否则表示检验失败。
        /// <br/>
        /// （将在 <see cref="IEnumerator.MoveNext"/> 和 <see cref="IEnumerator.Reset"/> 时调用。第 1 个参数将传入 <paramref name="rootNode"/>，第 2 个参数将传入 <paramref name="validateState"/>。）
        /// </param>
        /// <param name="validateState">
        /// 自定义数据。
        /// <br/>
        /// （它将传递给 <paramref name="funcValidate"/> 的第 2 个参数。）
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="rootNode"/> 或 <paramref name="funcGetChildren"/> 为 <see langword="null"/>。</exception>
        protected TreeEnumerator(
            T                       rootNode,
            Func<T, IEnumerable<T>> funcGetChildren,
            Func<T, object, bool>   funcValidate  = null,
            object                  validateState = null)
        {
            RootNode        = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
            FuncGetChildren = funcGetChildren ?? throw new ArgumentNullException(nameof(funcGetChildren));
            _funcValidate   = funcValidate;
            _validateState  = validateState;
        }

        /// <summary>
        /// 如果校验失败，则抛出 <see cref="InvalidOperationException"/>。
        /// </summary>
        /// <exception cref="InvalidOperationException">校验失败。</exception>
        protected void ThrowIfInvalid()
        {
            if (_funcValidate == null)
            {
                return;
            }
            if (_funcValidate(RootNode, _validateState))
            {
                return;
            }
            throw new InvalidOperationException();
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
        }

        /// <inheritdoc />
        public abstract bool MoveNext();

        /// <inheritdoc />
        public abstract T Current { get; }

        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public virtual void Reset()
        {
        }
    }
}
