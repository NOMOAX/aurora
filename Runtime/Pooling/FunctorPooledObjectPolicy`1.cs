using System;

namespace Aurora.Pooling
{
    /// <summary>
    /// 表示使用指定方法管理池化对象的策略。
    /// </summary>
    /// <typeparam name="T">池化对象的类型。</typeparam>
    public sealed class FunctorPooledObjectPolicy<T> : IPooledObjectPolicy<T> where T : class
    {
        private static readonly Action<T> DefaultOnGetHandler = EmptyAction;

        private static readonly Func<T, bool> DefaultReturnHandler = EmptyFuncReturnTrue;

        private static readonly Action<T> DefaultDisposeHandler = EmptyAction;

        private readonly Func<T> _createFunc;

        private readonly Action<T> _onGetAction;

        private readonly Func<T, bool> _returnFunc;

        private readonly Action<T> _disposeAction;

        /// <summary>
        /// 初始化 <see cref="FunctorPooledObjectPolicy{T}"/> 类的新实例。
        /// </summary>
        /// <param name="createFunc">用于实现 <see cref="IPooledObjectPolicy{T}.Create"/> 的方法。</param>
        /// <param name="onGetAction">用于实现 <see cref="IPooledObjectPolicy{T}.OnGet"/> 的方法。</param>
        /// <param name="returnFunc">用于实现 <see cref="IPooledObjectPolicy{T}.Return"/> 的方法。</param>
        /// <param name="disposeAction">用于实现 <see cref="IPooledObjectPolicy{T}.Dispose"/> 的方法。</param>
        /// <exception cref="ArgumentNullException"><paramref name="createFunc"/> 为 <see langword="null"/>。</exception>
        public FunctorPooledObjectPolicy(
            Func<T>       createFunc,
            Action<T>     onGetAction,
            Func<T, bool> returnFunc,
            Action<T>     disposeAction)
        {
            _createFunc    = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _onGetAction   = onGetAction ?? DefaultOnGetHandler;
            _returnFunc    = returnFunc ?? DefaultReturnHandler;
            _disposeAction = disposeAction ?? DefaultDisposeHandler;
        }

        /// <inheritdoc />
        public T Create()
        {
            return _createFunc();
        }

        /// <inheritdoc />
        public void OnGet(T obj)
        {
            _onGetAction(obj);
        }

        /// <inheritdoc />
        public bool Return(T obj)
        {
            return _returnFunc(obj);
        }

        /// <inheritdoc />
        public void Dispose(T obj)
        {
            _disposeAction(obj);
        }

        private static void EmptyAction(T obj)
        {
        }

        private static bool EmptyFuncReturnTrue(T obj)
        {
            return true;
        }
    }
}
