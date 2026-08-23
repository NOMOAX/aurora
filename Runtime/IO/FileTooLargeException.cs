using System.IO;

namespace Aurora.IO
{
    /// <summary>
    /// 在文件体积过大时引发的异常。
    /// </summary>
    public sealed class FileTooLargeException : IOException
    {
        /// <summary>
        /// 初始化 <see cref="FileTooLargeException"/> 类的新实例。
        /// </summary>
        public FileTooLargeException()
        {
        }

        /// <summary>
        /// 使用指定的错误消息初始化 <see cref="FileTooLargeException"/> 类的新实例。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public FileTooLargeException(string message) : base(message)
        {
        }
    }
}
