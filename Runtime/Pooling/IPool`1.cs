using System;

namespace Aurora.Pooling
{
    /// <summary>
    /// 对象池。
    /// </summary>
    /// <typeparam name="T">对象池中成员的类型。</typeparam>
    public interface IPool<T> : IDisposable where T : class
    {
        /// <summary>
        /// 获取一个值，这个值指示对象池是否为空。
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 如果对象池中有可用的成员，则取出该成员；否则创建一个新对象。
        /// </summary>
        /// <returns>从对象池中取出的成员或创建的新对象。</returns>
        T Get();

        /// <summary>
        /// 将对象放入对象池。
        /// </summary>
        /// <param name="obj">要放入对象池的对象。</param>
        void Return(T obj);

        /// <summary>
        /// 移除对象池中的所有成员。
        /// </summary>
        void Clear();
    }
}
