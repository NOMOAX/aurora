using System;
using System.Threading;

namespace Aurora.CompilerServices
{
    /// <summary>
    /// 提供用于切换到线程池线程的可等待上下文。
    /// </summary>
    public readonly struct ThreadPoolThreadAwaitable : IAwaitable
    {
        private readonly CancellationToken _cancellationToken;

        /// <summary>
        /// 初始化 <see cref="ThreadPoolThreadAwaitable"/> 结构的新实例。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        public ThreadPoolThreadAwaitable(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
        }

        /// <inheritdoc />
        public IAwaiter GetAwaiter()
        {
            return new Awaiter(_cancellationToken);
        }

        private readonly struct Awaiter : IAwaiter
        {
            private static readonly WaitCallback RunAction = state => ((Action) state)();

            private readonly CancellationToken _cancellationToken;

            internal Awaiter(CancellationToken cancellationToken)
            {
                _cancellationToken = cancellationToken;
            }

            public bool IsCompleted => InternalIsCompleted;

            private bool InternalIsCompleted => IsCanceled || IsThreadPoolThread || IsSingleThreadEnvironment;

            private bool IsCanceled => _cancellationToken.IsCancellationRequested;

            private static bool IsThreadPoolThread => Thread.CurrentThread.IsThreadPoolThread;

            private static bool IsSingleThreadEnvironment => AuroraEnvironment.IsSingleThreadEnvironment;

            public void OnCompleted(Action continuation)
            {
                OnCompleted(continuation, true);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                OnCompleted(continuation, false);
            }

            public void GetResult()
            {
                _cancellationToken.ThrowIfCancellationRequested();
            }

            private void OnCompleted(Action continuation, bool flowExecutionContext)
            {
                if (InternalIsCompleted)
                {
                    continuation();
                }
                else if (flowExecutionContext)
                {
                    ThreadPool.QueueUserWorkItem(RunAction, continuation);
                }
                else

                {
                    ThreadPool.UnsafeQueueUserWorkItem(RunAction, continuation);
                }
            }
        }
    }
}
