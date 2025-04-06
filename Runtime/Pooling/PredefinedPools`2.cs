using System.Collections.Generic;

namespace Aurora.Pooling
{
    /// <summary>
    /// 提供一组预定义的具有 2 个类型参数的公共对象池。
    /// </summary>
    /// <typeparam name="T1">对象池的成员所需要的第 1 个类型参数。</typeparam>
    /// <typeparam name="T2">对象池的成员所需要的第 2 个类型参数。</typeparam>
    public static class PredefinedPools<T1, T2>
    {
        /// <summary>
        /// 字典池。
        /// </summary>
        public static readonly IPool<Dictionary<T1, T2>> Dictionary =
            new Pool<Dictionary<T1, T2>>(new PooledDictionaryPolicy<T1, T2>());
    }
}
