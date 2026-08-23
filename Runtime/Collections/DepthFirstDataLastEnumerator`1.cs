using System;
using System.Collections;
using System.Collections.Generic;
using Aurora.Pooling;

namespace Aurora.Collections
{
    /// <summary>
    /// Depth-first, recursively enumerates the tree's nodes following the rule "enumerate the root node last".
    /// </summary>
    /// <typeparam name="T">The type of the tree's nodes.</typeparam>
    /// <remarks>This enumerator computes the enumeration result of every step in the constructor and in <see cref="IEnumerator.Reset"/>. Be aware of the performance cost caused by this behavior.</remarks>
    public abstract class DepthFirstDataLastEnumerator<T> : TreeEnumerator<T> where T : class
    {
        private Stack<T> _stack;

        private T _current;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepthFirstDataLastEnumerator{T}"/> class.
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
        /// Adds the children of a precomputed node, computed by <see cref="TreeEnumerator{T}.FuncGetChildren"/>, to the top of the internal stack.
        /// </summary>
        /// <param name="stack">The stack.</param>
        /// <param name="children">The children of the precomputed node.</param>
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
