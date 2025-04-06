using System;

namespace Aurora
{
    /// <summary>
    /// 有一个 <see cref="object"/> 类型的参数且返回值的调用。
    /// </summary>
    /// <typeparam name="TResult">返回值的类型。</typeparam>
    public sealed class InvocationFuncWithState<TResult> : Invocation<TResult>
    {
        private readonly Func<object, TResult> _funcWithState;

        private readonly object _state;

        /// <summary>
        /// 初始化 <see cref="InvocationFuncWithState{TResult}"/> 类的新实例。
        /// </summary>
        /// <param name="funcWithState">有一个 <see cref="object"/> 类型的参数且返回值的方法。</param>
        /// <param name="state">由 <paramref name="funcWithState"/> 使用的参数。</param>
        /// <exception cref="ArgumentNullException"><paramref name="funcWithState"/> 为 <see langword="null"/>。</exception>
        public InvocationFuncWithState(Func<object, TResult> funcWithState, object state)
        {
            _funcWithState = funcWithState ?? throw new ArgumentNullException(nameof(funcWithState));
            _state         = state;
        }

        /// <inheritdoc />
        public override TResult Invoke()
        {
            return _funcWithState(_state);
        }
    }
}
