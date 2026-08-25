using System;
using System.IO;

namespace Aurora.IO
{
    /// <summary>
    /// Performs operations on <see cref="string"/> instances that contain file or directory path information. These operations are performed in a cross-platform manner.
    /// </summary>
    public static class PathUtility
    {
        /// <summary>
        /// Replaces backslashes ("\") with forward slashes ("/").
        /// </summary>
        /// <param name="path">The path string from which backslashes will be replaced with forward slashes.</param>
        /// <returns>The replaced path string.</returns>
        public static string ReplaceBackslashWithForwardSlash(string path)
        {
            return path?.Replace('\\', '/');
        }

        /// <summary>
        /// Returns a relative path from one path to another.
        /// </summary>
        /// <param name="relativeTo">The source path the result should be relative to. This path is always considered to be a directory.</param>
        /// <param name="path">The destination path.</param>
        /// <param name="relationship">
        /// When this method returns, contains the relationship of <paramref name="relativeTo"/> and <paramref name="path"/>.
        /// <list type="table">
        /// <listheader><term>value</term><description>description</description></listheader>
        /// <item><term><see cref="PathRelationship.IsChildOf"/></term><description><paramref name="path"/> is a child of <paramref name="relativeTo"/></description></item>
        /// <item><term><see cref="PathRelationship.IsEqualTo"/></term><description><paramref name="path"/> is equal to <paramref name="relativeTo"/></description></item>
        /// <item><term><see cref="PathRelationship.IsNeitherChildOfNorEqualTo"/></term><description><paramref name="path"/> is neither a child of <paramref name="relativeTo"/> nor equal to <paramref name="relativeTo"/></description></item>
        /// <item><term><see cref="PathRelationship.AreUnrelated"/></term><description><paramref name="path"/> and <paramref name="relativeTo"/> don't share the same root</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// See below:
        /// <list type="table">
        /// <listheader><term>situation</term><description>description</description></listheader>
        /// <item><term><paramref name="path"/> is a child of <paramref name="relativeTo"/></term><description>the relative path</description></item>
        /// <item><term><paramref name="path"/> is equal to <paramref name="relativeTo"/></term><description>"."</description></item>
        /// <item><term><paramref name="path"/> is neither a child of <paramref name="relativeTo"/> nor equal to <paramref name="relativeTo"/></term><description>the relative path</description></item>
        /// <item><term><paramref name="path"/> and <paramref name="relativeTo"/> don't share the same root</term><description><see langword="null"/></description></item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="relativeTo"/> or <paramref name="path"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="relativeTo"/> or <paramref name="path"/> is effectively empty.</exception>
        /// <exception cref="PathTooLongException"><paramref name="relativeTo"/>, <paramref name="path"/>, or both exceed the system-defined maximum length.</exception>
        public static string GetRelativePath(string relativeTo, string path, out PathRelationship relationship)
        {
            if (relativeTo == null)
            {
                throw new ArgumentNullException(nameof(relativeTo));
            }
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            var normalizedRelativeTo = ReplaceBackslashWithForwardSlash(Path.GetFullPath(relativeTo));
            var normalizedPath       = ReplaceBackslashWithForwardSlash(Path.GetFullPath(path));
            var normalizedRelativePath =
                ReplaceBackslashWithForwardSlash(Path.GetRelativePath(normalizedRelativeTo, normalizedPath));

            if (normalizedRelativePath is ".")
            {
                relationship = PathRelationship.IsEqualTo;
                return ".";
            }
            if (normalizedRelativePath == normalizedPath)
            {
                relationship = PathRelationship.AreUnrelated;
                return null;
            }
            relationship =
                normalizedRelativePath is ".." || normalizedRelativePath.StartsWith("../", StringComparison.Ordinal)
                    ? PathRelationship.IsNeitherChildOfNorEqualTo
                    : PathRelationship.IsChildOf;
            return normalizedRelativePath;
        }
    }
}
