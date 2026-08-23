namespace Aurora
{
    /// <summary>
    /// 表示一种方法，该方法定义一组条件并确定指定对象是否符合这些条件。
    /// </summary>
    /// <param name="obj">要确定是否满足指定条件的对象。</param>
    /// <param name="state">由方法使用的数据。</param>
    /// <typeparam name="TSource">要确定是否满足指定条件的对象的类型。</typeparam>
    /// <typeparam name="TState">由用户传入的状态参数的类型。</typeparam>
    /// <returns>如果指定对象满足由此委托表示的方法定义的条件，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
    public delegate bool ParameterizedPredicate<in TSource, in TState>(TSource obj, TState state);
}
