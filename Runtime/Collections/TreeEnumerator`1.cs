using System;
using System.Collections;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// Enumerates the tree's nodes.
    /// </summary>
    /// <typeparam name="T">The type of the tree's nodes.</typeparam>
    public abstract class TreeEnumerator<T> : IEnumerator<T> where T : class
    {
        /// <summary>
        /// The root node.
        /// </summary>
        protected T RootNode;

        /// <summary>
        /// A method that gets the collection of all children of a specified node.
        /// <br/>
        /// (In particular, if the return value of this method is <see cref="IReadOnlyList{T}"/> or <see cref="IList{T}"/>, it helps improve the efficiency of this enumerator.)
        /// </summary>
        protected readonly Func<T, IEnumerable<T>> FuncGetChildren;

        private readonly Func<T, object, bool> _funcValidate;

        private readonly object _validateState;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeEnumerator{T}"/> class.
        /// </summary>
        /// <param name="rootNode">
        /// The root node.
        /// <br/>
        /// (It will be passed as the 1st argument to <paramref name="funcValidate"/>.)
        /// </param>
        /// <param name="funcGetChildren">
        /// A method that gets the collection of all children of a specified node.
        /// <br/>
        /// (In particular, if the return value of this method is <see cref="IReadOnlyList{T}"/> or <see cref="IList{T}"/>, it helps improve the efficiency of this enumerator.)
        /// </param>
        /// <param name="funcValidate">
        /// The validation method.
        /// <br/>
        /// If the return value is <see langword="true"/>, the validation passes; otherwise, it fails.
        /// <br/>
        /// (It will be called in <see cref="IEnumerator.MoveNext"/> and <see cref="IEnumerator.Reset"/>. The 1st argument will be <paramref name="rootNode"/>, and the 2nd will be <paramref name="validateState"/>.)
        /// </param>
        /// <param name="validateState">
        /// Custom data.
        /// <br/>
        /// (It will be passed as the 2nd argument to <paramref name="funcValidate"/>.)
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="rootNode"/> or <paramref name="funcGetChildren"/> is <see langword="null"/>.</exception>
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
        /// Throws <see cref="InvalidOperationException"/> if the validation fails.
        /// </summary>
        /// <exception cref="InvalidOperationException">The validation failed.</exception>
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
