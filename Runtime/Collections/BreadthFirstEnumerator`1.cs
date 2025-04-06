using System;
using System.Collections.Generic;
using Aurora.Pooling;

namespace Aurora.Collections
{
    /// <summary>
    /// 广度优先地，按照“从上层到下层“的规则，枚举树的结点。
    /// </summary>
    /// <typeparam name="T">树的结点的类型。</typeparam>
    public abstract class BreadthFirstEnumerator<T> : TreeEnumerator<T> where T : class
    {
        private Queue<T> _queue;

        private T _current;

        /// <summary>
        /// 初始化 <see cref="BreadthFirstEnumerator{T}"/> 类的新实例。
        /// </summary>
        /// <inheritdoc />
        protected BreadthFirstEnumerator(
            T                       rootNode,
            Func<T, IEnumerable<T>> getChildrenFunc,
            Func<T, object, bool>   validateFunc  = null,
            object                  validateState = null) : base(rootNode, getChildrenFunc, validateFunc, validateState)
        {
            _queue = PredefinedPools<T>.Queue.Get();
            _queue.Enqueue(RootNode);
            _current = null;
        }

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
                PredefinedPools<T>.Queue.Return(_queue);
                _queue   = null;
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
            if (_queue.Count == 0)
            {
                _current = null;
                return false;
            }
            _current = _queue.Dequeue();
            var children = FuncGetChildren(_current);
            EnqueueChildren(_queue, children);
            return true;
        }

        /// <summary>
        /// 将当前结点经 <see cref="TreeEnumerator{T}.FuncGetChildren"/> 计算后得到的子结点集合添加到内部队列的结尾处。
        /// </summary>
        /// <param name="queue">队列。</param>
        /// <param name="children">当前结点的子结点集合。</param>
        protected abstract void EnqueueChildren(Queue<T> queue, IEnumerable<T> children);

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
                _queue.Clear();
                _queue.Enqueue(RootNode);
                _current = null;
            }
            finally
            {
                base.Reset();
            }
        }
    }
}
