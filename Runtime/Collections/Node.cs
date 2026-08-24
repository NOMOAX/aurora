using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Aurora.Pooling;

namespace Aurora.Collections
{
    /// <summary>
    /// Represents a node of a tree.
    /// </summary>
    public class Node : IEnumerable<Node>
    {
        private static readonly Func<Node, IEnumerable<Node>> FuncGetChildren = GetChildrenAsEnumerable;

        private static readonly Func<Node, object, bool> FuncValidate = ValidateVersion;

        private Node _parent;

        /// <summary>
        /// Gets or sets the direct parent node.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is this node itself, or <paramref name="value"/> is a child of this node, or this type's implementation rejects setting <paramref name="value"/> as the direct parent, or <paramref name="value"/> is not <see langword="null"/>, and <paramref name="value"/> type's implementation rejects adding this node to <paramref name="value"/>'s list of direct children.</exception>
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
        /// Gets the read-only collection that stores the direct children.
        /// </summary>
        public IReadOnlyList<Node> Children
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _children;
        }

        /// <summary>
        /// Gets the root node.
        /// </summary>
        public Node Root
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GetRoot();
        }

        /// <summary>
        /// Gets a value indicating whether this node is the root.
        /// </summary>
        public bool IsRoot
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _parent == null;
        }

        /// <summary>
        /// Gets a value indicating whether this node is a leaf.
        /// </summary>
        public bool IsLeaf
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _children.Count == 0;
        }

        /// <summary>
        /// Gets a value indicating the level of this node.
        /// </summary>
        /// <remarks>The root node is at level 0.</remarks>
        public int Level
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GetLevel();
        }

        /// <summary>
        /// Gets the node's version.
        /// </summary>
        /// <remarks>
        /// Expose this property to allow additional enumerators to be implemented.
        /// <br/>
        /// This value updates whenever direct or indirect children change.
        /// <br/>
        /// The invariants based on this value ensure enumeration remains valid.
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
        /// Gets a value indicating whether this node is a child of the specified node.
        /// </summary>
        /// <param name="node">The specified node.</param>
        /// <returns>Whether this node is a child of the specified node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
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
        /// Gets a value indicating whether this node is the parent of the specified node.
        /// </summary>
        /// <param name="node">The specified node.</param>
        /// <returns>Whether this node is the parent of the specified node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
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
        /// Gets the first child that satisfies the specified condition.
        /// </summary>
        /// <param name="match">The condition.</param>
        /// <returns>The first child that satisfies <paramref name="match"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
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
        /// Gets the first child that satisfies the specified condition.
        /// </summary>
        /// <param name="match">The condition.</param>
        /// <param name="state">The second parameter passed to <paramref name="match"/>.</param>
        /// <returns>The first child that satisfies <paramref name="match"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
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
        /// Gets all direct children and stores them into the specified collection.
        /// </summary>
        /// <param name="results">The collection used to store the results.</param>
        /// <exception cref="ArgumentNullException"><paramref name="results"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="results"/> is read-only.</exception>
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
        /// Gets all direct children of the specified type and stores them into the specified collection.
        /// </summary>
        /// <param name="results">The collection used to store the results.</param>
        /// <typeparam name="TNode">The type of direct children to get.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="results"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="results"/> is read-only.</exception>
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
        /// Throws <see cref="InvalidOperationException"/> if the current version is not equal to the specified version.
        /// </summary>
        /// <param name="version">The expected version.</param>
        /// <exception cref="InvalidOperationException">The current version is not equal to <paramref name="version"/>.</exception>
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
        /// Adds a child node.
        /// </summary>
        /// <param name="child">The child node to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="child"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="child"/> is this node itself, or <paramref name="child"/> is the parent of this node, or <paramref name="child"/> type's implementation rejects setting this node as <paramref name="child"/>'s direct parent, or this type's implementation rejects adding <paramref name="child"/> to this node's list of direct children.</exception>
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
        /// Removes a child node.
        /// </summary>
        /// <param name="child">The child node to remove.</param>
        /// <returns><see langword="true"/> if the child node was successfully removed; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="child"/> is <see langword="null"/>.</exception>
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
        /// About to set <paramref name="parent"/> as this node's direct parent; to reject this, a subclass may override this method and throw <see cref="ArgumentException"/> in its implementation.
        /// </summary>
        /// <param name="parent">The new direct parent (not <see langword="null"/>).</param>
        /// <exception cref="ArgumentException">In a subclass implementation, <paramref name="parent"/> is rejected as this node's direct parent.</exception>
        protected virtual void ThrowIfRejectParent(Node parent)
        {
        }

        /// <summary>
        /// About to add <paramref name="child"/> to this node's list of direct children; to reject this, a subclass may override this method and throw <see cref="ArgumentException"/> in its implementation.
        /// </summary>
        /// <param name="child">The new direct child (not <see langword="null"/>).</param>
        /// <exception cref="ArgumentException">In a subclass implementation, <paramref name="child"/> is rejected from being added to this node's list of direct children.</exception>
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
                    throw new ArgumentException("Cannot set a node as its own parent");
                }
                if (parent.InternalIsChildOf(this))
                {
                    throw new ArgumentException("Cannot set a child node as a parent");
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
        /// Gets an enumerator that enumerates this <see cref="Node"/> according to the specified enumeration order.
        /// </summary>
        /// <param name="order">The enumeration order.</param>
        /// <returns>An enumerator used to enumerate this <see cref="Node"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="order"/> is not a member defined in the <see cref="TreeEnumOrder"/> enum.</exception>
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
                    throw new ObjectDisposedException(nameof(DirectChildEnumerator));
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
        /// A scope that ensures the <see cref="Node.Version"/> of a node is unchanged.
        /// </summary>
        public struct ValidateVersionScope : IDisposable
        {
            private Node _node;

            private readonly int _version;

            /// <summary>
            /// Initializes a new instance of the <see cref="ValidateVersionScope"/> struct.
            /// </summary>
            /// <param name="node">The node.</param>
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
