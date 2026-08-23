using System;
using System.Collections.Generic;
using Aurora.Pooling;

namespace Aurora.Collections
{
    /// <summary>
    /// Breadth-first, enumerates the tree's nodes following the rule "from upper levels to lower levels, then enumerate each node in reverse order within each level".
    /// </summary>
    /// <typeparam name="T">The type of the tree's nodes.</typeparam>
    public sealed class RlEnumerator<T> : BreadthFirstEnumerator<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RlEnumerator{T}"/> class.
        /// </summary>
        /// <inheritdoc />
        public RlEnumerator(
            T                       rootNode,
            Func<T, IEnumerable<T>> getChildrenFunc,
            Func<T, object, bool>   validateFunc  = null,
            object                  validateState = null) : base(rootNode, getChildrenFunc, validateFunc, validateState)
        {
        }

        /// <inheritdoc />
        protected override void EnqueueChildren(Queue<T> queue, IEnumerable<T> children)
        {
            switch (children)
            {
                case IReadOnlyList<T> readOnlyList:
                    for (var i = readOnlyList.Count - 1; i >= 0; i--)
                    {
                        queue.Enqueue(readOnlyList[i]);
                    }
                    break;
                case IList<T> list:
                    for (var i = list.Count - 1; i >= 0; i--)
                    {
                        queue.Enqueue(list[i]);
                    }
                    break;
                default:
                    var list1 = PredefinedPools<T>.List.Get();
                    try
                    {
                        list1.AddRange(children);
                        for (var i = list1.Count - 1; i >= 0; i--)
                        {
                            queue.Enqueue(list1[i]);
                        }
                    }
                    finally
                    {
                        PredefinedPools<T>.List.Return(list1);
                    }
                    break;
            }
        }
    }
}
