using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aurora.Threading
{
    /// <summary>
    /// An asynchronous barrier that blocks participants until all other participants have signaled.
    /// </summary>
    public class AsyncBarrier
    {
        private static readonly Action<object> Cancel = state =>
        {
            var (taskCompletionSource, cancellationToken) =
                (Tuple<TaskCompletionSource<VoidResult>, CancellationToken>)state;
            taskCompletionSource.TrySetCanceled(cancellationToken);
        };

        private readonly int _participantCount;

        private readonly Stack<Waiter> _waiters;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncBarrier"/> class.
        /// </summary>
        /// <param name="participants">The number of participants.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="participants"/> is less than 1.</exception>
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
        /// A participant signals that it is ready, and returns a task that completes when all other participants are ready.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that indicates that the participant is no longer interested in continuing to wait. Even so, the signaling behavior is not canceled.</param>
        /// <returns>The task that completes when the last participant calls this method.</returns>
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
                                      Cancel,
                                      Tuple.Create(taskCompletionSource, cancellationToken)
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
    }
}
