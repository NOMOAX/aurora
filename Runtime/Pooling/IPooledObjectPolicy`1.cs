namespace Aurora.Pooling
{
    /// <summary>
    /// 表示用于管理池化对象的策略。
    /// </summary>
    /// <typeparam name="T">池化对象的类型。</typeparam>
    public interface IPooledObjectPolicy<T> where T : class
    {
        /// <summary>
        /// 创建一个 <typeparamref name="T"/>。
        /// </summary>
        /// <returns>创建出来的 <typeparamref name="T"/>。</returns>
        T Create();

        /// <summary>
        /// 在取出对象池中的可用的成员或创建新对象时，要对该成员或新对象执行的操作。
        /// </summary>
        /// <param name="obj">从对象池中取出的可用的成员，或创建的新对象。</param>
        void Get(T obj);

        /// <summary>
        /// 判断对象是否可以放入池。
        /// <br/>
        /// 如果对象可以放入池，此方法还应初始化对象。
        /// </summary>
        /// <param name="obj">要放入池的对象。</param>
        /// <returns>如果应该把对象放入池，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        bool Return(T obj);

        /// <summary>
        /// 当池释放时，释放池中缓存的所有对象；或者当对象被拒绝放入池时，释放该对象。
        /// </summary>
        /// <param name="obj">要释放的对象。</param>
        void Dispose(T obj);
    }
}
