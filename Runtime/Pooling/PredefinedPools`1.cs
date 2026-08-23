using System.Collections.Generic;
using Aurora.Collections;

namespace Aurora.Pooling
{
    /// <summary>
    /// Provides a set of predefined public object pools with 1 type parameter.
    /// </summary>
    /// <typeparam name="T">The type parameter required by the pool's members.</typeparam>
    public static class PredefinedPools<T>
    {
        /// <summary>
        /// A pool of arrays of length 2.
        /// </summary>
        public static readonly IPool<T[]> ArrayLength2 = new Pool<T[]>(new PooledArrayLength2Policy<T>());

        /// <summary>
        /// A pool of arrays of length 4.
        /// </summary>
        public static readonly IPool<T[]> ArrayLength4 = new Pool<T[]>(new PooledArrayLength4Policy<T>());

        /// <summary>
        /// A pool of arrays of length 8.
        /// </summary>
        public static readonly IPool<T[]> ArrayLength8 = new Pool<T[]>(new PooledArrayLength8Policy<T>());

        /// <summary>
        /// A pool of hash sets.
        /// </summary>
        public static readonly IPool<HashSet<T>> HashSet = new Pool<HashSet<T>>(new PooledHashSetPolicy<T>());

        /// <summary>
        /// A pool of lists.
        /// </summary>
        public static readonly IPool<List<T>> List = new Pool<List<T>>(new PooledListPolicy<T>());

        /// <summary>
        /// A pool of queues.
        /// </summary>
        public static readonly IPool<Queue<T>> Queue = new Pool<Queue<T>>(new PooledQueuePolicy<T>());

        /// <summary>
        /// A pool of stacks.
        /// </summary>
        public static readonly IPool<Stack<T>> Stack = new Pool<Stack<T>>(new PooledStackPolicy<T>());

        /// <summary>
        /// A pool of deques.
        /// </summary>
        public static readonly IPool<Deque<T>> Deque = new Pool<Deque<T>>(new PooledDequePolicy<T>());
    }
}
