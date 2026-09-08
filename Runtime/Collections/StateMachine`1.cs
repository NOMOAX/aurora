using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Aurora.Collections
{
    /// <summary>
    /// A finite state machine.
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
    /// <example>
    /// <code>
    /// public sealed class MyStateOne : IState&lt;Type&gt;
    /// {
    ///     public Type Id => typeof(MyStateOne);
    ///     public void OnEnter(StateMachine&lt;Type&gt; stateMachine, IState&lt;Type&gt; from)
    ///     {
    ///         if (from == null)
    ///         {
    ///             Console.WriteLine($"[{nameof(MyStateOne)}.{nameof(OnEnter)}] NULL -> {Id}");
    ///         }
    ///         else
    ///         {
    ///             Console.WriteLine($"[{nameof(MyStateOne)}.{nameof(OnEnter)}] {from.Id} -> {Id}");
    ///         }
    ///         stateMachine.ScheduleTransitionTo(typeof(MyStateTwo));
    ///     }
    ///     public void OnExit(StateMachine&lt;Type&gt; stateMachine, IState&lt;Type&gt; to)
    ///     {
    ///         if (to != null)
    ///         {
    ///             Console.WriteLine($"[{nameof(MyStateOne)}.{nameof(OnExit)}] {Id} -> {to.Id}");
    ///         }
    ///         else
    ///         {
    ///             Console.WriteLine($"[{nameof(MyStateOne)}.{nameof(OnExit)}] {Id} -> NULL");
    ///         }
    ///     }
    /// }
    /// public sealed class MyStateTwo : IState&lt;Type&gt;
    /// {
    ///     public Type Id => typeof(MyStateTwo);
    ///     public void OnEnter(StateMachine&lt;Type&gt; stateMachine, IState&lt;Type&gt; from)
    ///     {
    ///         if (from == null)
    ///         {
    ///             Console.WriteLine($"[{nameof(MyStateTwo)}.{nameof(OnEnter)}] NULL -> {Id}");
    ///         }
    ///         else
    ///         {
    ///             Console.WriteLine($"[{nameof(MyStateTwo)}.{nameof(OnEnter)}] {from.Id} -> {Id}");
    ///         }
    ///         stateMachine.ScheduleTransitionToNull();
    ///     }
    ///     public void OnExit(StateMachine&lt;Type&gt; stateMachine, IState&lt;Type&gt; to)
    ///     {
    ///         if (to != null)
    ///         {
    ///             Console.WriteLine($"[{nameof(MyStateTwo)}.{nameof(OnExit)}] {Id} -> {to.Id}");
    ///         }
    ///         else
    ///         {
    ///             Console.WriteLine($"[{nameof(MyStateTwo)}.{nameof(OnExit)}] {Id} -> NULL");
    ///         }
    ///     }
    /// }
    /// // state machine owner code
    /// var stateMachine = new StateMachine&lt;Type&gt;();
    /// stateMachine.AddState(new MyStateOne());
    /// stateMachine.AddState(new MyStateTwo());
    /// stateMachine.ScheduleTransitionTo(typeof(MyStateOne));
    /// stateMachine.Update(); // output 1: [MyStateOne.OnEnter] NULL -> MyStateOne
    /// stateMachine.Update(); // output 2: [MyStateOne.OnExit] MyStateOne -> MyStateTwo
    ///                        // output 3: [MyStateTwo.OnEnter] MyStateOne -> MyStateTwo
    /// stateMachine.Update(); // output 4: [MyStateTwo.OnExit] MyStateTwo -> NULL
    /// var updated = stateMachine.Update(); // updated: false
    /// </code>
    /// </example>
    public class StateMachine<T>
    {
        private readonly Dictionary<T, IState<T>> _states;

        private bool _isEntering;

        private bool _isExiting;

        private IState<T> _currentState;

        private bool _isStateTransitionScheduled;

        private IState<T> _nextState;

        private Blackboard _blackboard;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachine{T}"/> class.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StateMachine()
        {
            _states = new Dictionary<T, IState<T>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachine{T}"/> class with the specified equality comparer.
        /// </summary>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> used to compare whether state identifiers are equal.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StateMachine(IEqualityComparer<T> comparer)
        {
            _states = new Dictionary<T, IState<T>>(comparer);
        }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        public IState<T> CurrentState
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _currentState;
        }

        /// <summary>
        /// Gets the blackboard.
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
        /// Gets the <see cref="IEqualityComparer{T}"/> used to compare whether state identifiers are equal.
        /// </summary>
        public IEqualityComparer<T> Comparer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _states.Comparer;
        }

        /// <summary>
        /// Adds the specified state to the <see cref="StateMachine{T}"/>.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="state"/>'s identifier is <see langword="null"/>, or the finite state machine already contains a state with the same identifier as <paramref name="state"/>, or a subclass implementation rejects adding <paramref name="state"/> to the finite state machine.</exception>
        public void AddState(IState<T> state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            var stateId = state.Id;
            if (stateId == null)
            {
                throw new ArgumentException("The state identifier cannot be null", nameof(state));
            }
            ThrowIfEnteringOrExiting();
            if (_states.ContainsKey(stateId))
            {
                throw new ArgumentException($"A state with identifier '{stateId}' already exists");
            }
            ThrowIfRejectState(state);
            _states.Add(stateId, state);
        }

        /// <summary>
        /// About to add the specified state to the <see cref="StateMachine{T}"/>; to reject this, a subclass may override this method and throw <see cref="ArgumentException"/> in its implementation.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <exception cref="ArgumentException">In a subclass implementation, <paramref name="state"/> is rejected from being added to the finite state machine.</exception>
        protected virtual void ThrowIfRejectState(IState<T> state)
        {
        }

        /// <summary>
        /// Removes the state with the specified identifier from the <see cref="StateMachine{T}"/>.
        /// </summary>
        /// <param name="stateId">The state identifier.</param>
        /// <returns><see langword="true"/> if the state with the specified identifier was found and removed; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stateId"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The finite state machine is entering or exiting a state.</exception>
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
        /// Makes the <see cref="StateMachine{T}"/> transition to the state with the specified identifier.
        /// </summary>
        /// <param name="stateId">The state identifier.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stateId"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The finite state machine is entering or exiting a state.</exception>
        /// <exception cref="ArgumentException">The finite state machine has no state with the identifier <paramref name="stateId"/>.</exception>
        /// <remarks>This method must not be called from within the finite state machine (for example, from <see cref="IState{T}.OnEnter"/> or <see cref="IState{T}.OnExit"/>). In this case, call <see cref="ScheduleTransitionTo(T)"/> instead.</remarks>
        public void TransitionTo(T stateId)
        {
            if (stateId == null)
            {
                throw new ArgumentException("The state identifier cannot be null", nameof(stateId));
            }
            ThrowIfEnteringOrExiting();
            InternalScheduleTransitionTo(stateId);
            InternalUpdate();
        }

        /// <summary>
        /// Schedules the <see cref="StateMachine{T}"/> to transition to the state with the specified identifier on the next execution of <see cref="Update"/>.
        /// </summary>
        /// <param name="stateId">The state identifier.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stateId"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The finite state machine is exiting a state.</exception>
        /// <exception cref="ArgumentException">The finite state machine has no state with the identifier <paramref name="stateId"/>.</exception>
        public void ScheduleTransitionTo(T stateId)
        {
            if (stateId == null)
            {
                throw new ArgumentException("The state identifier cannot be null", nameof(stateId));
            }
            ThrowIfExiting();
            InternalScheduleTransitionTo(stateId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalScheduleTransitionTo(T stateId)
        {
            if (!_states.TryGetValue(stateId, out var nextState))
            {
                throw new ArgumentException($"No state with identifier '{stateId}' exists");
            }
            _isStateTransitionScheduled = true;
            _nextState                  = nextState;
        }

        /// <summary>
        /// Makes the <see cref="StateMachine{T}"/> exit the current state.
        /// </summary>
        /// <exception cref="InvalidOperationException">The finite state machine is entering or exiting a state.</exception>
        /// <remarks>This method must not be called from within the finite state machine (for example, from <see cref="IState{T}.OnEnter"/> or <see cref="IState{T}.OnExit"/>). In this case, call <see cref="ScheduleTransitionToNull"/> instead.</remarks>
        public void TransitionToNull()
        {
            ThrowIfEnteringOrExiting();
            InternalScheduleTransitionToNull();
            InternalUpdate();
        }

        /// <summary>
        /// Schedules the <see cref="StateMachine{T}"/> to exit the current state on the next execution of <see cref="Update"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The finite state machine is exiting a state.</exception>
        public void ScheduleTransitionToNull()
        {
            ThrowIfExiting();
            InternalScheduleTransitionToNull();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalScheduleTransitionToNull()
        {
            _isStateTransitionScheduled = _currentState != null;
            _nextState                  = null;
        }

        /// <summary>
        /// Updates the <see cref="StateMachine{T}"/>.
        /// </summary>
        /// <returns><see langword="true"/> if a state transition was performed; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="InvalidOperationException">The finite state machine is entering or exiting a state.</exception>
        /// <remarks>This method must not be called from within the finite state machine (for example, from <see cref="IState{T}.OnEnter"/> or <see cref="IState{T}.OnExit"/>).</remarks>
        public bool Update()
        {
            ThrowIfEnteringOrExiting();
            return InternalUpdate();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool InternalUpdate()
        {
            if (!_isStateTransitionScheduled)
            {
                return false;
            }
            _isStateTransitionScheduled = false;
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
        /// Throws <see cref="InvalidOperationException"/> if the <see cref="StateMachine{T}"/> is entering or exiting a state.
        /// </summary>
        /// <exception cref="InvalidOperationException">The finite state machine is entering or exiting a state.</exception>
        /// <remarks>When a subclass adds a custom operation, it should call this method at the beginning of the operation to ensure that no state is entering or exiting.</remarks>
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
                throw new InvalidOperationException("The finite state machine is entering a state");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfExiting()
        {
            if (_isExiting)
            {
                throw new InvalidOperationException("The finite state machine is exiting a state");
            }
        }
    }
}
