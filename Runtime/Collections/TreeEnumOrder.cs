namespace Aurora.Collections
{
    /// <summary>
    /// The order in which a tree structure is enumerated.
    /// </summary>
    public enum TreeEnumOrder
    {
        /// <summary>
        /// Performs only a simple enumeration, i.e. enumerates direct child nodes.
        /// </summary>
        Default,

        /// <summary>
        /// Breadth-first, from upper levels to lower levels, then enumerates each node within each level.
        /// </summary>
        BreadthFirstLr,

        /// <summary>
        /// Breadth-first, from upper levels to lower levels, then enumerates each node in reverse order within each level.
        /// </summary>
        BreadthFirstRl,

        /// <summary>
        /// Depth-first, recursively enumerates the root node first, then each child node.
        /// </summary>
        DepthFirstDlr,

        /// <summary>
        /// Depth-first, recursively enumerates the root node first, then each child node in reverse order.
        /// </summary>
        DepthFirstDrl,

        /// <summary>
        /// Depth-first, recursively enumerates each child node first, then the root node.
        /// </summary>
        DepthFirstLrd,

        /// <summary>
        /// Depth-first, recursively enumerates each child node in reverse order first, then the root node.
        /// </summary>
        DepthFirstRld
    }
}
