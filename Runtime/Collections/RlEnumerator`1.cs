using System;
using System.Collections.Generic;
using Aurora.Pooling;

namespace Aurora.Collections
{
    /// <summary>
    /// 广度优先地，按照“从上层到下层，然后每层中倒序枚举各结点“的规则，枚举树的结点。
    /// </summary>
    /// <typeparam name="T">树的结点的类型。</typeparam>
    public sealed class RlEnumerator<T> : BreadthFirstEnumerator<T> where T : class
    {
        /// <summary>
        /// 初始化 <see cref="RlEnumerator{T}"/> 类的新实例。
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
