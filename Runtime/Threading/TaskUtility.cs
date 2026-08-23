using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Aurora.Threading
{
    /// <summary>
    /// 为 <see cref="Task"/> 类提供工具方法。
    /// </summary>
    public static class TaskUtility
    {
        /// <summary>
        /// 在新的上下文中等待 <see cref="Task"/>。
        /// </summary>
        /// <param name="task">任务。</param>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> 为 <see langword="null"/>。</exception>
        public static async void BeginAwait(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            await task.ConfigureAwait(false);
        }

        /// <summary>
        /// 如果 <see cref="Task"/> 处于错误状态，则抛出导致其处于错误状态的根本异常；如果 <see cref="Task"/> 处于取消状态，则抛出 <see cref="TaskCanceledException"/>。
        /// </summary>
        /// <param name="task">任务。</param>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="TaskCanceledException"><paramref name="task"/> 处于取消状态。</exception>
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
        /// 如果 <see cref="Task"/> 处于错误状态，则返回导致其处于错误状态的根本异常；否则返回 <see langword="null"/>。
        /// </summary>
        /// <param name="task">任务。</param>
        /// <returns>如果 <see cref="Task"/> 处于错误状态，则为导致其处于错误状态的根本异常；否则为 <see langword="null"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> 为 <see langword="null"/>。</exception>
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
        /// 如果目标任务处于错误状态或取消状态，则对应地设置任务完成源错误或取消，并返回 <see langword="true"/>；否则返回 <see langword="false"/>。
        /// </summary>
        /// <param name="task">任务。</param>
        /// <param name="taskCompletionSource">任务完成源。</param>
        /// <typeparam name="TResult">任务完成源的类型参数。</typeparam>
        /// <returns>如果目标任务处于错误状态或取消状态，则对应地设置任务完成源为错误或取消状态，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> 或 <paramref name="taskCompletionSource"/> 为 <see langword="null"/>。</exception>
        public static bool HandleFaultsAndCancellation<TResult>(
            Task                          task,
            TaskCompletionSource<TResult> taskCompletionSource)
        {
            return InternalHandleFaultsAndCancellation(task, taskCompletionSource, CancellationToken.None);
        }

        /// <summary>
        /// 如果目标任务处于错误状态或取消状态，则对应地设置任务完成源错误或取消，并返回 <see langword="true"/>；否则返回 <see langword="false"/>。
        /// </summary>
        /// <param name="task">任务。</param>
        /// <param name="taskCompletionSource">任务完成源。</param>
        /// <param name="cancellationToken">取消令牌。它是目标任务处于取消状态的原因。</param>
        /// <typeparam name="TResult">任务完成源的类型参数。</typeparam>
        /// <returns>如果目标任务处于错误状态或取消状态，则对应地设置任务完成源为错误或取消状态，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> 或 <paramref name="taskCompletionSource"/> 为 <see langword="null"/>。</exception>
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
        /// 创建一个在任务完成时同步执行的延续任务。
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> 为 <see langword="null"/>。</exception>
        /// <remarks>如果任务已完成，则立即执行延续任务。</remarks>
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
        /// 创建一个在任务完成时同步执行的延续任务。
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> 为 <see langword="null"/>。</exception>
        /// <remarks>如果任务已完成，则立即执行延续任务。</remarks>
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
        /// 创建一个在任务完成时同步执行的延续任务。
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> 为 <see langword="null"/>。</exception>
        /// <remarks>如果任务已完成，则立即执行延续任务。</remarks>
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
        /// 创建一个在任务完成时同步执行的延续任务。
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> 为 <see langword="null"/>。</exception>
        /// <remarks>如果任务已完成，则立即执行延续任务。</remarks>
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
        /// 创建一个在任务完成时同步执行的延续任务。
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> 为 <see langword="null"/>。</exception>
        /// <remarks>如果任务已完成，则立即执行延续任务。</remarks>
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
        /// 创建一个在任务完成时同步执行的延续任务。
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> 为 <see langword="null"/>。</exception>
        /// <remarks>如果任务已完成，则立即执行延续任务。</remarks>
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
        /// 创建一个在任务完成时同步执行的延续任务。
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> 为 <see langword="null"/>。</exception>
        /// <remarks>如果任务已完成，则立即执行延续任务。</remarks>
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
        /// 创建一个在任务完成时同步执行的延续任务。
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> 为 <see langword="null"/>。</exception>
        /// <remarks>如果任务已完成，则立即执行延续任务。</remarks>
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
