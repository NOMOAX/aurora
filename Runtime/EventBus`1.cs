using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using Aurora.Diagnostics;

namespace Aurora
{
    /// <summary>
    /// Manages event registration, unregistration, and publishing.
    /// </summary>
    /// <typeparam name="T">The type of event identifier.</typeparam>
    public sealed class EventBus<T>
    {
        private const string SubscriptionFailed = "Subscription (ID = {0}, Type = {1}) failed, will retry.";

        private const string UnsubscriptionFailed = "Unsubscription (ID = {0}, Type = {1}) failed, will retry.";

        /// <remarks>Represents the signature of the private <c>TryRemoveInternal</c> method in <c>ConcurrentDictionary&lt;T, Delegate&gt;</c> class.</remarks>
        private delegate bool TryRemoveInternalMethodSignature(
            T            key,
            out Delegate value,
            bool         matchValue,
            Delegate     oldValue);

        /// <summary>
        /// Get a shared <see cref="EventBus{T}"/> instance.
        /// </summary>
        public static EventBus<T> Shared { get; } = new();

        private readonly ConcurrentDictionary<T, Delegate> _delegates = new();

        /// <remarks>Represents the private <c>TryRemoveInternal</c> method of the <see cref="_delegates"/> instance.</remarks>
        private readonly TryRemoveInternalMethodSignature _tryRemoveInternal;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBus{T}"/> class.
        /// </summary>
        public EventBus()
        {
            _tryRemoveInternal = (TryRemoveInternalMethodSignature)Delegate.CreateDelegate(
                typeof(TryRemoveInternalMethodSignature),
                _delegates,
                typeof(ConcurrentDictionary<T, Delegate>).GetMethod(
                    "TryRemoveInternal",
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    CallingConventions.Standard | CallingConventions.HasThis,
                    new[] { typeof(T), typeof(Delegate).MakeByRefType(), typeof(bool), typeof(Delegate) },
                    null
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
        public void Subscribe(T id, Delegate @delegate)
        {
            if (@delegate == null)
            {
                return;
            }
            while (true)
            {
                if (_delegates.TryGetValue(id, out var oldDelegate))
                {
                    var newDelegate = Delegate.Combine(oldDelegate, @delegate);
                    if (_delegates.TryUpdate(id, newDelegate, oldDelegate))
                    {
                        return;
                    }
                }
                else
                {
                    if (_delegates.TryAdd(id, @delegate))
                    {
                        return;
                    }
                }
                Log.V(string.Format(SubscriptionFailed, id, TypeUtility.GetNicelyFormattedName(@delegate.GetType())));
            }
        }

        /// <summary>
        /// Unsubscribes a delegate from the event identified by the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the event.</param>
        /// <param name="delegate">The delegate to unsubscribe from the event.</param>
        /// <exception cref="ArgumentException">Both the event associated with <paramref name="id"/> and <paramref name="delegate"/> are not <see langword="null"/>, and they are not instances of the same delegate type.</exception>
        public void Unsubscribe(T id, Delegate @delegate)
        {
            if (@delegate == null)
            {
                return;
            }
            while (true)
            {
                if (!_delegates.TryGetValue(id, out var oldDelegate))
                {
                    return;
                }
                var newDelegate = Delegate.Remove(oldDelegate, @delegate);
                if (newDelegate != null)
                {
                    if (_delegates.TryUpdate(id, newDelegate, oldDelegate))
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
                Log.V(string.Format(UnsubscriptionFailed, id, TypeUtility.GetNicelyFormattedName(@delegate.GetType())));
            }
        }

        /// <summary>
        /// Publishes the event identified by the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the event.</param>
        /// <param name="args">An array of objects that are the arguments to pass to the event associated with <paramref name="id"/>.</param>
        /// <returns>The object returned by the event associated with <paramref name="id"/>, or <see langword="null"/> if <paramref name="id"/> is not found.</returns>
        /// <exception cref="TargetParameterCountException">The <paramref name="args"/> array does not have the correct number of arguments.</exception>
        /// <exception cref="ArgumentException">The element of the <paramref name="args"/> array do not match the signature of the event associated with <paramref name="id"/>.</exception>
        public object Publish(T id, params object[] args)
        {
            return _delegates.TryGetValue(id, out var @delegate) ? Invoke(@delegate, args) : null;
        }

        /// <summary>
        /// Publishes the event identified by the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the event.</param>
        /// <param name="args">An array of objects that are the arguments to pass to the event associated with <paramref name="id"/>.</param>
        /// <returns>An array of objects that are returned by each of the invocation list of the event associated with <paramref name="id"/>, or an empty array if <paramref name="id"/> is not found.</returns>
        /// <exception cref="TargetParameterCountException">The <paramref name="args"/> array does not have the correct number of arguments.</exception>
        /// <exception cref="ArgumentException">The element of the <paramref name="args"/> array do not match the signature of the event associated with <paramref name="id"/>.</exception>
        public object[] PublishAll(T id, params object[] args)
        {
            return _delegates.TryGetValue(id, out var @delegate) ? InvokeAll(@delegate, args) : Array.Empty<object>();
        }

        /// <summary>
        /// Removes all events in <see cref="EventBus{T}"/>.
        /// </summary>
        public void Clear()
        {
            _delegates.Clear();
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
        /// Removes a key and value from <see cref="_delegates"/>. Both the key and value must match the entry in the dictionary for it to be removed.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryRemove(T key, Delegate oldValue)
        {
            return _tryRemoveInternal(key, out _, true, oldValue);
        }
    }
}
