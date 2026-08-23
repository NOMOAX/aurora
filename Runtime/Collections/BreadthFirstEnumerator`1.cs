using System;
using System.Collections.Generic;
using Aurora.Pooling;

namespace Aurora.Collections
{
    /// <summary>
    /// Breadth-first, enumerates the tree's nodes following the rule "from upper levels to lower levels".
    /// </summary>
    /// <typeparam name="T">The type of the tree's nodes.</typeparam>
    public abstract class BreadthFirstEnumerator<T> : TreeEnumerator<T> where T : class
    {
        private Queue<T> _queue;

        private T _current;

        /// <summary>
        /// Initializes a new instance of the <see cref="BreadthFirstEnumerator{T}"/> class.
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
        /// Adds the children of the current node, computed by <see cref="TreeEnumerator{T}.FuncGetChildren"/>, to the end of the internal queue.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="children">The children of the current node.</param>
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
