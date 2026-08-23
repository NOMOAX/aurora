using System;

namespace Aurora
{
    /// <summary>
    /// 无参数且不返回值的调用。
    /// </summary>
    public sealed class InvocationAction : Invocation
    {
        private readonly Action _action;

        /// <summary>
        /// 初始化 <see cref="InvocationAction"/> 类的新实例。
        /// </summary>
        /// <param name="action">无参数且不返回值的方法。</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> 为 <see langword="null"/>。</exception>
        public InvocationAction(Action action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        /// <inheritdoc />
        public override void Invoke()
        {
            _action();
        }
    }
}
