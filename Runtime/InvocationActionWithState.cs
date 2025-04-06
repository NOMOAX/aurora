using System;

namespace Aurora
{
    /// <summary>
    /// 有一个 <see cref="object"/> 类型的参数且不返回值的调用。
    /// </summary>
    public sealed class InvocationActionWithState : Invocation
    {
        private readonly Action<object> _actionWithState;

        private readonly object _state;

        /// <summary>
        /// 初始化 <see cref="InvocationActionWithState"/> 类的新实例。
        /// </summary>
        /// <param name="actionWithState">有一个 <see cref="object"/> 类型的参数且不返回值的方法。</param>
        /// <param name="state">由 <paramref name="actionWithState"/> 使用的参数。</param>
        /// <exception cref="ArgumentNullException"><paramref name="actionWithState"/> 为 <see langword="null"/>。</exception>
        public InvocationActionWithState(Action<object> actionWithState, object state)
        {
            _actionWithState = actionWithState ?? throw new ArgumentNullException(nameof(actionWithState));
            _state           = state;
        }

        /// <inheritdoc />
        public override void Invoke()
        {
            _actionWithState(_state);
        }
    }
}
