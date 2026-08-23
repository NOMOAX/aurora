using System;

namespace Aurora
{
    /// <summary>
    /// For program members that do not support XML documentation comments.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public sealed class CommentAttribute : Attribute
    {
        /// <summary>
        /// A comment.
        /// </summary>
        public readonly string Comment;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentAttribute"/> class.
        /// </summary>
        /// <param name="comment">A comment.</param>
        public CommentAttribute(string comment)
        {
            Comment = comment;
        }
    }
}
