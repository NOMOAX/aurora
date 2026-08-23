using System;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// Breadth-first, enumerates the tree's nodes following the rule "from upper levels to lower levels, then enumerate each node within each level".
    /// </summary>
    /// <typeparam name="T">The type of the tree's nodes.</typeparam>
    public sealed class LrEnumerator<T> : BreadthFirstEnumerator<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LrEnumerator{T}"/> class.
        /// </summary>
        /// <inheritdoc />
        public LrEnumerator(
            T                       rootNode,
            Func<T, IEnumerable<T>> getChildrenFunc,
            Func<T, object, bool>   validateFunc  = null,
            object                  validateState = null) : base(rootNode, getChildrenFunc, validateFunc, validateState)
        {
        }

        /// <inheritdoc />
        protected override void EnqueueChildren(Queue<T> queue, IEnumerable<T> children)
        {
            foreach (var child in children)
            {
                queue.Enqueue(child);
            }
        }
    }
}
