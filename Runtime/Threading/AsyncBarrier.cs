using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Aurora.Threading
{
    /// <summary>
    /// 异步屏障，它阻塞参与者，直到所有其他参与者都发出信号。
    /// </summary>
    public class AsyncBarrier
    {
        private static readonly Action<object> ActionCancel = Cancel;

        private readonly int _participantCount;

        private readonly Stack<Waiter> _waiters;

        /// <summary>
        /// 初始化 <see cref="AsyncBarrier"/> 类的新实例。
        /// </summary>
        /// <param name="participants">参与者的数量。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="participants"/> 小于 1。</exception>
        public AsyncBarrier(int participants)
        {
            if (participants < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(participants));
            }
            _participantCount = participants;
            _waiters          = new Stack<Waiter>(participants - 1);
        }

        /// <summary>
        /// 一个参与者告知其已准备就绪，并且返回一个当所有其他参与者都准备就绪时完成的任务。
        /// </summary>
        /// <param name="cancellationToken">一个表示参与者对继续等待失去兴趣的取消令牌。即便如此，告知行为不会被取消。</param>
        /// <returns>当最后一个参与者调用此方法时完成的方法。</returns>
        public Task SignalAndWait(CancellationToken cancellationToken = default)
        {
            lock (_waiters)
            {
                if (_waiters.Count + 1 < _participantCount)
                {
                    var taskCompletionSource =
                        new TaskCompletionSource<VoidResult>(TaskCreationOptions.RunContinuationsAsynchronously);
                    var ctr = cancellationToken.CanBeCanceled
                                  ? cancellationToken.Register(
                                      ActionCancel,
                                      new State(taskCompletionSource, cancellationToken)
                                  )
                                  : default;
                    _waiters.Push(new Waiter(taskCompletionSource, ctr));
                    return taskCompletionSource.Task;
                }

                while (_waiters.Count > 0)
                {
                    var waiter = _waiters.Pop();
                    waiter.CompletionSource.TrySetResult(new VoidResult());
                    waiter.CancellationRegistration.Dispose();
                }
                return cancellationToken.IsCancellationRequested
                           ? Task.FromCanceled(cancellationToken)
                           : Task.CompletedTask;
            }
        }

        private static void Cancel(object state)
        {
            Cancel((State) state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Cancel(State state)
        {
            var taskCompletionSource = state.TaskCompletionSource;
            var cancellationToken    = state.Cancellation;
            taskCompletionSource.TrySetCanceled(cancellationToken);
        }

        private struct Waiter
        {
            internal readonly TaskCompletionSource<VoidResult> CompletionSource;

            internal readonly CancellationTokenRegistration CancellationRegistration;

            public Waiter(
                TaskCompletionSource<VoidResult> completionSource,
                CancellationTokenRegistration    cancellationRegistration)
            {
                CompletionSource         = completionSource;
                CancellationRegistration = cancellationRegistration;
            }
        }

        private sealed class State
        {
            internal readonly TaskCompletionSource<VoidResult> TaskCompletionSource;

            internal readonly CancellationToken Cancellation;

            internal State(TaskCompletionSource<VoidResult> taskCompletionSource, CancellationToken cancellation)
            {
                TaskCompletionSource = taskCompletionSource;
                Cancellation         = cancellation;
            }
        }
    }
}
