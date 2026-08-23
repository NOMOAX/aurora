using System;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// Depth-first, recursively enumerates the tree's nodes following the rule "enumerate the root node first, then enumerate each child node in reverse order".
    /// </summary>
    /// <typeparam name="T">The type of the tree's nodes.</typeparam>
    public sealed class DrlEnumerator<T> : DepthFirstDataFirstEnumerator<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrlEnumerator{T}"/> class.
        /// </summary>
        /// <inheritdoc />
        public DrlEnumerator(
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
