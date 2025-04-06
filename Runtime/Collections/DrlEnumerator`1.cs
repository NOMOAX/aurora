using System;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// 深度优先、递归地，按照“先枚举根结点，后倒序枚举各个子结点”的规则，枚举树的结点。
    /// </summary>
    /// <typeparam name="T">树的结点的类型。</typeparam>
    public sealed class DrlEnumerator<T> : DepthFirstDataFirstEnumerator<T> where T : class
    {
        /// <summary>
        /// 初始化 <see cref="DrlEnumerator{T}"/> 类的新实例。
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
