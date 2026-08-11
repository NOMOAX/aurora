using System;
using System.Threading;

namespace Aurora.CompilerServices
{
    /// <summary>
    /// 提供用于切换到目标 <see cref="SynchronizationContext"/> 的可等待上下文。
    /// </summary>
    public readonly struct SynchronizationContextAwaitable : IAwaitable
    {
        private readonly SynchronizationContext _synchronizationContext;

        private readonly CancellationToken _cancellationToken;

        /// <summary>
        /// 初始化 <see cref="SynchronizationContextAwaitable"/> 结构的新实例。
        /// </summary>
        /// <param name="synchronizationContext">同步上下文。</param>
        public SynchronizationContextAwaitable(SynchronizationContext synchronizationContext)
        {
            _synchronizationContext = synchronizationContext;
            _cancellationToken      = CancellationToken.None;
        }

        /// <summary>
        /// 初始化 <see cref="SynchronizationContextAwaitable"/> 结构的新实例。
        /// </summary>
        /// <param name="synchronizationContext">同步上下文。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        public SynchronizationContextAwaitable(
            SynchronizationContext synchronizationContext,
            CancellationToken      cancellationToken)
        {
            _synchronizationContext = synchronizationContext;
            _cancellationToken      = cancellationToken;
        }

        /// <inheritdoc />
        public IAwaiter GetAwaiter()
        {
            return new Awaiter(_synchronizationContext, _cancellationToken);
        }

        private readonly struct Awaiter : IAwaiter
        {
            private static readonly SendOrPostCallback RunAction = state => ((Action)state)();

            private readonly SynchronizationContext _synchronizationContext;

            private readonly CancellationToken _cancellationToken;

            internal Awaiter(SynchronizationContext synchronizationContext, CancellationToken cancellationToken)
            {
                _synchronizationContext = synchronizationContext;
                _cancellationToken      = cancellationToken;
            }

            public bool IsCompleted => IsCanceledOrDefaultInitialized || IsCurrentSynchronizationContext;

            private bool IsCanceledOrDefaultInitialized => IsCanceled || IsDefaultInitialized;

            private bool IsCurrentSynchronizationContext => SynchronizationContext.Current == _synchronizationContext;

            private bool IsCanceled => _cancellationToken.IsCancellationRequested;

            private bool IsDefaultInitialized => _synchronizationContext == null;

            public void OnCompleted(Action continuation)
            {
                InternalOnCompleted(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                InternalOnCompleted(continuation);
            }

            public void GetResult()
            {
                _cancellationToken.ThrowIfCancellationRequested();
            }

            private void InternalOnCompleted(Action continuation)
            {
                if (IsCanceledOrDefaultInitialized)
                {
                    continuation();
                }
                else if (IsCurrentSynchronizationContext)
                {
                    _synchronizationContext.Send(RunAction, continuation);
                }
                else
                {
                    _synchronizationContext.Post(RunAction, continuation);
                }
            }
        }
    }
}
