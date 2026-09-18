# Aurora

![license](https://img.shields.io/github/license/NOMOAX/aurora)
![version](https://img.shields.io/badge/version-1.5.3-blue)
![lowest Unity version](https://img.shields.io/badge/Unity-2021.2%2B-blue)

High-performance, low-memory-consumption C# toolkit for Unity.

English | [中文](README.zh.md)

## Installation

1. Open Unity package manager.
2. Click the `+` button in the upper-left corner, then select `Add package from git URL...`.
3. Input `https://github.com/NOMOAX/aurora.git` and then click `Add` button.

## Managed Code Stripping

`EventBus<T>` reaches the private `TryRemoveInternal` method of `ConcurrentDictionary<TKey, TValue>` through reflection. The Unity linker cannot see that reference during its static analysis, so this package ships a [link.xml](link.xml) that declares the method as a root.

**Unity only reads `link.xml` files under `Assets/`, so the copy inside this package is ignored.** Add it to your project by hand:

- If your project has no `link.xml`, copy `link.xml` from this package to `<Unity project path>/Assets`.
- If your project already has a `link.xml`, merge the contents of this one into it.

```xml

<linker>
    <assembly fullname="mscorlib">
        <type fullname="System.Collections.Concurrent.ConcurrentDictionary`2">
            <method name="TryRemoveInternal" />
        </type>
    </assembly>
</linker>
```

## Event Bus

`EventBus<T>` manages multicast delegates by event identifier, for decoupled communication between modules. It is built on `ConcurrentDictionary`, hence thread-safe, and supports delegates of any type.

```csharp
public static class Id
{
    public const int SendMessage = 1;
    public const int GetPhoneNumber = 2;
}

public static void OnMessageReceived(string sender, string content)
{
    Console.WriteLine($"{sender} : {content}");
}

EventBus<int>.Subscribe(Id.SendMessage, (Action<string>)OnMessageReceived); // registers the event
EventBus<int>.Unsubscribe(Id.SendMessage, (Action<string>)OnMessageReceived); // unregisters the event
EventBus<int>.Publish(Id.SendMessage, "Kevin", "Hello world!"); // publishes the event

// If the delegate type of the event has a return value, all return values can be collected at once when publishing
var phoneNumbers = (string[])EventBus<int>.PublishAll(Id.GetPhoneNumber);

// Waits for the event to be published
await EventBus<int>.WhenPublished(Id.SendMessage);

// A CancellationToken can also be passed in, to end the wait when canceled
await EventBus<int>.WhenPublished(Id.SendMessage, cancellationToken);
```

`WhenPublished` returns a `Task` that completes when the specified event is published by `Publish` / `PublishAll`. If the event has already been published before the call, that publication does not complete this call. `Clear` completes all the waiting `Task`s.

## Logging

`Log` provides the `V` / `D` / `I` / `W` / `E` log levels and writes to the console by default.

The `D` level is marked with `Conditional("DEBUG")`.

The output can be customized through `ILogger`, and the log level, the thread identifier and the timestamp can be configured.

```csharp
Log.I($"Loading resource: {path}");
Log.D("Debug information, output only in DEBUG builds");
Log.W("Not enough available disk space");
Log.E("The parameter is null");

Log.Level = LogLevel.I; // only the I / W / E levels are logged
Log.WithCurrentThreadId = true; // appends the identifier of the current thread
Log.DateTimeOffsetFormat = LogDateTimeOffsetFormat.S; // appends an ISO 8601 timestamp
Log.Logger = new MyLogger(); // switches to a custom implementation

public sealed class MyLogger : ILogger
{
    public void Log(object value, LogLevel logLevel)
    {
        // writes to a file, uploads to a server, and so on
    }
}
```

## Practical Collections

### Deque

`Deque<T>` is implemented with a circular buffer, so adding and removing at both the head and the tail cost `O(1)`. It implements `IList<T>` / `IReadOnlyList<T>`, supports indexed reads and writes, and grows automatically when the capacity is insufficient.

```csharp
var deque = new Deque<int>();
deque.EnqueueLast(10); // [10]
deque.EnqueueFirst(20); // [20, 10]
deque.EnqueueLast(30); // [20, 10, 30]

var head = deque.DequeueFirst(); // 20
var tail = deque.DequeueLast(); // 30

// Peeks at the head without removing it
if (deque.TryPeekFirst(out var first))
{
    // ...
}

// Dequeues safely when the deque is empty
if (deque.TryDequeueLast(out var last))
{
    // ...
}

deque.Insert(0, 99);
deque.RemoveAt(0);
deque[0] = 42; // reads and writes by index
deque.Reverse();
deque.TrimExcess(); // shrinks the capacity to close to the actual number of elements
var snapshot = deque.ToArray();
```

### Binary Heap

`BinaryHeap<T>` is a binary heap maintained by a comparer (a min-heap by default), suitable for priority queue scenarios. It supports bulk adding, taking the top element, removing by value and clearing.

```csharp
var heap = new BinaryHeap<int>();
heap.Add(3); // [3]
heap.Add(1); // [1, 3]
heap.AddRange(new[] { 2, 0, 5 }); // [0, 1, 2, 3, 5]

var count = heap.Count; // 5

heap.Peek(); // 0
heap.Take(); // 0
heap.Remove(5); // removes by value
heap.Clear();

// Custom priority: pass in a comparer
var binaryHeap = new BinaryHeap<Item>(
    Comparer<Item>.Create(
        (a, b) => Comparer<int>.Default.Compare(a.Priority, b.Priority)
    )
);
```

### Tree and Node

`Node` is a general-purpose tree node: it maintains the parent, the child list, the root node, the level and the version number. It supports adding and removing children and testing ancestor and descendant relationships; subclasses can override the protected hooks to constrain structural changes.

```csharp
var root = new Node();
var a = new Node();
var b = new Node();
root.Add(a);
root.Add(b);
a.Add(new Node());

root.IsRoot; // true
b.IsLeaf; // true
a.Level; // 1
root.Root; // the root node itself
root.IsParentOf(b); // true
b.IsChildOf(root); // true
```

```csharp
public class MyNode : Node
{
    private int _value;

    public int Value => _value;

    public MyNode(int value)
    {
        _value = value;
    }

    public override string ToString()
    {
        return _value.ToString();
    }
}

// The object initializer is used here, which actually calls the Add method
var rootNode = new MyNode(0)
{
    new MyNode(1)
    {
        new MyNode(2)
    },
    new MyNode(3),
    new MyNode(4)
};
```

#### Tree Traversal

`Node.GetEnumerator()` enumerates the direct children; `GetEnumerator(TreeEnumOrder)` traverses the whole tree in the specified order, and throws if the structure changes during enumeration (the version check keeps this safe).

```csharp
// Enumerates only the direct children
foreach (Node child in root)
{
    // ...
}

// Depth-first, from left to right
foreach (Node node in root.GetEnumerator(TreeEnumOrder.DepthFirstDlr))
{
    // ...
}

// Breadth-first, from left to right
foreach (Node node in root.GetEnumerator(TreeEnumOrder.BreadthFirstLr))
{
    // ...
}
```

`TreeEnumOrder` provides 7 orders: `Default` (direct children), `BreadthFirstLr`, `BreadthFirstRl`, `DepthFirstDlr`, `DepthFirstDrl`, `DepthFirstLrd`, `DepthFirstRld`.

The underlying enumerators (`LrEnumerator`, `RlEnumerator`, `DlrEnumerator`, `DrlEnumerator`, `LrdEnumerator`, `RldEnumerator` and their base classes) are public as well, so any "get the children" function can be passed in to traverse any tree-shaped data.

### Finite State Machine

`StateMachine<T>` is a general-purpose finite state machine, where `T` is the identifier type of the state. It supports enter / exit callbacks, immediate and scheduled transitions, and state protection during transitions.

```csharp
// Using System.Type as the state identifier type is recommended

public sealed class IdleState : IState<Type>
{
    Type IState<Type>.Id => typeof(IdleState);

    void IState<Type>.OnEnter(StateMachine<Type> stateMachine, IState<Type> from)
    {
        Console.WriteLine("Drink coffee.");
        stateMachine.ScheduleTransitionTo(typeof(WorkingState));
    }

    void IState<Type>.OnExit(StateMachine<Type> stateMachine, IState<Type> to)
    {
        // nothing to do when leaving this state
    }
}

public sealed class WorkingState : IState<Type>
{
    private int _current = 0;

    private int _total = 100;

    Type IState<Type>.Id => typeof(WorkingState);

    void IState<Type>.OnEnter(StateMachine<Type> stateMachine, IState<Type> from)
    {
        Console.WriteLine("Start working.");
        _current++;
        if (_current < _total)
        {
            stateMachine.ScheduleTransitionTo(typeof(IdleState));
        }
        else
        {
            stateMachine.ScheduleTransitionToNull();
        }
    }

    void IState<Type>.OnExit(StateMachine<Type> stateMachine, IState<Type> to)
    {
        if (_current < _total)
        {
            Console.WriteLine("Have a rest.");
        }
        else
        {
            Console.WriteLine("Stop working.");
        }
    }
}

var stateMachine = new StateMachine<Type>();
stateMachine.AddState(new IdleState());
stateMachine.AddState(new WorkingState());
stateMachine.ScheduleTransitionTo(typeof(IdleState));

Console.WriteLine(stateMachine.CurrentState); // null
// Keeps updating the state machine until it is stopped
while (stateMachine.Update())
{
    Console.WriteLine(stateMachine.CurrentState); // IdleState or WorkingState
}
Console.WriteLine(stateMachine.CurrentState); // null
```

| Method                     | Who may call it                       | When it takes effect |
|----------------------------|---------------------------------------|----------------------|
| `TransitionTo`             | The direct owner of the state machine | Immediately          |
| `TransitionToNull`         | The direct owner of the state machine | Immediately          |
| `ScheduleTransitionTo`     | Anyone                                | The next `Update`    |
| `ScheduleTransitionToNull` | Anyone                                | The next `Update`    |

#### Blackboard

`Blackboard` is a dictionary that always uses `string` as its key, a companion tool for passing data between the states of a state machine.

```csharp
var blackboard = new Blackboard();
blackboard.SetValue("hp", 100);

var hp = blackboard.GetValue<int>("hp");
if (blackboard.TryGetValue("hp", out var hp1))
{
    Console.WriteLine(hp1);
}

var removed = blackboard.GetAndRemoveValue<int>("hp"); // gets the value and removes it at the same time

blackboard.Remove("hp");
blackboard.Clear();

// Works together with a state machine
stateMachine.Blackboard.SetValue("coin", 100);
```

### Fixer

`Fixer` derives from `Node` and models "a tree that needs fixing" as a tree of nodes: every node provides a test of "whether it is ready" and an "asynchronous fix" operation, along with a priority (a smaller one is handled first). The whole tree is fixed recursively in priority order until every node is ready.

A simple example:

- Suppose you are developing a note-taking app.
- There is a note window (referred to as `A` below), which has a close button.
- Under `A`, there is an image list child window (referred to as `B` below) and a note body child window (referred to as `C` below).
- Suppose the user is uploading an image in `B`, but the upload is still in progress; the asynchronous upload task is `B.uploadTask`.
- Suppose the user has edited the note body and clicked the save button of `C` to upload the new note body to the server, but the request is still in progress; the asynchronous request task is `C.saveTask`.
- At this point, the user clicks the close button of `A`.
- Check the value of `A.Fixer`: it is `true` itself, but its children have `B.Fixer.IsFixed == false` and `C.Fixer.IsFixed == false`, so `A.Fixer.IsFixed == false`.
- A dialog pops up, letting the user choose whether to wait for all the asynchronous tasks to complete or to close `A` immediately.
- Suppose the user chooses `wait for all the asynchronous tasks to complete`.
- Execute `await A.Fixer.FixAsync()`, with a reasonable timeout set.
- When the method returns, check the value of `A.Fixer.IsFixed` again; if it is `true`, `A` can be closed.
- If `A.Fixer.IsFixed == false`, pop up again to remind the user that `the upload failed / the save failed`, and block this close operation of `A`; the user can retry `uploading the image / saving the note body` at will.

```csharp
var fixerA = new Fixer();

var fixerB = new Fixer(() => _uploadTask == null || _uploadTask.IsCompletedSuccessfully);
fixerA.Add(fixerB);

var fixerC = new Fixer(() => _saveTask == null || _saveTask.IsCompletedSuccessfully);
fixerA.Add(fixerC);

private async void OnCloseButtonClicked()
{
    if (fixerA.IsFixed)
    {
        Close();
    }
    else
    {
        // Pops up a dialog, letting the user choose a solution
        int result;
        {
            var dialog = new Dialog();
            dialog.Show(
                "There is still unsaved work. Continue?",
                new Option[]
                {
                    new Option("Save and close", () => 0),
                    new Option("Close", () => 1),
                    new Option("Cancel", () => 2)
                }
            );
            result = await dialog.WaitForSelectionAsync();
            dialog.Close();
        }
        switch (result)
        {
            // Save and close
            case 0:
            {
                bool isFixed;
                bool isCanceled;
                fullScreenMask.SetActive(true); // covers the whole screen, to keep the user from clicking UI elements at this point
                try
                {
                    // Equivalent to awaiting fixerA.FixAsync() first and then returning the latest value of fixerA.IsFixed
                    // A CancellationToken can be passed in, to specify a timeout
                    isFixed = await fixerA.EnsureFixedAsync();
                    isCanceled = false;
                }
                catch (OperationCanceledException)
                {
                    isFixed = false;
                    isCanceled = true;
                }
                finally
                {
                    fullScreenMask.SetActive(false); // do not forget this
                }
                if (isCanceled)
                {
                    Log.W("The upload image / save note body request timed out. Please retry.");
                }
                else if (isFixed)
                {
                    Log.I("Upload image / save note body succeeded.");
                    Close();
                }
                else
                {
                    Log.W("Upload image / save note body failed. Please retry.");
                }
                break;
            }
            // Close
            case 1:
            {
                // Waits for _uploadTask and _saveTask in a new context, ensuring that their exceptions are caught
                if (_uploadTask != null)
                {
                    if (!_uploadTask.IsCompleted)
                    {
                        TaskUtility.BeginAwait(_uploadTask);
                    }
                    _uploadTask = null;
                }
                if (_saveTask != null)
                {
                    if (!_saveTask.IsCompleted)
                    {
                        TaskUtility.BeginAwait(_saveTask);
                    }
                    _saveTask = null;
                }
                Close();
                break;
            }
            // Cancel
            case 2:
            {
                // The user clicked cancel, so nothing is done
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(result));
            }
        }
    }
}
```

### Enumerator Tools

`EnumeratorEnumerable` / `EnumeratorEnumerable<T>` wraps an existing `IEnumerator` into an `IEnumerable`, which makes `foreach` and LINQ convenient; `NullEnumerator` / `NullEnumerator<T>` is the null-object pattern and produces no element at all.

```csharp
IEnumerator<int> enumerator = GetExistingEnumerator(); // an existing enumerator
IEnumerable<int> enumerable = new EnumeratorEnumerable<int>(enumerator);
foreach (var item in enumerable)
{
    // iterates directly
}

// An empty sequence (a singleton)
IEnumerable<int> empty = new EnumeratorEnumerable<int>(NullEnumerator<int>.Instance);
```

## Collection Extensions

### Array Extension Methods

`ArrayExtensions` provides shuffling, searching and conversion extension methods for arrays. The searching and conversion methods have "state parameter" overloads, which avoid closure allocations when the context needs to be captured.

```csharp
int[] numbers = { 1, 2, 3, 4, 5 };

// The criterion is passed through state, which avoids a closure allocation
int firstElementGreaterThanThree = numbers.Find((element, state) => element > state, 3); // 4
int firstElementIndexGreaterThanThree = numbers.FindIndex((element, state) => element > state, 3); // 3
int lastElementLessThanThree = numbers.FindLast((element, state) => element < state, 3); // 2
int lastElementIndexLessThanThree = numbers.FindLastIndex((element, state) => element < state, 3); // 1

numbers.ShuffleInPlace(); // shuffles in place
```

### List Extension Methods

`ListExtensions` provides the list counterparts of the array extensions, plus bulk removal by condition.

```csharp
List<int> list = new() { 1, 2, 3, 4, 5 };

int firstElementGreaterThanThree = list.Find((element, state) => element > state, 3); // 4
int removedElementCount = list.RemoveAll((element, state) => element > state, 3); // removes every element greater than 3, returning the removed count

list.ShuffleInPlace(); // shuffles in place
```

### Sequence Index Lookup

`EnumerableExtensions` provides the sequence operations that `System.Linq.Enumerable` does not provide: looking up the **index** of an element in any `IEnumerable<T>`, by value or by predicate.

```csharp
IEnumerable<string> sequence = GetNames(); // any sequence
int i = sequence.IndexOf("target"); // the index of the first match, or -1 if not found
int j = sequence.LastIndexOf("target");
int k = sequence.FindIndex(s => s.StartsWith("A"));
```

## Sorting

### Quick Sort

`QuickSort` uses the standard quicksort algorithm, but avoids recursion internally, so it does not overflow the stack because the sequence to be sorted is too long.

```csharp
List<int> list = new() { 5, 3, 8, 1, 9 };
QuickSort.Sort(list); // the default comparer
QuickSort.Sort(list, Comparer<int>.Default); // a specified comparer
QuickSort.Sort(list, 1, 3); // sorts only part of the range

var items = new List<Item>();
QuickSort.Sort(items, Comparer<Item>.Create((a, b) => a.Price.CompareTo(b.Price))); // a custom comparer
QuickSort.Sort(keys, values); // if elements in keys are swapped, the elements at the corresponding positions in values are swapped accordingly, so that a key and its value always match
```

### Tim Sort

`TimSort` is a replication of the original logic from the Java source code. It provides the same capabilities as `QuickSort` (in-place sorting, ranges, custom comparers, synchronized key-value sorting) and is slightly slower than `QuickSort`, but it is stable — equal elements keep their relative order before and after sorting.

```csharp
List<int> list = new() { 5, 3, 8, 1, 9 };
TimSort.Sort(list); // the default comparer
TimSort.Sort(list, Comparer<int>.Default); // a specified comparer
```

### Comparer Utilities

- `ComparerExtensions.Reversed<T>()`: reverses the comparison result
- `ReversedComparer<T>`: a reversed comparer, providing `Default`
- `FunctorComparer<T>`: constructs a comparer from a `Comparison<T>` delegate
- `HashCodeComparer<T>`: compares by `GetHashCode`

```csharp
IComparer<int> descending = Comparer<int>.Default.Reversed();
IComparer<Item> byName = new FunctorComparer<Item>((a, b) => string.Compare(a.Name, b.Name));
QuickSort.Sort(items, byName);
```

## Object Pooling

The object pooling APIs (`Pool<T>`, `IPool<T>`, `IPooledObjectPolicy<T>`) reuse objects that are created and destroyed frequently, which reduces the GC pressure. `Pool<T>` is thread-safe, and the "whether it can be reused" test and the reset logic can be customized on returning.

### Predefined Pools

`PredefinedPools` provides ready-made pools for common types; just borrow on demand and return when done:

- `ByteArrayLength4096` (`byte[4096]`), `MemoryStream`, `Stopwatch`, `StringBuilder`
- `PredefinedPools<T>`: `ArrayLength2`, `ArrayLength4`, `ArrayLength8`, `HashSet<T>`, `List<T>`, `Queue<T>`, `Stack<T>`, `Deque<T>`
- `PredefinedPools<T1, T2>`: `Dictionary<TKey, TValue>`

```csharp
var builder = PredefinedPools.StringBuilder.Get();
try
{
    builder.Append("...");
    // uses the builder
}
finally
{
    PredefinedPools.StringBuilder.Return(builder); // returns it and resets it
}
```

### Automatic Borrow and Return with UsingScope

Each `UsingScope` class wraps "acquire → use → return" into `IDisposable`, so a `using` statement performs the borrowing and the returning automatically:

`ListUsingScope`, `DictionaryUsingScope`, `HashSetUsingScope`, `QueueUsingScope`, `StackUsingScope`, `DequeUsingScope`, `ArrayLength2UsingScope`, `ArrayLength4UsingScope`, `ArrayLength8UsingScope`, `ByteArray4096UsingScope`, `MemoryStreamUsingScope`, `StopwatchUsingScope`, `StringBuilderUsingScope`

```csharp
using (var scope = new ListUsingScope<int>(out var list))
{
    list.Add(1);
    list.Add(2);
    // returns it automatically on leaving the scope
}

using var stringBuilderScope = new StringBuilderUsingScope(out var sb);
sb.Append("...");
```

Custom object pool: implement the create, get, return and dispose policies of `IPooledObjectPolicy<T>` and hand them to `Pool<T>` to manage.

```csharp
public sealed class PooledBulletPolicy : IPooledObjectPolicy<Bullet>
{
    public Bullet Create()
    {
        return new Bullet();
    }

    public void Get(Bullet obj)
    {
        // activates it on getting
        obj.Active = true;
    }

    public bool Return(Bullet obj)
    {
        if (obj == null)
        {
            return false; // false means it cannot be reused
        }
        obj.Active = false; // resets it on returning
        return true; // true means it can be reused
    }

    public void Dispose(Bullet obj)
    {
        // releases resources when the pool is destroyed or refuses to reuse it
        obj?.Dispose(); // releases the resources held by the Bullet itself
    }
}

IPool<Bullet> pool = new Pool<Bullet>(new PooledBulletPolicy(), maximumRetained: 64);
Bullet bullet = pool.Get();
pool.Return(bullet);
```

## Awaitable Expressions

Custom awaiters based on the C# awaitable convention, used for context switching inside `async` methods.

- `NullAwaitable`: completes immediately, used to eliminate the CS1998 (async method lacks await) warning; use it only for temporary purposes.
- `SynchronizationContextAwaitable`: switches to a specified `SynchronizationContext` (such as the main thread)
- `ThreadPoolThreadAwaitable`: switches to a thread pool thread

```csharp
public async Task HeavyWorkAsync()
{
    // Switches to the thread pool, to avoid blocking the main thread
    await new ThreadPoolThreadAwaitable(CancellationToken.None);
    DoHeavyWork();

    // Switches back to the main thread (the synchronization context saved on the main thread)
    await new SynchronizationContextAwaitable(_mainContext);
    ApplyResult(); // operations that require the main thread can be performed safely here
}

public async Task PlaceholderAsync()
{
    await new NullAwaitable(); // eliminates the CS1998 warning
    // TODO the logic is not implemented yet
}
```

## Multithreading and Tasks

### Task Utilities

`TaskUtility` provides helpers for task error handling, state forwarding and synchronous continuation execution:

- `BeginAwait`: waits for a task in a new context, without blocking the current program execution flow
- `ThrowIfFaultedOrCanceled`: throws the underlying exception when the task is faulted, and `TaskCanceledException` when it is canceled
- `GetBaseException`: gets the root exception of a faulted task
- `HandleFaultsAndCancellation`: when the task is faulted or canceled, correspondingly puts the `TaskCompletionSource` into the faulted or canceled state
- `ContinueWithSynchronously`: creates a continuation task that executes synchronously

### Async Barrier

`AsyncBarrier` blocks the participants until the specified number of participants have all signalled, then they continue together; it suits stage synchronization of multiple tasks.

```csharp
var barrier = new AsyncBarrier(3); // 3 participants

// Each of the three concurrent tasks executes:
await barrier.SignalAndWait(); // waits for all the other participants to arrive
// All of them continue after all have arrived
```

### `CancellationTokenSourceUtility`

`CancellationTokenSourceUtility.IsDisposed` detects whether a `CancellationTokenSource` has been disposed (the official API offers no public way to detect this).

## Random Utilities

- `RandomUtility`: the global random utility (which uses `AuroraRandom.Instance` internally)
- `P(probability)`: returns a boolean according to a probability
- `Choose` / `TryChoose`: picks one element from a collection at random by weight, optionally only within a range of the collection
- `GetChosenIndex`: returns the index of the element chosen by weight; the parameters take the same forms as above
- `AuroraRandom`: a class derived from `Random`; `Instance` is the instance of the current thread (one per thread, which avoids contention between threads)
    - `NextDoubleIncludingOne()`: returns a `double` in the closed interval `[0, 1]`

```csharp
if (RandomUtility.P(0.3))
{
    // 30% probability
}

var items = new List<string> { "common", "rare", "epic" };
var weights = new List<double> { 0.7, 0.2, 0.1 };
var chosen = RandomUtility.Choose(items, weights); // picks by weight
var ok = RandomUtility.TryChoose(items, weights, out var picked);
var index = RandomUtility.GetChosenIndex(weights);

// Picks only within the specified range of the collection
var chosenInRange = RandomUtility.Choose(items, weights, 1, 2);

var d = AuroraRandom.Instance.NextDoubleIncludingOne(); // the closed interval [0, 1]
```

## Enumeration Utilities

`EnumUtility<TEnum>` provides the common reflection and validation operations of an enumeration; the results are cached statically, so the performance is good.

```csharp
foreach (string name in EnumUtility<MyEnum>.Names)
{
    // all the names
}
MyEnum[] values = EnumUtility<MyEnum>.Values; // all the values
int count = EnumUtility<MyEnum>.Count;
Type underlying = EnumUtility<MyEnum>.UnderlyingType;
bool isBitwise = EnumUtility<MyEnum>.IsBitwise; // whether it is a bitwise enumeration

bool isDefined = EnumUtility<MyEnum>.IsDefined(MyEnum.Value1);

FieldInfo fieldInfo = EnumUtility<MyEnum>.GetFieldInfo("Value1");
bool isObsolete = EnumUtility<MyEnum>.IsObsolete(MyEnum.Value1); // whether it is marked with Obsolete
```

## IO Utilities and `Stream` / `TextReader` Extensions

- `IOUtility`: queries the remaining space of a drive, throws a dedicated exception when it is insufficient, and creates an empty file of a specified length
- `PathUtility`: path string operations (replacing the path separators, computing a relative path and reporting the relationship between the two paths)
- `StreamExtensions`: `CopyToFrugally` / `CopyToFrugallyAsync`, which copy a stream reusing a buffer pool, reducing allocations
- `TextReaderExtensions`: `SkipWhiteSpaces`, which skips consecutive white space characters
- Companion exceptions: `FileTooLargeException`, `NotEnoughAvailableFreeSpaceOnDriveException`

```csharp
long free = IOUtility.GetAvailableFreeSpaceOnDrive(@"D:\");
IOUtility.ThrowIfNotEnoughAvailableFreeSpaceOnDrive(@"D:\", 1024 * 1024); // throws when it is insufficient

IOUtility.CreateEmptyFile(@"D:\data.bin", 1024 * 1024); // creates a 1 MB empty file

using (var src = File.OpenRead("in.bin"))
using (var dst = File.Create("out.bin"))
{
    src.CopyToFrugally(dst); // copies reusing a buffer pool
    // The asynchronous version: await src.CopyToFrugallyAsync(dst, cancellationToken);
}

using var reader = new StringReader("   hello");
reader.SkipWhiteSpaces(); // skips the leading white space
```

`PathUtility.GetRelativePath` returns the relative path and, at the same time, tells you the relationship between the two paths through `PathRelationship`: `IsChildOf` (a child), `IsEqualTo` (equal), `IsNeitherChildOfNorEqualTo` (neither a child nor equal), `AreUnrelated` (no common root).

```csharp
var relative = PathUtility.GetRelativePath(@"C:\A", @"C:\A\B\C", out var relationship);
// relative is the relative path (such as "B\C"), and relationship is PathRelationship.IsChildOf

var equal = PathUtility.GetRelativePath(@"C:\A", @"C:\A", out var relationship1);
// equal is ".", and relationship1 is PathRelationship.IsEqualTo

var unrelated = PathUtility.GetRelativePath(@"C:\A", @"D:\B", out var relationship2);
// unrelated is null, and relationship2 is PathRelationship.AreUnrelated

var normalized = PathUtility.ReplaceBackslashWithForwardSlash(@"C:\data\file.txt"); // "C:/data/file.txt"
```

## Time Measurement and Safe Counting

- `ValueStopwatch`: a value-type stopwatch, which avoids boxing and heap allocation
- `CountIncrementSafeHandler`: when the number of executions of a loop or a recursion cannot be predicted, gives a reasonable maximum execution count and treats exceeding the limit as an exception

## Interpolation and Easing

The `Interpolation` enumeration provides 31 easing modes (Linear; In / Out / InOut of Sine, Quad, Cubic, Quart, Quint, Expo, Circ, Back, Elastic and Bounce); `InterpolationUtility` provides interpolation, inverse interpolation and mode conversion.

```csharp
double t = 0.5; // the weight [0, 1]

double value = InterpolationUtility.Interpolate(0.0, 1.0, t, Interpolation.InOutCubic);
double eased = InterpolationUtility.Transform(t, Interpolation.OutBack); // converts it into an easing weight
double weight = InterpolationUtility.InverseLinearInterpolate(0.0, 10.0, 7.5); // inverse interpolation, to get the weight
```

## Bit Operations

`BitUtility.UnsignedRightShift` provides the unsigned right shift (`int` / `uint` / `long` / `ulong`), that is, the equivalent of the `>>>` operator before C# 11.

```csharp
var value = BitUtility.UnsignedRightShift(-1, 1); // 2147483647 (0x7FFFFFFF); whereas -1 >> 1 gives -1
```

## Other Small Utility Classes

### Hexadecimal Character Parsing

`HexCharParseUtility` provides parsing of hexadecimal characters (`0-9`, `A-F`, `a-f`).

```csharp
byte b = HexCharParseUtility.Parse('F'); // 15; an invalid character throws ArgumentOutOfRangeException
byte b2 = HexCharParseUtility.ParseNoCheck('F'); // the unchecked fast version
bool ok = HexCharParseUtility.TryParse('G', out byte v); // false
```

### Temporary ID Generation

`TempIdGenerator` generates a temporary ID with a prefix / suffix, whose middle is the `N` format string of a `Guid`, and provides a format match test.

```csharp
var generator = new TempIdGenerator("obj_", "");
string id = generator.NewTempId; // for example "obj_3f2a9c1e..."
bool matched = generator.Match(id); // tests whether the ID matches the format of this generator
```

### Friendly Type Name Formatting

`TypeUtility.GetNicelyFormattedName` outputs a readable type name: built-in types are mapped to the C# keywords, and the array, nullable, nested and generic forms are handled as well.

```csharp
string s = TypeUtility.GetNicelyFormattedName(typeof(List<int>)); // System.Collections.Generic.List<int>
string t = TypeUtility.GetNicelyFormattedName(typeof((int, string))); // (int,string)
```

### English Utilities

`EnglishUtility` provides small utilities for English text.

```csharp
var pluralized = EnglishUtility.Pluralize("apple", "apples", 3); // "apples"
var singularized = EnglishUtility.Pluralize("apple", "apples", 1); // "apple"
var ordinal = EnglishUtility.Th(21); // "st", that is, $"{21}{ordinal}" == "21st"
```

### Invocation Family

Wraps a piece of logic into an invocation object that can be delayed and passed around; `OneTimeInvocation` guarantees that the whole piece of logic really executes only once (thread-safe).

```csharp
Invocation a = new InvocationAction(() =>
{
    Console.WriteLine("Executes");
});
a.Invoke(); // executes immediately

Invocation<int> b = Invocation<int>.FromResult(42); // a fixed return value, set at construction
int result = b.Invoke(); // 42

Invocation once = new OneTimeInvocation(a);
once.Invoke(); // really executes the first time
once.Invoke(); // does not execute afterwards
```

### Hash Code Combination

`HashHelper.CombineHashCodes` combines several hash codes into a single hash code; it provides overloads for 2 to 16 parameters and a `params` array version.

```csharp
int hash = HashHelper.CombineHashCodes(a, b, c);
int hash2 = HashHelper.CombineHashCodes(values); // params int[]
```

### Empty Result and Null Disposable Object

`VoidResult` explicitly represents "no result", suitable for signalling scenarios such as `TaskCompletionSource<VoidResult>` that only care about completion / cancellation / exception; `NullDisposable` is the null-object implementation of `IDisposable` (a singleton).

```csharp
var done = new TaskCompletionSource<VoidResult>();
done.TrySetResult(new VoidResult()); // a completion signal

// Under some compilation conditions a real IDisposable is needed, and under others only a formal placeholder is
using (
#if XXX
    new Xxx()
#else
    NullDisposable.Instance
#endif
)
{
    // ...
}
```

### Miscellaneous

- `UnexpectedException`: represents that something unexpected happened
- `CommentAttribute`: an attribute for writing a description of a member that does not support XML documentation comments; the description can be viewed quickly in the IDE
- `Environment`: `IsSingleThreadEnvironment`, a runtime environment flag
- `Constant`: commonly used constants
    - `Constant.String`: string constants, such as the `G9` / `G17` floating-point round-trip formats
    - `Constant.Regex.EmailAddressRegex`: a regular expression that matches email addresses conforming to the RFC 5322 standard, precompiled with `RegexOptions.Compiled` and ready to reuse without creating one each time
    - `Constant.Character`: a large number of Unicode character constants

```csharp
if (Aurora.Environment.IsSingleThreadEnvironment)
{
    // the optimized path in a single-threaded environment
}

// Email address validation
bool isEmailAddress = Constant.Regex.EmailAddressRegex.IsMatch(input);
```
