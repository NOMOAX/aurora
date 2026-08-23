namespace Aurora.Pooling
{
    /// <summary>
    /// An object pool.
    /// </summary>
    /// <typeparam name="T">The type of the members in the object pool.</typeparam>
    public interface IPool<T> where T : class
    {
        /// <summary>
        /// If the pool has an available member, retrieves it; otherwise, creates a new object.
        /// </summary>
        /// <returns>A member retrieved from the pool or a newly created object.</returns>
        T Get();

        /// <summary>
        /// Returns an object to the pool.
        /// </summary>
        /// <param name="obj">The object to return to the pool.</param>
        void Return(T obj);

        /// <summary>
        /// Removes all members from the pool.
        /// </summary>
        void Clear();
    }
}
