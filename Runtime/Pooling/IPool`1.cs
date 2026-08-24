namespace Aurora.Pooling
{
    /// <summary>
    /// An object pool.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the pool.</typeparam>
    public interface IPool<T> where T : class
    {
        /// <summary>
        /// Gets an object from the pool if one is available; otherwise, creates a new object.
        /// </summary>
        /// <returns>An object retrieved from the pool or a newly created object.</returns>
        T Get();

        /// <summary>
        /// Returns an object to the pool.
        /// </summary>
        /// <param name="obj">The object to return to the pool.</param>
        void Return(T obj);

        /// <summary>
        /// Removes all objects from the pool.
        /// </summary>
        void Clear();
    }
}
