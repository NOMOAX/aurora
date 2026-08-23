using System;
using System.Collections;
using System.Collections.Generic;
using Aurora.Pooling;

namespace Aurora.Collections
{
    /// <summary>
    /// 深度优先、递归地，按照“后枚举根结点”的规则，枚举树的结点。
    /// </summary>
    /// <typeparam name="T">树的结点的类型。</typeparam>
    /// <remarks>此枚举器将在构造函数中和 <see cref="IEnumerator.Reset"/> 中计算完每一步的枚举结果，请注意此行为导致的性能消耗。</remarks>
    public abstract class DepthFirstDataLastEnumerator<T> : TreeEnumerator<T> where T : class
    {
        private Stack<T> _stack;

        private T _current;

        /// <summary>
        /// 初始化 <see cref="DepthFirstDataLastEnumerator{T}"/> 类的新实例。
        /// </summary>
        /// <inheritdoc />
        protected DepthFirstDataLastEnumerator(
            T                       rootNode,
            Func<T, IEnumerable<T>> funcGetChildren,
            Func<T, object, bool>   funcValidate  = null,
            object                  validateState = null) : base(rootNode, funcGetChildren, funcValidate, validateState)
        {
            _stack = PredefinedPools<T>.Stack.Get();
            PopulateStack();
            _current = null;
        }

        private void PopulateStack()
        {
            var stack = PredefinedPools<T>.Stack.Get();
            try
            {
                stack.Push(RootNode);
                do
                {
                    var pop = stack.Pop();
                    _stack.Push(pop);
                    var children = FuncGetChildren(pop);
                    PushChildren(stack, children);
                } while (stack.Count > 0);
            }
            finally
            {
                PredefinedPools<T>.Stack.Return(stack);
            }
        }

        /// <summary>
        /// 将预计算结点经 <see cref="TreeEnumerator{T}.FuncGetChildren"/> 计算后得到的子结点集合添加到内部栈的顶部。
        /// </summary>
        /// <param name="stack">栈。</param>
        /// <param name="children">预计算结点的子结点集合。</param>
        protected abstract void PushChildren(Stack<T> stack, IEnumerable<T> children);

        private void ThrowIfDisposed()
        {
            if (RootNode != null)
            {
                return;
            }
            throw new ObjectDisposedException(GetType().FullName);
        }

        /// <inheritdoc />
        public sealed override void Dispose()
        {
            try
            {
                if (RootNode == null)
                {
                    return;
                }
                RootNode = null;
                PredefinedPools<T>.Stack.Return(_stack);
                _stack   = null;
                _current = null;
            }
            finally
            {
                base.Dispose();
            }
        }

        /// <inheritdoc />
        public override bool MoveNext()
        {
            ThrowIfDisposed();
            ThrowIfInvalid();
            if (_stack.Count == 0)
            {
                _current = null;
                return false;
            }
            _current = _stack.Pop();
            return true;
        }

        /// <inheritdoc />
        public override T Current
        {
            get
            {
                ThrowIfDisposed();
                return _current;
            }
        }

        /// <inheritdoc />
        public sealed override void Reset()
        {
            try
            {
                ThrowIfDisposed();
                ThrowIfInvalid();
                _stack.Clear();
                PopulateStack();
                _current = null;
            }
            finally
            {
                base.Reset();
            }
        }
    }
}
