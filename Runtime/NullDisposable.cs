using System;
using System.Runtime.CompilerServices;

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
        public static NullDisposable Instance
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        } = new NullDisposable();

        private NullDisposable()
        {
        }

        void IDisposable.Dispose()
        {
        }
    }
}
