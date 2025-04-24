using System;

namespace Aurora
{
    /// <summary>
    /// 用于不支持 XML 文档注释的程序成员。
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public sealed class CommentAttribute : Attribute
    {
        /// <summary>
        /// 注释。
        /// </summary>
        public readonly string Comment;

        /// <summary>
        /// 初始化 <see cref="CommentAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="comment">注释。</param>
        public CommentAttribute(string comment)
        {
            Comment = comment;
        }
    }
}
