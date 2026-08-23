using System;
using System.Collections;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// Depth-first, recursively enumerates the tree's nodes following the rule "enumerate each child node first, then enumerate the root node".
    /// </summary>
    /// <typeparam name="T">The type of the tree's nodes.</typeparam>
    /// <remarks>This enumerator computes the enumeration result of every step in the constructor and in <see cref="IEnumerator.Reset"/>. Be aware of the performance cost caused by this behavior.</remarks>
    public sealed class LrdEnumerator<T> : DepthFirstDataLastEnumerator<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LrdEnumerator{T}"/> class.
        /// </summary>
        /// <inheritdoc />
        public LrdEnumerator(
            T                       rootNode,
            Func<T, IEnumerable<T>> getChildrenFunc,
            Func<T, object, bool>   validateFunc  = null,
            object                  validateState = null) : base(rootNode, getChildrenFunc, validateFunc, validateState)
        {
        }

        /// <inheritdoc />
        protected override void PushChildren(Stack<T> stack, IEnumerable<T> children)
        {
            foreach (var child in children)
            {
                stack.Push(child);
            }
        }
    }
}
