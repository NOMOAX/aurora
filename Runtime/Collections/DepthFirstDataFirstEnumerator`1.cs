using System;
using System.Collections.Generic;
using Aurora.Pooling;

namespace Aurora.Collections
{
    /// <summary>
    /// Depth-first, recursively enumerates the tree's nodes following the rule "enumerate the root node first".
    /// </summary>
    /// <typeparam name="T">The type of the tree's nodes.</typeparam>
    public abstract class DepthFirstDataFirstEnumerator<T> : TreeEnumerator<T> where T : class
    {
        private Stack<T> _stack;

        private T _current;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepthFirstDataFirstEnumerator{T}"/> class.
        /// </summary>
        /// <inheritdoc />
        protected DepthFirstDataFirstEnumerator(
            T                       rootNode,
            Func<T, IEnumerable<T>> funcGetChildren,
            Func<T, object, bool>   funcValidate  = null,
            object                  validateState = null) : base(rootNode, funcGetChildren, funcValidate, validateState)
        {
            _stack = PredefinedPools<T>.Stack.Get();
            _stack.Push(RootNode);
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
            var children = FuncGetChildren(_current);
            PushChildren(_stack, children);
            return true;
        }

        /// <summary>
        /// Adds the children of the current node, computed by <see cref="TreeEnumerator{T}.FuncGetChildren"/>, to the top of the internal stack.
        /// </summary>
        /// <param name="stack">The stack.</param>
        /// <param name="children">The children of the current node.</param>
        protected abstract void PushChildren(Stack<T> stack, IEnumerable<T> children);

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
                _stack.Push(RootNode);
                _current = null;
            }
            finally
            {
                base.Reset();
            }
        }
    }
}
