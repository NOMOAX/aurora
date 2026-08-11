using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Aurora.Pooling;

namespace Aurora.Collections
{
    /// <summary>
    /// 表示树的结点。
    /// </summary>
    public class Node : IEnumerable<Node>
    {
        private static readonly Func<Node, IEnumerable<Node>> FuncGetChildren = GetChildrenAsEnumerable;

        private static readonly Func<Node, object, bool> FuncValidate = ValidateVersion;

        private Node _parent;

        /// <summary>
        /// 获取或设置直接父结点。
        /// </summary>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> 为此结点自身，或者 <paramref name="value"/> 为此结点的子结点，或者在此类型的实现中拒绝将 <paramref name="value"/> 设置为直接父结点，或者 <paramref name="value"/> 不为 <see langword="null"/>，并且在 <paramref name="value"/> 类型的实现中拒绝将此结点添加到 <paramref name="value"/> 的直接子结点列表中。</exception>
        public Node Parent
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _parent;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => SetParent(value);
        }

        private readonly List<Node> _children = new();

        private int _version;

        /// <summary>
        /// 获取存放直接子结点的只读集合。
        /// </summary>
        public IReadOnlyList<Node> Children
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _children;
        }

        /// <summary>
        /// 获取根结点。
        /// </summary>
        public Node Root
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GetRoot();
        }

        /// <summary>
        /// 获取一个值，这个值指示这个结点是否是根结点。
        /// </summary>
        public bool IsRoot
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _parent == null;
        }

        /// <summary>
        /// 获取一个值，这个值指示这个结点是否是叶子结点。
        /// </summary>
        public bool IsLeaf
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _children.Count == 0;
        }

        /// <summary>
        /// 获取一个值，这个值指示这个结点的层次。
        /// </summary>
        /// <remarks>根结点的层次为 0。</remarks>
        public int Level
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GetLevel();
        }

        /// <summary>
        /// 获取结点的版本。
        /// </summary>
        /// <remarks>
        /// 公开此属性，以允许实现额外的枚举器。
        /// <br/>
        /// 每当直接和间接子结点发生变化，这个值都会更新。
        /// <br/>
        /// 根据这个值的不变确保枚举依然合理。
        /// </remarks>
        public int Version
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _version;
        }

        private static IEnumerable<Node> GetChildrenAsEnumerable(Node node)
        {
            return node._children;
        }

        private static bool ValidateVersion(Node node, object state)
        {
            return node.Version == (int)state;
        }

        /// <summary>
        /// 获取一个值，这个值指示这个结点是否是指定结点的子结点。
        /// </summary>
        /// <param name="node">指定的结点。</param>
        /// <returns>这个结点是否是指定结点的子结点。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> 为 <see langword="null"/>。</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsChildOf(Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }
            return InternalIsChildOf(node);
        }

        /// <summary>
        /// 获取一个值，这个值指示这个结点是否是指定结点的父结点。
        /// </summary>
        /// <param name="node">指定的结点。</param>
        /// <returns>这个结点是否是指定结点的父结点。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> 为 <see langword="null"/>。</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsParentOf(Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }
            return node.InternalIsChildOf(this);
        }

        private bool InternalIsChildOf(Node node)
        {
            if (node == this)
            {
                return false;
            }
            for (var parent = _parent; parent != null; parent = parent._parent)
            {
                if (node == parent)
                {
                    return true;
                }
            }
            return false;
        }

        private Node GetRoot()
        {
            var root   = this;
            var parent = _parent;
            while (parent != null)
            {
                root   = parent;
                parent = parent._parent;
            }
            return root;
        }

        private int GetLevel()
        {
            var level  = 0;
            var parent = _parent;
            while (parent != null)
            {
                ++level;
                parent = parent._parent;
            }
            return level;
        }

        /// <summary>
        /// 获取第一个满足指定条件的子结点。
        /// </summary>
        /// <param name="match">条件。</param>
        /// <returns>第一个满足 <paramref name="match"/> 的子结点。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> 为 <see langword="null"/>。</exception>
        protected Node GetChild(Func<Node, bool> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            var count = _children.Count;
            for (var i = 0; i < count; i++)
            {
                var child = _children[i];
                if (match(child))
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取第一个满足指定条件的子结点。
        /// </summary>
        /// <param name="match">条件。</param>
        /// <param name="state">将传递给 <paramref name="match"/> 的第二个形参。</param>
        /// <returns>第一个满足 <paramref name="match"/> 的子结点。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> 为 <see langword="null"/>。</exception>
        protected Node GetChild(Func<Node, object, bool> match, object state)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            var count = _children.Count;
            for (var i = 0; i < count; i++)
            {
                var child = _children[i];
                if (match(child, state))
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取所有直接子结点，将他们存入指定的集合。
        /// </summary>
        /// <param name="results">用于存放结果的集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="results"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentException"><paramref name="results"/> 是只读的。</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetChildren(ICollection<Node> results)
        {
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }
            if (results.IsReadOnly)
            {
                throw new ArgumentException();
            }
            foreach (var child in _children)
            {
                results.Add(child);
            }
        }

        /// <summary>
        /// 获取所有指定类型的直接子结点，将他们存入指定的集合。
        /// </summary>
        /// <param name="results">用于存放结果的集合。</param>
        /// <typeparam name="TNode">要获取的直接子结点类型。</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="results"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentException"><paramref name="results"/> 是只读的。</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetChildren<TNode>(ICollection<TNode> results) where TNode : Node
        {
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }
            if (results.IsReadOnly)
            {
                throw new ArgumentException();
            }
            foreach (var child in _children)
            {
                if (child is TNode tNode)
                {
                    results.Add(tNode);
                }
            }
        }

        /// <summary>
        /// 如果当前版本不等于指定的版本，则抛出 <see cref="InvalidOperationException"/>。
        /// </summary>
        /// <param name="version">期待版本。</param>
        /// <exception cref="InvalidOperationException">当前版本不等于 <paramref name="version"/>。</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ValidateVersion(int version)
        {
            if (version == Version)
            {
                return;
            }
            throw new InvalidOperationException();
        }

        /// <summary>
        /// 添加子结点。
        /// </summary>
        /// <param name="child">要添加的子结点。</param>
        /// <exception cref="ArgumentNullException"><paramref name="child"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentException"><paramref name="child"/> 为此结点自身，或者 <paramref name="child"/> 为此结点的父结点，或者在 <paramref name="child"/> 类型的实现中拒绝将此结点设置为 <paramref name="child"/> 的直接父结点，或者在此类型的实现中拒绝将 <see langword="child"/> 添加到此结点的直接子结点列表中。</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Node child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }
            child.SetParent(this);
        }

        /// <summary>
        /// 移除子结点。
        /// </summary>
        /// <param name="child">要移除的子结点。</param>
        /// <returns>如果成功地移除了子结点，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="child"/> 为 <see langword="null"/>。</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(Node child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }
            if (child._parent != this)
            {
                return false;
            }
            child.SetParent(null);
            return true;
        }

        /// <summary>
        /// 即将设置 <paramref name="parent"/> 为此结点的直接父结点；若拒绝此行为，子类可重写此方法并在实现中抛出 <see cref="ArgumentException"/> 异常。
        /// </summary>
        /// <param name="parent">（不为 <see langword="null"/>）新的直接父结点。</param>
        /// <exception cref="ArgumentException">在子类的实现中，拒绝设置 <paramref name="parent"/> 为此结点的直接父结点。</exception>
        protected virtual void ThrowIfRejectParent(Node parent)
        {
        }

        /// <summary>
        /// 即将添加 <paramref name="child"/> 到此结点的直接子结点列表中；若拒绝此行为，子类可重写此方法并在实现中抛出 <see cref="ArgumentException"/> 异常。
        /// </summary>
        /// <param name="child">（不为 <see langword="null"/>）新的直接子结点。</param>
        /// <exception cref="ArgumentException">在子类的实现中，拒绝添加 <paramref name="child"/> 到此结点的直接子结点列表中。</exception>
        protected virtual void ThrowIfRejectChild(Node child)
        {
        }

        private void SetParent(Node parent)
        {
            if (parent == _parent)
            {
                return;
            }
            if (parent != null)
            {
                if (parent == this)
                {
                    throw new ArgumentException("不能将结点自身设置为父结点");
                }
                if (parent.InternalIsChildOf(this))
                {
                    throw new ArgumentException("不能将子结点设置为父结点");
                }
                ThrowIfRejectParent(parent);
                parent.ThrowIfRejectChild(this);
            }
            var versionUpdateNodes = PredefinedPools<Node>.HashSet.Get();
            try
            {
                if (_parent != null)
                {
                    _parent._children.Remove(this);
                    PushParents(_parent, versionUpdateNodes);
                }
                if (parent != null)
                {
                    parent._children.Add(this);
                    PushParents(parent, versionUpdateNodes);
                }
                _parent = parent;
                foreach (var shouldUpdateVersionNode in versionUpdateNodes)
                {
                    ++shouldUpdateVersionNode._version;
                }
            }
            finally
            {
                PredefinedPools<Node>.HashSet.Return(versionUpdateNodes);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PushParents(Node parent, HashSet<Node> hashSet)
        {
            do
            {
                hashSet.Add(parent);
                parent = parent._parent;
            } while (parent != null);
        }

        /// <summary>
        /// 根据指定的枚举顺序，获取一个枚举此 <see cref="Node"/> 的枚举器。
        /// </summary>
        /// <param name="order">枚举顺序。</param>
        /// <returns>用于枚举此 <see cref="Node"/> 的枚举器。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="order"/> 不是在 <see cref="TreeEnumOrder"/> 枚举中定义的成员。</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<Node> GetEnumerator(TreeEnumOrder order)
        {
            return InternalGetEnumerator(order);
        }

        private IEnumerator<Node> InternalGetEnumerator(TreeEnumOrder order)
        {
            return order switch
            {
                TreeEnumOrder.Default        => new DirectChildEnumerator(this),
                TreeEnumOrder.BreadthFirstLr => new LrEnumerator<Node>(this, FuncGetChildren, FuncValidate, Version),
                TreeEnumOrder.BreadthFirstRl => new RlEnumerator<Node>(this, FuncGetChildren, FuncValidate, Version),
                TreeEnumOrder.DepthFirstDlr  => new DlrEnumerator<Node>(this, FuncGetChildren, FuncValidate, Version),
                TreeEnumOrder.DepthFirstDrl  => new DrlEnumerator<Node>(this, FuncGetChildren, FuncValidate, Version),
                TreeEnumOrder.DepthFirstLrd  => new LrdEnumerator<Node>(this, FuncGetChildren, FuncValidate, Version),
                TreeEnumOrder.DepthFirstRld  => new RldEnumerator<Node>(this, FuncGetChildren, FuncValidate, Version),
                _                            => throw new ArgumentOutOfRangeException(nameof(order), order, null)
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalGetEnumerator(TreeEnumOrder.Default);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<Node> GetEnumerator()
        {
            return InternalGetEnumerator(TreeEnumOrder.Default);
        }

        private struct DirectChildEnumerator : IEnumerator<Node>
        {
            private Node _node;

            private int _index;

            private readonly int _version;

            private Node _current;

            internal DirectChildEnumerator(Node node)
            {
                _node    = node;
                _index   = 0;
                _version = node.Version;
                _current = null;
            }

            void IDisposable.Dispose()
            {
                if (_node == null)
                {
                    return;
                }
                _node    = null;
                _current = null;
            }

            bool IEnumerator.MoveNext()
            {
                if (_node == null)
                {
                    throw new ObjectDisposedException(typeof(DirectChildEnumerator).FullName);
                }
                if (_version != _node.Version)
                {
                    throw new InvalidOperationException();
                }
                if (_index == _node._children.Count)
                {
                    _current = null;
                    return false;
                }
                _current = _node._children[_index];
                ++_index;
                return true;
            }

            readonly object IEnumerator.Current => Current;

            readonly Node IEnumerator<Node>.Current => Current;

            private readonly Node Current
            {
                get
                {
                    if (_node == null)
                    {
                        throw new ObjectDisposedException(typeof(DirectChildEnumerator).FullName);
                    }
                    return _current ?? throw new InvalidOperationException();
                }
            }

            void IEnumerator.Reset()
            {
                if (_node == null)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }
                if (_version != _node.Version)
                {
                    throw new InvalidOperationException();
                }
                _index   = 0;
                _current = null;
            }
        }

        /// <summary>
        /// 确保结点的 <see cref="Node.Version"/> 未改变的范围。
        /// </summary>
        public struct ValidateVersionScope : IDisposable
        {
            private Node _node;

            private readonly int _version;

            /// <summary>
            /// 初始化 <see cref="ValidateVersionScope"/> 结构的新实例。
            /// </summary>
            /// <param name="node">结点。</param>
            public ValidateVersionScope(Node node)
            {
                _node    = node;
                _version = node.Version;
            }

            /// <inheritdoc />
            public void Dispose()
            {
                var node = _node;
                if (node != null && Interlocked.CompareExchange(ref _node, null, node) == node)
                {
                    node.ValidateVersion(_version);
                }
            }
        }
    }
}
