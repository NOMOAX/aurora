using System;

namespace Aurora
{
    /// <summary>
    /// 在遇到了意外情况时引发的异常。
    /// </summary>
    public class UnexpectedException : Exception
    {
        /// <summary>
        /// 初始化 <see cref="UnexpectedException"/> 类的新实例。
        /// </summary>
        public UnexpectedException() : base("遇到了意外情况")
        {
        }

        /// <summary>
        /// 用指定的错误消息初始化 <see cref="UnexpectedException"/> 类的新实例。
        /// </summary>
        /// <inheritdoc />
        public UnexpectedException(string message) : base(message)
        {
        }

        /// <summary>
        /// 使用指定的错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="UnexpectedException"/> 类的新实例。
        /// </summary>
        /// <inheritdoc />
        public UnexpectedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
