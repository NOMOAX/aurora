namespace Aurora.Collections
{
    /// <summary>
    /// 枚举树结构的顺序。
    /// </summary>
    public enum TreeEnumOrder
    {
        /// <summary>
        /// 仅执行简单枚举，即枚举直接子结点。
        /// </summary>
        Default,

        /// <summary>
        /// 广度优先地，从上层到下层，然后每层中枚举各结点。
        /// </summary>
        BreadthFirstLr,

        /// <summary>
        /// 广度优先地，从上层到下层，然后每层中倒序枚举各结点。
        /// </summary>
        BreadthFirstRl,

        /// <summary>
        /// 深度优先、递归地，先枚举根结点，后枚举各个子结点。
        /// </summary>
        DepthFirstDlr,

        /// <summary>
        /// 深度优先、递归地，先枚举根结点，后倒序枚举各个子结点。
        /// </summary>
        DepthFirstDrl,

        /// <summary>
        /// 深度优先、递归地，先枚举各个子结点，后枚举根结点。
        /// </summary>
        DepthFirstLrd,

        /// <summary>
        /// 深度优先、递归地，先倒序枚举各个子结点，后枚举根结点。
        /// </summary>
        DepthFirstRld
    }
}
