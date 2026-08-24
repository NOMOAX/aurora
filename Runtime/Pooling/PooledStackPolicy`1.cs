using System.Collections.Generic;

namespace Aurora.Pooling
{
    /// <summary>
    /// A strategy for managing stacks in the pool.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the stack.</typeparam>
    public class PooledStackPolicy<T> : IPooledObjectPolicy<Stack<T>>
    {
        /// <summary>
        /// Gets or sets the initial capacity of pooled stacks.
        /// </summary>
        public int InitialCapacity { get; set; } = 16;

        /// <summary>
        /// Gets or sets the maximum length of stacks allowed into the pool.
        /// </summary>
        public int MaximumRetainedCount { get; set; } = 256;

        /// <inheritdoc />
        public Stack<T> Create()
        {
            return new Stack<T>(InitialCapacity);
        }

        /// <inheritdoc />
        public void Get(Stack<T> obj)
        {
        }

        /// <inheritdoc />
        public bool Return(Stack<T> obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.Count > MaximumRetainedCount)
            {
                return false;
            }
            obj.Clear();
            return true;
        }

        /// <inheritdoc />
        public void Dispose(Stack<T> obj)
        {
            obj?.Clear();
        }
    }
}
