using System;

namespace Aurora
{
    /// <summary>
    /// 使用空对象模式实现 <see cref="IDisposable"/>。
    /// </summary>
    public sealed class NullDisposable : IDisposable
    {
        /// <summary>
        /// 获取单一实例。
        /// </summary>
        public static NullDisposable Instance { get; } = new();

        private NullDisposable()
        {
        }

        void IDisposable.Dispose()
        {
        }
    }
}
