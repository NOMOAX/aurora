using System;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// 广度优先地，按照“从上层到下层，然后每层中枚举各结点“的规则，枚举树的结点。
    /// </summary>
    /// <typeparam name="T">树的结点的类型。</typeparam>
    public sealed class LrEnumerator<T> : BreadthFirstEnumerator<T> where T : class
    {
        /// <summary>
        /// 初始化 <see cref="LrEnumerator{T}"/> 类的新实例。
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
