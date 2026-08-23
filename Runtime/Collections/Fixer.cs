using System;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Pooling;
using Aurora.Sorting;

namespace Aurora.Collections
{
    /// <summary>
    /// A fixer.
    /// </summary>
    public sealed class Fixer : Node, IComparable, IComparable<Fixer>
    {
        /// <summary>
        /// Represents a method whose return value is <see langword="true"/>.
        /// </summary>
        public static readonly Func<bool> IsAlwaysFixed = () => true;

        /// <summary>
        /// Represents a method whose return value is a task that is either canceled or successfully completed, depending on the state of the <see cref="CancellationToken"/> argument.
        /// </summary>
        public static readonly Func<CancellationToken, Task> CompletedTask = cancellationToken =>
            cancellationToken.IsCancellationRequested ? Task.FromCanceled(cancellationToken) : Task.CompletedTask;

        private readonly Func<bool> _isFixedFunc;

        private readonly Func<CancellationToken, Task> _fixAsyncFunc;

        private readonly int _priority;

        /// <summary>
        /// Initializes a new instance of the <see cref="Fixer"/> class.
        /// <br/>
        /// It is always fixed, and the asynchronous fix operation completes synchronously immediately.
        /// </summary>
        public Fixer()
        {
            _isFixedFunc  = IsAlwaysFixed;
            _fixAsyncFunc = CompletedTask;
            _priority     = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Fixer"/> class.
        /// </summary>
        /// <param name="isFixedFunc">A method whose return value is <see cref="bool"/>. The return value indicates whether the node is fixed.</param>
        /// <param name="fixAsyncFunc">A method whose return value is <see cref="Task"/>. The return value is a task; executing it attempts to fix the node asynchronously.</param>
        /// <param name="priority">The priority. A parent node accesses and processes children with a smaller priority first.</param>
        /// <exception cref="ArgumentNullException"><paramref name="isFixedFunc"/> or <paramref name="fixAsyncFunc"/> is <see langword="null"/></exception>
        public Fixer(Func<bool> isFixedFunc, Func<CancellationToken, Task> fixAsyncFunc, int priority)
        {
            _isFixedFunc  = isFixedFunc ?? throw new ArgumentNullException(nameof(isFixedFunc));
            _fixAsyncFunc = fixAsyncFunc ?? throw new ArgumentNullException(nameof(fixAsyncFunc));
            _priority     = priority;
        }

        /// <summary>
        /// The priority.
        /// </summary>
        /// <remarks>A parent node accesses and processes children with a smaller priority first.</remarks>
        public int Priority => _priority;

        /// <summary>
        /// Gets a value that indicates whether this node is fixed.
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
        /// Attempts to fix this node asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> has been canceled.</exception>
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
        /// If the value of <see cref="IsFixed"/> is <see langword="true"/>, directly returns <see langword="true"/>; otherwise, attempts to fix this node asynchronously and then returns the value of <see cref="IsFixed"/> after the attempt.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The final value of <see cref="IsFixed"/>.</returns>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> has been canceled.</exception>
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
