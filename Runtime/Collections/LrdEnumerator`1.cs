using System;
using System.Collections;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// 深度优先、递归地，按照“先枚举各个子结点，后枚举根结点”的规则，枚举树的结点。
    /// </summary>
    /// <typeparam name="T">树的结点的类型。</typeparam>
    /// <remarks>此枚举器将在构造函数中和 <see cref="IEnumerator.Reset"/> 中计算完每一步的枚举结果，请注意此行为导致的性能消耗。</remarks>
    public sealed class LrdEnumerator<T> : DepthFirstDataLastEnumerator<T> where T : class
    {
        /// <summary>
        /// 初始化 <see cref="LrdEnumerator{T}"/> 类的新实例。
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
