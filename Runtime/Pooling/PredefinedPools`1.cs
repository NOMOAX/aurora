using System.Collections.Generic;
using Aurora.Collections;

namespace Aurora.Pooling
{
    /// <summary>
    /// 提供一组预定义的具有 1 个类型参数的公共对象池。
    /// </summary>
    /// <typeparam name="T">对象池的成员所需要的类型参数。</typeparam>
    public static class PredefinedPools<T>
    {
        /// <summary>
        /// 长度为 2 的数组池。
        /// </summary>
        public static readonly IPool<T[]> ArrayLength2 = new Pool<T[]>(new PooledArrayLength2Policy<T>());

        /// <summary>
        /// 长度为 4 的数组池。
        /// </summary>
        public static readonly IPool<T[]> ArrayLength4 = new Pool<T[]>(new PooledArrayLength4Policy<T>());

        /// <summary>
        /// 长度为 8 的数组池。
        /// </summary>
        public static readonly IPool<T[]> ArrayLength8 = new Pool<T[]>(new PooledArrayLength8Policy<T>());

        /// <summary>
        /// 哈希集池。
        /// </summary>
        public static readonly IPool<HashSet<T>> HashSet = new Pool<HashSet<T>>(new PooledHashSetPolicy<T>());

        /// <summary>
        /// 列表池。
        /// </summary>
        public static readonly IPool<List<T>> List = new Pool<List<T>>(new PooledListPolicy<T>());

        /// <summary>
        /// 队列池。
        /// </summary>
        public static readonly IPool<Queue<T>> Queue = new Pool<Queue<T>>(new PooledQueuePolicy<T>());

        /// <summary>
        /// 栈池。
        /// </summary>
        public static readonly IPool<Stack<T>> Stack = new Pool<Stack<T>>(new PooledStackPolicy<T>());

        /// <summary>
        /// 双端队列池。
        /// </summary>
        public static readonly IPool<Deque<T>> Deque = new Pool<Deque<T>>(new PooledDequePolicy<T>());
    }
}
