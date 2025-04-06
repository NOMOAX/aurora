namespace Aurora.Collections
{
    /// <summary>
    /// （有限状态机中的）状态。
    /// </summary>
    /// <typeparam name="T">状态的标识符的类型。</typeparam>
    public interface IState<T>
    {
        /// <summary>
        /// 获取这个 <see cref="IState{T}"/> 的标识符。
        /// </summary>
        /// <remarks>返回值不应该为 <see langword="null"/>。</remarks>
        T Id { get; }

        /// <summary>
        /// 有限状态机进入这个状态。
        /// </summary>
        /// <param name="stateMachine">有限状态机。</param>
        /// <param name="from">上一个状态。</param>
        void OnEnter(StateMachine<T> stateMachine, IState<T> from);

        /// <summary>
        /// 有限状态机退出这个状态。
        /// </summary>
        /// <param name="stateMachine">有限状态机。</param>
        /// <param name="to">下一个状态。</param>
        void OnExit(StateMachine<T> stateMachine, IState<T> to);
    }
}
