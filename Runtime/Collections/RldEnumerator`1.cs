using System;
using System.Collections.Generic;
using Aurora.Pooling;

namespace Aurora.Collections
{
    /// <summary>
    /// Depth-first, recursively enumerates the tree's nodes following the rule "enumerate each child node in reverse order first, then enumerate the root node".
    /// </summary>
    /// <typeparam name="T">The type of the tree's nodes.</typeparam>
    public sealed class RldEnumerator<T> : DepthFirstDataLastEnumerator<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RldEnumerator{T}"/> class.
        /// </summary>
        /// <inheritdoc />
        public RldEnumerator(
            T                       rootNode,
            Func<T, IEnumerable<T>> getChildrenFunc,
            Func<T, object, bool>   validateFunc  = null,
            object                  validateState = null) : base(rootNode, getChildrenFunc, validateFunc, validateState)
        {
        }

        /// <inheritdoc />
        protected override void PushChildren(Stack<T> stack, IEnumerable<T> children)
        {
            switch (children)
            {
                case IReadOnlyList<T> readOnlyList:
                    for (var i = readOnlyList.Count - 1; i >= 0; i--)
                    {
                        stack.Push(readOnlyList[i]);
                    }
                    break;
                case IList<T> list:
                    for (var i = list.Count - 1; i >= 0; i--)
                    {
                        stack.Push(list[i]);
                    }
                    break;
                default:
                    var list1 = PredefinedPools<T>.List.Get();
                    try
                    {
                        list1.AddRange(children);
                        for (var i = list1.Count - 1; i >= 0; i--)
                        {
                            stack.Push(list1[i]);
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
