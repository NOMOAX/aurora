using System;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Pooling;
using Aurora.Sorting;

namespace Aurora.Collections
{
    /// <summary>
    /// 修复器。
    /// </summary>
    public sealed class Fixer : Node, IComparable, IComparable<Fixer>
    {
        /// <summary>
        /// 表示一个方法，该方法的返回值为 <see langword="true"/>。
        /// </summary>
        public static readonly Func<bool> IsAlwaysFixed = () => true;

        /// <summary>
        /// 表示一个方法，该方法的返回值为一个任务，根据 <see cref="CancellationToken"/> 参数的状态不同，该任务为已取消或已成功完成的任务。
        /// </summary>
        public static readonly Func<CancellationToken, Task> CompletedTask = cancellationToken =>
            cancellationToken.IsCancellationRequested ? Task.FromCanceled(cancellationToken) : Task.CompletedTask;

        private readonly Func<bool> _isFixedFunc;

        private readonly Func<CancellationToken, Task> _fixAsyncFunc;

        private readonly int _priority;

        /// <summary>
        /// 初始化 <see cref="Fixer"/> 类的新实例。
        /// <br/>
        /// 它总是已修复的，并且异步修复操作会立即同步完成。
        /// </summary>
        public Fixer()
        {
            _isFixedFunc  = IsAlwaysFixed;
            _fixAsyncFunc = CompletedTask;
            _priority     = 0;
        }

        /// <summary>
        /// 初始化 <see cref="Fixer"/> 类的新实例。
        /// </summary>
        /// <param name="isFixedFunc">一个返回值为 <see cref="bool"/> 的方法。返回值表示该结点是否已修复。</param>
        /// <param name="fixAsyncFunc">一个返回值为 <see cref="Task"/> 的方法。返回值是一个任务，执行该任务以尝试异步修复该结点。</param>
        /// <param name="priority">优先级。父结点会优先访问和处理优先级更小的子结点。</param>
        /// <exception cref="ArgumentNullException"><paramref name="isFixedFunc"/> 或 <paramref name="fixAsyncFunc"/> 为 <see langword="null"/></exception>
        public Fixer(Func<bool> isFixedFunc, Func<CancellationToken, Task> fixAsyncFunc, int priority)
        {
            _isFixedFunc  = isFixedFunc ?? throw new ArgumentNullException(nameof(isFixedFunc));
            _fixAsyncFunc = fixAsyncFunc ?? throw new ArgumentNullException(nameof(fixAsyncFunc));
            _priority     = priority;
        }

        /// <summary>
        /// 优先级。
        /// </summary>
        /// <remarks>父结点会优先访问和处理优先级更小的子结点。</remarks>
        public int Priority => _priority;

        /// <summary>
        /// 获取一个值，这个值指示此结点是否已修复。
        /// </summary>
        public bool IsFixed => InternalIsFixed;

        private bool InternalIsFixed
        {
            get
            {
                var version     = Version;
                var selfIsFixed = _isFixedFunc();
                ValidateVersion(version);
                if (!selfIsFixed)
                {
                    return false;
                }
                foreach (var node in Children)
                {
                    if (node is Fixer { InternalIsFixed: false })
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 尝试异步修复此结点。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> 已取消。</exception>
        public Task FixAsync(CancellationToken cancellationToken = default)
        {
            return InternalFixAsync(cancellationToken);
        }

        private async Task InternalFixAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var list = PredefinedPools<Fixer>.List.Get();
            try
            {
                GetChildren(list);
                if (list.Count > 0)
                {
                    TimSort.Sort(list);
                    foreach (var child in list)
                    {
                        if (child.InternalIsFixed)
                        {
                            continue;
                        }
                        await child.InternalFixAsync(cancellationToken);
                        if (!child.InternalIsFixed)
                        {
                            return;
                        }
                    }
                }
            }
            finally
            {
                PredefinedPools<Fixer>.List.Return(list);
            }
            var version     = Version;
            var selfIsFixed = _isFixedFunc();
            ValidateVersion(version);
            if (!selfIsFixed)
            {
                version = Version;
                await _fixAsyncFunc(cancellationToken).ConfigureAwait(false);
                ValidateVersion(version);
            }
        }

        /// <summary>
        /// 如果 <see cref="IsFixed"/> 的值为 <see langword="true"/>，则直接返回 <see langword="true"/>；否则，尝试异步修复此结点，然后返回尝试修复后 <see cref="IsFixed"/> 的值。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>最终的 <see cref="IsFixed"/> 的值。</returns>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> 已取消。</exception>
        public async Task<bool> EnsureFixedAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (InternalIsFixed)
            {
                return true;
            }
            await InternalFixAsync(cancellationToken);
            return InternalIsFixed;
        }

        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            if (this == obj)
            {
                return 0;
            }
            return obj is Fixer other ? CompareTo(other) : throw new ArgumentException();
        }

        /// <inheritdoc />
        public int CompareTo(Fixer other)
        {
            if (other == null)
            {
                return 1;
            }
            return this == other ? 0 : _priority.CompareTo(other._priority);
        }
    }
}
