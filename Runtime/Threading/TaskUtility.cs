using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Aurora.Threading
{
    /// <summary>
    /// Provides utility methods for the <see cref="Task"/> class.
    /// </summary>
    public static class TaskUtility
    {
        /// <summary>
        /// Awaits the <see cref="Task"/> in a new context.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
        public static async void BeginAwait(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            await task.ConfigureAwait(false);
        }

        /// <summary>
        /// If the <see cref="Task"/> is faulted, throws the underlying exception that caused the fault; if the <see cref="Task"/> is canceled, throws <see cref="TaskCanceledException"/>.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
        /// <exception cref="TaskCanceledException"><paramref name="task"/> is canceled.</exception>
        public static void ThrowIfFaultedOrCanceled(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            if (task.IsFaulted)
            {
                throw task.Exception!.GetBaseException();
            }
            if (task.IsCanceled)
            {
                throw new TaskCanceledException(task);
            }
        }

        /// <summary>
        /// If the <see cref="Task"/> is faulted, returns the underlying exception that caused the fault; otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns>If the <see cref="Task"/> is faulted, the underlying exception that caused the fault; otherwise, <see langword="null"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
        public static Exception GetBaseException(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            return task.IsFaulted ? GetFaultedTaskBaseException(task) : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Exception GetFaultedTaskBaseException(Task faultedTask)
        {
            return faultedTask.Exception!.GetBaseException();
        }

        /// <summary>
        /// If the target task is faulted or canceled, correspondingly faults or cancels the task completion source and returns <see langword="true"/>; otherwise, returns <see langword="false"/>.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="taskCompletionSource">The task completion source.</param>
        /// <typeparam name="TResult">The type parameter of the task completion source.</typeparam>
        /// <returns><see langword="true"/> if the target task is faulted or canceled and the task completion source is correspondingly set to faulted or canceled; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> or <paramref name="taskCompletionSource"/> is <see langword="null"/>.</exception>
        public static bool HandleFaultsAndCancellation<TResult>(
            Task                          task,
            TaskCompletionSource<TResult> taskCompletionSource)
        {
            return InternalHandleFaultsAndCancellation(task, taskCompletionSource, CancellationToken.None);
        }

        /// <summary>
        /// If the target task is faulted or canceled, correspondingly faults or cancels the task completion source and returns <see langword="true"/>; otherwise, returns <see langword="false"/>.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="taskCompletionSource">The task completion source.</param>
        /// <param name="cancellationToken">The cancellation token. It is the reason the target task is canceled.</param>
        /// <typeparam name="TResult">The type parameter of the task completion source.</typeparam>
        /// <returns><see langword="true"/> if the target task is faulted or canceled and the task completion source is correspondingly set to faulted or canceled; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> or <paramref name="taskCompletionSource"/> is <see langword="null"/>.</exception>
        public static bool HandleFaultsAndCancellation<TResult>(
            Task                          task,
            TaskCompletionSource<TResult> taskCompletionSource,
            CancellationToken             cancellationToken)
        {
            return InternalHandleFaultsAndCancellation(task, taskCompletionSource, cancellationToken);
        }

        private static bool InternalHandleFaultsAndCancellation<TResult>(
            Task                          task,
            TaskCompletionSource<TResult> taskCompletionSource,
            CancellationToken             cancellationToken)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            if (taskCompletionSource == null)
            {
                throw new ArgumentNullException(nameof(taskCompletionSource));
            }
            if (task.IsFaulted)
            {
                taskCompletionSource.TrySetException(GetFaultedTaskBaseException(task));
                return true;
            }
            if (task.IsCanceled)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    taskCompletionSource.TrySetCanceled(cancellationToken);
                }
                else
                {
                    taskCompletionSource.TrySetCanceled();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a continuation task that executes synchronously when the task completes.
        /// </summary>
        /// <param name="task">The task to which the continuation task is attached.</param>
        /// <param name="continuationAction">The action to run when the task completes.</param>
        /// <returns>A new continuation <see cref="Task"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
        /// <remarks>If the task has already completed, the continuation task executes immediately.</remarks>
        /// <seealso cref="Task.ContinueWith(System.Action{System.Threading.Tasks.Task})"/>
        public static Task ContinueWithSynchronously(Task task, Action<Task> continuationAction)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            return task.ContinueWith(
                continuationAction,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default
            );
        }

        /// <summary>
        /// Creates a continuation task that executes synchronously when the task completes.
        /// </summary>
        /// <param name="task">The task to which the continuation task is attached.</param>
        /// <param name="continuationAction">The action to run when the task completes.</param>
        /// <param name="state">The state to pass to <paramref name="continuationAction"/>.</param>
        /// <returns>A new continuation <see cref="Task"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
        /// <remarks>If the task has already completed, the continuation task executes immediately.</remarks>
        /// <seealso cref="Task.ContinueWith(System.Action{System.Threading.Tasks.Task,object},object)"/>
        public static Task ContinueWithSynchronously(Task task, Action<Task, object> continuationAction, object state)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            return task.ContinueWith(
                continuationAction,
                state,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default
            );
        }

        /// <summary>
        /// Creates a continuation task that executes synchronously when the task completes.
        /// </summary>
        /// <param name="task">The task to which the continuation task is attached.</param>
        /// <param name="continuationFunction">The function to run when the task completes.</param>
        /// <returns>A new continuation <see cref="Task{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
        /// <remarks>If the task has already completed, the continuation task executes immediately.</remarks>
        /// <seealso cref="Task.ContinueWith{TResult}(System.Func{System.Threading.Tasks.Task,TResult})"/>
        public static Task<TResult> ContinueWithSynchronously<TResult>(
            Task                task,
            Func<Task, TResult> continuationFunction)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            return task.ContinueWith(
                continuationFunction,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default
            );
        }

        /// <summary>
        /// Creates a continuation task that executes synchronously when the task completes.
        /// </summary>
        /// <param name="task">The task to which the continuation task is attached.</param>
        /// <param name="continuationFunction">The function to run when the task completes.</param>
        /// <param name="state">The state to pass to <paramref name="continuationFunction"/>.</param>
        /// <returns>A new continuation <see cref="Task{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
        /// <remarks>If the task has already completed, the continuation task executes immediately.</remarks>
        /// <seealso cref="Task.ContinueWith{TResult}(System.Func{System.Threading.Tasks.Task,object,TResult},object)"/>
        public static Task<TResult> ContinueWithSynchronously<TResult>(
            Task                        task,
            Func<Task, object, TResult> continuationFunction,
            object                      state)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            return task.ContinueWith(
                continuationFunction,
                state,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default
            );
        }

        /// <summary>
        /// Creates a continuation task that executes synchronously when the task completes.
        /// </summary>
        /// <param name="task">The task to which the continuation task is attached.</param>
        /// <param name="continuationAction">The action to run when the task completes.</param>
        /// <returns>A new continuation <see cref="Task"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
        /// <remarks>If the task has already completed, the continuation task executes immediately.</remarks>
        /// <seealso cref="Task{TResult}.ContinueWith(System.Action{System.Threading.Tasks.Task{TResult}})"/>
        public static Task ContinueWithSynchronously<TResult>(
            Task<TResult>         task,
            Action<Task<TResult>> continuationAction)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            return task.ContinueWith(
                continuationAction,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default
            );
        }

        /// <summary>
        /// Creates a continuation task that executes synchronously when the task completes.
        /// </summary>
        /// <param name="task">The task to which the continuation task is attached.</param>
        /// <param name="continuationAction">The action to run when the task completes.</param>
        /// <param name="state">The state to pass to <paramref name="continuationAction"/>.</param>
        /// <returns>A new continuation <see cref="Task"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
        /// <remarks>If the task has already completed, the continuation task executes immediately.</remarks>
        /// <seealso cref="Task{TResult}.ContinueWith(System.Action{System.Threading.Tasks.Task{TResult},object},object)"/>
        public static Task ContinueWithSynchronously<TResult>(
            Task<TResult>                 task,
            Action<Task<TResult>, object> continuationAction,
            object                        state)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            return task.ContinueWith(
                continuationAction,
                state,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default
            );
        }

        /// <summary>
        /// Creates a continuation task that executes synchronously when the task completes.
        /// </summary>
        /// <param name="task">The task to which the continuation task is attached.</param>
        /// <param name="continuationFunction">The function to run when the task completes.</param>
        /// <returns>A new continuation <see cref="Task{TNewResult}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
        /// <remarks>If the task has already completed, the continuation task executes immediately.</remarks>
        /// <seealso cref="Task{TResult}.ContinueWith{TNewResult}(System.Func{System.Threading.Tasks.Task{TResult},TNewResult})"/>
        public static Task<TNewResult> ContinueWithSynchronously<TResult, TNewResult>(
            Task<TResult>                   task,
            Func<Task<TResult>, TNewResult> continuationFunction)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            return task.ContinueWith(
                continuationFunction,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default
            );
        }

        /// <summary>
        /// Creates a continuation task that executes synchronously when the task completes.
        /// </summary>
        /// <param name="task">The task to which the continuation task is attached.</param>
        /// <param name="continuationFunction">The function to run when the task completes.</param>
        /// <param name="state">The state to pass to <paramref name="continuationFunction"/>.</param>
        /// <returns>A new continuation <see cref="Task{TNewResult}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
        /// <remarks>If the task has already completed, the continuation task executes immediately.</remarks>
        /// <seealso cref="Task{TResult}.ContinueWith{TNewResult}(System.Func{System.Threading.Tasks.Task{TResult},object,TNewResult},object)"/>
        public static Task<TNewResult> ContinueWithSynchronously<TResult, TNewResult>(
            Task<TResult>                           task,
            Func<Task<TResult>, object, TNewResult> continuationFunction,
            object                                  state)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            return task.ContinueWith(
                continuationFunction,
                state,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default
            );
        }
    }
}
