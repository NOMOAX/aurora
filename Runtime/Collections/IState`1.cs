using System;

namespace Aurora.Collections
{
    /// <summary>
    /// A state (in a finite state machine).
    /// </summary>
    /// <typeparam name="T">The type of the state's identifier.</typeparam>
    /// <remarks>
    /// Types recommended for use as the state's identifier:
    /// <list type="bullet">
    /// <item><description><see cref="Type"/> (most recommended)</description></item>
    /// <item><description>an enumeration type</description></item>
    /// <item><description><see cref="string"/></description></item>
    /// </list>
    /// </remarks>
    public interface IState<T>
    {
        /// <summary>
        /// Gets the identifier of this <see cref="IState{T}"/>.
        /// </summary>
        /// <remarks>The return value should not be <see langword="null"/>.</remarks>
        T Id { get; }

        /// <summary>
        /// The finite state machine enters this state.
        /// </summary>
        /// <param name="stateMachine">The finite state machine.</param>
        /// <param name="from">The previous state.</param>
        void OnEnter(StateMachine<T> stateMachine, IState<T> from);

        /// <summary>
        /// The finite state machine exits this state.
        /// </summary>
        /// <param name="stateMachine">The finite state machine.</param>
        /// <param name="to">The next state.</param>
        void OnExit(StateMachine<T> stateMachine, IState<T> to);
    }
}
