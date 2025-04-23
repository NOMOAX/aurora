using System;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Pooling;
using Aurora.Sorting;

namespace Aurora.Collections
{
    /// <summary>
    /// 可修复可操作性的可操作结点。
    /// </summary>
    public sealed class RepairableOperableNode : Node, IComparable, IComparable<RepairableOperableNode>
    {
        /// <summary>
        /// 表示一个方法，该方法的返回值为 <see langword="true"/>。
        /// </summary>
        public static readonly Func<bool> FuncReturnTrue = ReturnTrue;

        /// <summary>
        /// 表示一个方法，该方法的返回值为一个任务，根据 <see cref="CancellationToken"/> 参数的状态不同，该任务为已取消或已完成的任务。
        /// </summary>
        public static readonly Func<CancellationToken, Task> FuncReturnCompletedTask = ReturnCompletedTask;

        private static bool ReturnTrue()
        {
            return true;
        }

        private static Task ReturnCompletedTask(CancellationToken cancellationToken)
        {
            return cancellationToken.IsCancellationRequested
                       ? Task.FromCanceled(cancellationToken)
                       : Task.CompletedTask;
        }

        private readonly Func<bool> _operableFunc;

        private readonly Func<CancellationToken, Task> _repairOperabilityAsyncFunc;

        private readonly int _priority;

        /// <summary>
        /// 初始化 <see cref="RepairableOperableNode"/> 类的新实例。
        /// </summary>
        public RepairableOperableNode()
        {
            _operableFunc               = FuncReturnTrue;
            _repairOperabilityAsyncFunc = FuncReturnCompletedTask;
            _priority                   = 0;
        }

        /// <summary>
        /// 初始化 <see cref="RepairableOperableNode"/> 类的新实例。
        /// </summary>
        /// <param name="operableFunc">一个返回值为 <see cref="bool"/> 的方法。返回值表示该结点是否可操作。</param>
        /// <param name="repairOperabilityAsyncFunc">一个返回值为 <see cref="Task"/> 的方法。返回值是一个任务，执行该任务以尝试异步修复该结点的可操作性。</param>
        /// <param name="priority">优先级。父结点会优先访问和操作优先级更小的子结点。</param>
        /// <exception cref="ArgumentNullException"><paramref name="operableFunc"/> 或 <paramref name="repairOperabilityAsyncFunc"/> 为 <see langword="null"/></exception>
        public RepairableOperableNode(
            Func<bool>                    operableFunc,
            Func<CancellationToken, Task> repairOperabilityAsyncFunc,
            int                           priority)
        {
            _operableFunc = operableFunc ?? throw new ArgumentNullException(nameof(operableFunc));
            _repairOperabilityAsyncFunc = repairOperabilityAsyncFunc ??
                                          throw new ArgumentNullException(nameof(repairOperabilityAsyncFunc));
            _priority = priority;
        }

        /// <summary>
        /// 优先级。
        /// </summary>
        /// <remarks>父结点会优先访问和操作优先级更小的子结点。</remarks>
        public int Priority => _priority;

        /// <summary>
        /// 获取一个值，这个值指示此结点是否可操作。
        /// </summary>
        public bool Operable => InternalOperable;

        private bool InternalOperable
        {
            get
            {
                var version      = Version;
                var selfOperable = _operableFunc();
                ValidateVersion(version);
                if (!selfOperable)
                {
                    return false;
                }
                foreach (var node in Children)
                {
                    if (node is RepairableOperableNode { InternalOperable: false })
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 尝试异步修复此结点的可操作性。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> 已取消。</exception>
        public Task RepairOperabilityAsync(CancellationToken cancellationToken = default)
        {
            return InternalRepairOperabilityAsync(cancellationToken);
        }

        private async Task InternalRepairOperabilityAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var list = PredefinedPools<RepairableOperableNode>.List.Get();
            try
            {
                GetChildren(list);
                if (list.Count > 0)
                {
                    TimSort.Sort(list);
                    foreach (var child in list)
                    {
                        if (child.InternalOperable)
                        {
                            continue;
                        }
                        await child.InternalRepairOperabilityAsync(cancellationToken);
                        if (!child.InternalOperable)
                        {
                            return;
                        }
                    }
                }
            }
            finally
            {
                PredefinedPools<RepairableOperableNode>.List.Return(list);
            }
            var version      = Version;
            var selfOperable = _operableFunc();
            ValidateVersion(version);
            if (!selfOperable)
            {
                version = Version;
                await _repairOperabilityAsyncFunc(cancellationToken).ConfigureAwait(false);
                ValidateVersion(version);
            }
        }

        /// <summary>
        /// 在允许执行一次修复的条件下，获取 <see cref="Operable"/> 的值。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>最终的 <see cref="Operable"/> 的值。</returns>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> 已取消。</exception>
        public async Task<bool> GetRepairedOperableAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (InternalOperable)
            {
                return true;
            }
            await InternalRepairOperabilityAsync(cancellationToken);
            return InternalOperable;
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
            return obj is RepairableOperableNode other ? CompareTo(other) : throw new ArgumentException();
        }

        /// <inheritdoc />
        public int CompareTo(RepairableOperableNode other)
        {
            if (other == null)
            {
                return 1;
            }
            return this == other ? 0 : _priority.CompareTo(other._priority);
        }
    }
}
