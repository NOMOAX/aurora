using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Aurora.Collections
{
    /// <summary>
    /// 有限状态机。
    /// </summary>
    /// <typeparam name="T">状态的标识符的类型。</typeparam>
    public class StateMachine<T>
    {
        private readonly Dictionary<T, IState<T>> _states;

        private bool _isEntering;

        private bool _isExiting;

        private IState<T> _currentState;

        private bool _hasNextState;

        private IState<T> _nextState;

        private Blackboard _blackboard;

        /// <summary>
        /// 初始化 <see cref="StateMachine{T}"/> 类的新实例。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StateMachine()
        {
            _states = new Dictionary<T, IState<T>>();
        }

        /// <summary>
        /// 使用指定的相等性比较器，初始化 <see cref="StateMachine{T}"/> 类的新实例。
        /// </summary>
        /// <param name="comparer">用于比较状态的标识符是否相等的 <see cref="IEqualityComparer{T}"/>。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StateMachine(IEqualityComparer<T> comparer)
        {
            _states = new Dictionary<T, IState<T>>(comparer);
        }

        /// <summary>
        /// 获取当前状态。
        /// </summary>
        public IState<T> CurrentState
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _currentState;
        }

        /// <summary>
        /// 获取黑板。
        /// </summary>
        public Blackboard Blackboard
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_blackboard == null)
                {
                    Interlocked.CompareExchange(ref _blackboard, new Blackboard(), null);
                }
                return _blackboard;
            }
        }

        /// <summary>
        /// 获取用于比较状态的标识符是否相等的 <see cref="IEqualityComparer{T}"/>。
        /// </summary>
        public IEqualityComparer<T> Comparer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _states.Comparer;
        }

        /// <summary>
        /// 将指定的状态添加到 <see cref="StateMachine{T}"/> 中。
        /// </summary>
        /// <param name="state">状态。</param>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentException"><paramref name="state"/> 的标识符为 <see langword="null"/>，或者有限状态机已含有与 <paramref name="state"/> 具有相同标识符的状态，或者在子类的实现中拒绝添加 <paramref name="state"/> 到有限状态机。</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddState(IState<T> state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            var stateId = state.Id;
            if (stateId == null)
            {
                throw new ArgumentException("状态标识符不能为空", nameof(state));
            }
            ThrowIfEnteringOrExiting();
            if (_states.ContainsKey(stateId))
            {
                throw new ArgumentException($"已经存在标识符为 {stateId} 的状态");
            }
            ThrowIfRejectState(state);
            _states.Add(stateId, state);
        }

        /// <summary>
        /// 即将将指定的状态添加到 <see cref="StateMachine{T}"/>；若拒绝此行为，子类可重写此方法并在实现中抛出 <see cref="ArgumentException"/> 异常。
        /// </summary>
        /// <param name="state">状态。</param>
        /// <exception cref="ArgumentException">在子类的实现中，拒绝添加 <paramref name="state"/> 到有限状态机。</exception>
        protected virtual void ThrowIfRejectState(IState<T> state)
        {
        }

        /// <summary>
        /// 将具有指定标识符的状态从 <see cref="StateMachine{T}"/> 中移除。
        /// </summary>
        /// <param name="stateId">状态标识符。</param>
        /// <returns>如果成功找到并移除具有指定标识符的状态，则为 <see langword="true"/> ；否则为 <see langword="false"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stateId"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="InvalidOperationException">有限状态机正在进入或退出状态。</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveState(T stateId)
        {
            if (stateId == null)
            {
                throw new ArgumentNullException(nameof(stateId));
            }
            ThrowIfEnteringOrExiting();
            if (!_states.Remove(stateId))
            {
                return false;
            }
            if (_currentState != null && _states.Comparer.Equals(_currentState.Id, stateId))
            {
                _currentState = null;
            }
            if (_nextState != null && _states.Comparer.Equals(_nextState.Id, stateId))
            {
                _nextState = null;
            }
            return true;
        }

        /// <summary>
        /// 让 <see cref="StateMachine{T}"/> 切换到具有指定标识符的状态。
        /// </summary>
        /// <param name="stateId">状态标识符。</param>
        /// <exception cref="ArgumentNullException"><paramref name="stateId"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="InvalidOperationException">有限状态机正在进入或退出状态。</exception>
        /// <exception cref="ArgumentException">有限状态机不含有标识符为 <paramref name="stateId"/> 的状态。</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TransitionTo(T stateId)
        {
            if (stateId == null)
            {
                throw new ArgumentException("状态标识符不能为空", nameof(stateId));
            }
            ThrowIfEnteringOrExiting();
            InternalTransitionToDuringNextUpdate(stateId);
            InternalUpdate();
        }

        /// <summary>
        /// 让 <see cref="StateMachine{T}"/> 退出当前状态。
        /// </summary>
        /// <exception cref="InvalidOperationException">有限状态机正在进入或退出状态。</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TransitionToNull()
        {
            ThrowIfEnteringOrExiting();
            InternalTransitionToNullDuringNextUpdate();
            InternalUpdate();
        }

        /// <summary>
        /// 安排 <see cref="StateMachine{T}"/> 在下一次执行 <see cref="Update"/> 时切换到具有指定标识符的状态。
        /// </summary>
        /// <param name="stateId">状态标识符。</param>
        /// <exception cref="ArgumentNullException"><paramref name="stateId"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="InvalidOperationException">有限状态机正在退出状态。</exception>
        /// <exception cref="ArgumentException">有限状态机不含有标识符为 <paramref name="stateId"/> 的状态。</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TransitionToDuringNextUpdate(T stateId)
        {
            if (stateId == null)
            {
                throw new ArgumentException("状态标识符不能为空", nameof(stateId));
            }
            ThrowIfExiting();
            InternalTransitionToDuringNextUpdate(stateId);
        }

        /// <summary>
        /// 安排 <see cref="StateMachine{T}"/> 在下一次执行 <see cref="Update"/> 时退出当前状态。
        /// </summary>
        /// <exception cref="InvalidOperationException">有限状态机正在退出状态。</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TransitionToNullDuringNextUpdate()
        {
            ThrowIfExiting();
            InternalTransitionToNullDuringNextUpdate();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalTransitionToDuringNextUpdate(T stateId)
        {
            if (!_states.TryGetValue(stateId, out var nextState))
            {
                throw new ArgumentException($"不含有标识符为 {stateId} 的状态");
            }
            _hasNextState = true;
            _nextState    = nextState;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalTransitionToNullDuringNextUpdate()
        {
            _hasNextState = _currentState != null;
            _nextState    = null;
        }

        /// <summary>
        /// 更新 <see cref="StateMachine{T}"/>。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">有限状态机正在进入或退出状态。</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Update()
        {
            ThrowIfEnteringOrExiting();
            return InternalUpdate();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool InternalUpdate()
        {
            if (!_hasNextState)
            {
                return false;
            }
            _hasNextState = false;
            var currentState = _currentState;
            var nextState    = _nextState;
            _nextState = null;
            if (_currentState != null)
            {
                _isExiting = true;
                try
                {
                    _currentState.OnExit(this, nextState);
                }
                finally
                {
                    _isExiting = false;
                }
            }
            _currentState = nextState;
            if (_currentState != null)
            {
                _isEntering = true;
                try
                {
                    _currentState.OnEnter(this, currentState);
                }
                finally
                {
                    _isEntering = false;
                }
            }
            return true;
        }

        /// <summary>
        /// 若 <see cref="StateMachine{T}"/> 正在进入或退出状态，则抛出 <see cref="InvalidOperationException"/> 异常。
        /// </summary>
        /// <exception cref="InvalidOperationException">有限状态机正在进入或退出状态。</exception>
        /// <remarks>子类添加自定义操作时，应在操作开头调用此方法，以确保没有正在进入或退出状态。</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ThrowIfEnteringOrExiting()
        {
            ThrowIfEntering();
            ThrowIfExiting();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfEntering()
        {
            if (_isEntering)
            {
                throw new InvalidOperationException("有限状态机正在进入状态");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfExiting()
        {
            if (_isExiting)
            {
                throw new InvalidOperationException("有限状态机正在退出状态");
            }
        }
    }
}
