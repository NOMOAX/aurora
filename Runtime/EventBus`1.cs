using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Aurora
{
    /// <summary>
    /// Manages event registration, unregistration, publishing, and awaiting publication.
    /// </summary>
    /// <typeparam name="T">The type of event identifier.</typeparam>
    public static class EventBus<T>
    {
        private class Promise : TaskCompletionSource<VoidResult>
        {
            private readonly T _id;

            private readonly Delegate _delegate;

            internal Promise(T id)
            {
                _id       = id;
                _delegate = (Action)Complete;
                Subscribe();
            }

            private void Complete()
            {
                if (TrySetResult(new VoidResult()))
                {
                    CleanUp();
                }
            }

            protected virtual void CleanUp()
            {
                Unsubscribe();
            }

            private void Subscribe()
            {
                while (true)
                {
                    if (PromiseDelegates.TryGetValue(_id, out var oldDelegate))
                    {
                        var newDelegate = Delegate.Combine(oldDelegate, _delegate);
                        if (PromiseDelegates.TryUpdate(_id, newDelegate, oldDelegate))
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (PromiseDelegates.TryAdd(_id, _delegate))
                        {
                            return;
                        }
                    }
                }
            }

            private void Unsubscribe()
            {
                while (true)
                {
                    if (!PromiseDelegates.TryGetValue(_id, out var oldDelegate))
                    {
                        return;
                    }
                    var newDelegate = Delegate.Remove(oldDelegate, _delegate);
                    if (newDelegate != null)
                    {
                        if (PromiseDelegates.TryUpdate(_id, newDelegate, oldDelegate))
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (TryRemoveFromPromiseDelegates(_id, oldDelegate))
                        {
                            return;
                        }
                    }
                }
            }
        }

        private sealed class PromiseWithCancellation : Promise
        {
            private static readonly Action<object> Cancel = state =>
            {
                var (promiseWithCancellation, cancellationToken) =
                    (Tuple<PromiseWithCancellation, CancellationToken>)state;
                if (promiseWithCancellation.TrySetCanceled(cancellationToken))
                {
                    promiseWithCancellation.CleanUp();
                }
            };

            private readonly CancellationTokenRegistration _cancellationTokenRegistration;

            internal PromiseWithCancellation(T id, CancellationToken cancellationToken) : base(id)
            {
                _cancellationTokenRegistration = cancellationToken.Register(
                    Cancel,
                    Tuple.Create(this, cancellationToken)
                );
                if (Task.IsCompleted)
                {
                    _cancellationTokenRegistration.Dispose();
                }
            }

            protected override void CleanUp()
            {
                _cancellationTokenRegistration.Dispose();
                base.CleanUp();
            }
        }

        /// <remarks>Represents the signature of the private <c>TryRemoveInternal</c> method of the <c>ConcurrentDictionary&lt;T, Delegate&gt;</c> class.</remarks>
        private delegate bool TryRemoveInternalMethodSignature(
            T            key,
            out Delegate value,
            bool         matchValue,
            Delegate     oldValue);

        /// <summary>
        /// The events.
        /// </summary>
        private static readonly ConcurrentDictionary<T, Delegate> Delegates = new();

        /// <remarks>Represents the private <c>TryRemoveInternal</c> method of the <see cref="Delegates"/> instance.</remarks>
        private static readonly TryRemoveInternalMethodSignature TryRemoveInternalForDelegates;

        /// <summary>
        /// The <see cref="Promise.Complete"/> methods of promises.
        /// </summary>
        /// <remarks>The type of values is actually <see cref="Action"/>.</remarks>
        private static readonly ConcurrentDictionary<T, Delegate> PromiseDelegates = new();

        /// <remarks>Represents the private <c>TryRemoveInternal</c> method of the <see cref="PromiseDelegates"/> instance.</remarks>
        private static readonly TryRemoveInternalMethodSignature TryRemoveInternalForPromiseDelegates;

        static EventBus()
        {
            const string name = "TryRemoveInternal";
            const BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
            const Binder binder = null;
            const CallingConventions callConvention = CallingConventions.Standard | CallingConventions.HasThis;
            var types = new[] { typeof(T), typeof(Delegate).MakeByRefType(), typeof(bool), typeof(Delegate) };
            const ParameterModifier[] modifiers = null;

            TryRemoveInternalForDelegates = (TryRemoveInternalMethodSignature)Delegate.CreateDelegate(
                typeof(TryRemoveInternalMethodSignature),
                Delegates,
                typeof(ConcurrentDictionary<T, Delegate>).GetMethod(
                    name,
                    bindingAttr,
                    binder,
                    callConvention,
                    types,
                    modifiers
                )!,
                true
            );

            TryRemoveInternalForPromiseDelegates = (TryRemoveInternalMethodSignature)Delegate.CreateDelegate(
                typeof(TryRemoveInternalMethodSignature),
                PromiseDelegates,
                typeof(ConcurrentDictionary<T, Delegate>).GetMethod(
                    name,
                    bindingAttr,
                    binder,
                    callConvention,
                    types,
                    modifiers
                )!,
                true
            );
        }

        /// <summary>
        /// Subscribes a delegate to the event identified by the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the event.</param>
        /// <param name="delegate">The delegate to subscribe to the event.</param>
        /// <exception cref="ArgumentException">Both the event associated with <paramref name="id"/> and <paramref name="delegate"/> are not <see langword="null"/>, and they are not instances of the same delegate type.</exception>
        public static void Subscribe(T id, Delegate @delegate)
        {
            if (@delegate == null)
            {
                return;
            }
            while (true)
            {
                if (Delegates.TryGetValue(id, out var oldDelegate))
                {
                    var newDelegate = Delegate.Combine(oldDelegate, @delegate);
                    if (Delegates.TryUpdate(id, newDelegate, oldDelegate))
                    {
                        return;
                    }
                }
                else
                {
                    if (Delegates.TryAdd(id, @delegate))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribes a delegate from the event identified by the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the event.</param>
        /// <param name="delegate">The delegate to unsubscribe from the event.</param>
        /// <exception cref="ArgumentException">Both the event associated with <paramref name="id"/> and <paramref name="delegate"/> are not <see langword="null"/>, and they are not instances of the same delegate type.</exception>
        public static void Unsubscribe(T id, Delegate @delegate)
        {
            if (@delegate == null)
            {
                return;
            }
            while (true)
            {
                if (!Delegates.TryGetValue(id, out var oldDelegate))
                {
                    return;
                }
                var newDelegate = Delegate.Remove(oldDelegate, @delegate);
                if (newDelegate != null)
                {
                    if (Delegates.TryUpdate(id, newDelegate, oldDelegate))
                    {
                        return;
                    }
                }
                else
                {
                    if (TryRemove(id, oldDelegate))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Publishes the event identified by the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the event.</param>
        /// <param name="args">An array of objects that are the arguments to pass to the event associated with <paramref name="id"/>.</param>
        /// <returns>The object returned by the event associated with <paramref name="id"/>, or <see langword="null"/> if <paramref name="id"/> is not found.</returns>
        /// <exception cref="TargetParameterCountException">The <paramref name="args"/> array does not have the correct number of arguments.</exception>
        /// <exception cref="ArgumentException">The element of the <paramref name="args"/> array does not match the signature of the event associated with <paramref name="id"/>.</exception>
        public static object Publish(T id, params object[] args)
        {
            try
            {
                return Delegates.TryGetValue(id, out var @delegate) ? Invoke(@delegate, args) : null;
            }
            finally
            {
                if (PromiseDelegates.TryGetValue(id, out var promiseDelegate))
                {
                    ((Action)promiseDelegate)();
                }
            }
        }

        /// <summary>
        /// Publishes the event identified by the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the event.</param>
        /// <param name="args">An array of objects that are the arguments to pass to the event associated with <paramref name="id"/>.</param>
        /// <returns>An array of objects that are returned by each of the invocation list of the event associated with <paramref name="id"/>, or an empty array if <paramref name="id"/> is not found.</returns>
        /// <exception cref="TargetParameterCountException">The <paramref name="args"/> array does not have the correct number of arguments.</exception>
        /// <exception cref="ArgumentException">The element of the <paramref name="args"/> array does not match the signature of the event associated with <paramref name="id"/>.</exception>
        public static object[] PublishAll(T id, params object[] args)
        {
            try
            {
                return Delegates.TryGetValue(id, out var @delegate)
                           ? InvokeAll(@delegate, args)
                           : Array.Empty<object>();
            }
            finally
            {
                if (PromiseDelegates.TryGetValue(id, out var promiseDelegate))
                {
                    ((Action)promiseDelegate)();
                }
            }
        }

        /// <summary>
        /// Removes all events in <see cref="EventBus{T}"/>.
        /// </summary>
        public static void Clear()
        {
            Delegates.Clear();
            while (!PromiseDelegates.IsEmpty)
            {
                var snapshot = PromiseDelegates.ToArray();
                foreach (var (_, promiseDelegate) in snapshot)
                {
                    ((Action)promiseDelegate)();
                }
            }
        }

        /// <summary>
        /// Returns a task that completes when the event identified by the specified identifier is published.
        /// </summary>
        /// <param name="id">The unique identifier of the event.</param>
        /// <returns>A task that completes when the event identified by <paramref name="id"/> is published.</returns>
        /// <remarks>
        /// The task is completed when <see cref="Publish"/> or <see cref="PublishAll"/> is called with <paramref name="id"/>.
        /// If the event has already been published before this method is called, the returned task will never complete.
        /// </remarks>
        public static Task WhenPublished(T id)
        {
            var promise = new Promise(id);
            return promise.Task;
        }

        /// <summary>
        /// Returns a task that completes when the event identified by the specified identifier is published, or when the cancellation token is canceled.
        /// </summary>
        /// <param name="id">The unique identifier of the event.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the wait.</param>
        /// <returns>A task that completes when the event identified by <paramref name="id"/> is published, or when <paramref name="cancellationToken"/> is canceled.</returns>
        /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was canceled.</exception>
        /// <remarks>
        /// The task is completed when <see cref="Publish"/> or <see cref="PublishAll"/> is called with <paramref name="id"/>.
        /// If the event has already been published before this method is called, the returned task will never complete.
        /// </remarks>
        public static Task WhenPublished(T id, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }
            var promise = cancellationToken.CanBeCanceled switch
            {
                false => new Promise(id),
                true  => new PromiseWithCancellation(id, cancellationToken)
            };
            return promise.Task;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object Invoke(Delegate @delegate, object[] args)
        {
            return @delegate.DynamicInvoke(args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object[] InvokeAll(Delegate @delegate, object[] args)
        {
            var delegates    = @delegate.GetInvocationList();
            var length       = delegates.Length;
            var returnValues = new object[length];
            for (var i = 0; i < length; i++)
            {
                returnValues[i] = Invoke(delegates[i], args);
            }
            return returnValues;
        }

        /// <summary>
        /// Removes a key and value from <see cref="Delegates"/>.
        /// </summary>
        /// <remarks>Both the specified key and value must match the entry in the dictionary for it to be removed.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryRemove(T key, Delegate oldValue)
        {
            return TryRemoveInternalForDelegates(key, out _, true, oldValue);
        }

        /// <summary>
        /// Removes a key and value from <see cref="PromiseDelegates"/>.
        /// </summary>
        /// <remarks>Both the specified key and value must match the entry in the dictionary for it to be removed.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryRemoveFromPromiseDelegates(T key, Delegate oldValue)
        {
            return TryRemoveInternalForPromiseDelegates(key, out _, true, oldValue);
        }
    }
}
