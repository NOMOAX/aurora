namespace Aurora
{
    /// <summary>
    /// Represents a method that defines a set of conditions and determines whether a specified object satisfies them.
    /// </summary>
    /// <param name="obj">The object to determine whether it satisfies the specified conditions.</param>
    /// <param name="state">The data used by the method.</param>
    /// <typeparam name="TSource">The type of the objects to determine whether they satisfy the specified conditions.</typeparam>
    /// <typeparam name="TState">The type of the state parameter passed by the user.</typeparam>
    /// <returns><see langword="true"/> if the specified object satisfies the conditions defined by the method represented by this delegate; otherwise, <see langword="false"/>.</returns>
    public delegate bool ParameterizedPredicate<in TSource, in TState>(TSource obj, TState state);
}
