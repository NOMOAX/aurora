namespace Aurora.IO
{
    /// <summary>
    /// Represents the relationship between the <c>relativeTo</c> and <c>path</c> parameters of the <see cref="PathUtility.GetRelativePath"/> method.
    /// </summary>
    public enum PathRelationship
    {
        /// <summary>
        /// Indicates that <c>path</c> is a child of <c>relativeTo</c>.
        /// </summary>
        IsChildOf,

        /// <summary>
        /// Indicates that <c>path</c> is equal to <c>relativeTo</c>.
        /// </summary>
        IsEqualTo,

        /// <summary>
        /// Indicates that <c>path</c> is neither a child of <c>relativeTo</c> nor equal to <c>relativeTo</c>.
        /// </summary>
        IsNeitherChildOfNorEqualTo,

        /// <summary>
        /// Indicates that <c>path</c> and <c>relativeTo</c> don't share the same root.
        /// </summary>
        AreUnrelated
    }
}
