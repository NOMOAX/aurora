using System.Collections.Generic;

namespace Aurora.Pooling
{
    /// <summary>
    /// Provides a set of predefined public object pools with 2 type parameters.
    /// </summary>
    /// <typeparam name="T1">The first type parameter of the pooled objects.</typeparam>
    /// <typeparam name="T2">The second type parameter of the pooled objects.</typeparam>
    public static class PredefinedPools<T1, T2>
    {
        /// <summary>
        /// A pool of dictionaries.
        /// </summary>
        public static readonly IPool<Dictionary<T1, T2>> Dictionary =
            new Pool<Dictionary<T1, T2>>(new PooledDictionaryPolicy<T1, T2>());
    }
}
