namespace Aurora.Pooling
{
    /// <summary>
    /// Represents a policy for managing pooled objects.
    /// </summary>
    /// <typeparam name="T">The type of pooled objects.</typeparam>
    public interface IPooledObjectPolicy<T> where T : class
    {
        /// <summary>
        /// Creates a <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The created <typeparamref name="T"/>.</returns>
        T Create();

        /// <summary>
        /// The operation to perform on an available pool member or a newly created object when it is retrieved from the pool or created.
        /// </summary>
        /// <param name="obj">An available member retrieved from the pool, or a newly created object.</param>
        void Get(T obj);

        /// <summary>
        /// Determines whether an object can be returned to the pool.
        /// <br/>
        /// If the object can be returned to the pool, this method should also initialize it.
        /// </summary>
        /// <param name="obj">The object to return to the pool.</param>
        /// <returns><see langword="true"/> if the object should be returned to the pool; otherwise, <see langword="false"/>.</returns>
        bool Return(T obj);

        /// <summary>
        /// When the pool is disposed, disposes all cached objects in the pool; or when an object is rejected for return, disposes that object.
        /// </summary>
        /// <param name="obj">The object to dispose.</param>
        void Dispose(T obj);
    }
}
