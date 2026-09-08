# Aurora

![许可](https://img.shields.io/github/license/NOMOAX/aurora)
![版本](https://img.shields.io/badge/version-1.0.0-blue)
![最低 Unity 版本](https://img.shields.io/badge/Unity-2021.2%2B-blue)

适用于 Unity 的高性能、低内存消耗的 C# 工具包。

[English](README.md) | 中文

## 安装

1. 打开 Unity package manager。
2. 点击左上角的 `+` 按钮，然后选择 `Add package from git URL...`。
3. 填入 `https://github.com/NOMOAX/aurora.git` 并点击 `Add` 按钮。

## 事件总线

`EventBus<T>` 以事件标识符管理多播委托，用于模块间的解耦通信。基于 `ConcurrentDictionary` 实现，线程安全，支持任意类型的委托。

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

EventBus<int>.Subscribe(Id.SendMessage, (Action<string>)OnMessageReceived);   // 注册事件
EventBus<int>.Unsubscribe(Id.SendMessage, (Action<string>)OnMessageReceived); // 取消注册事件
EventBus<int>.Publish(Id.SendMessage, "Kevin", "Hello world!");               // 发布事件

// 如果事件的委托类型具有返回值，则可以在发布事件时一次性收集所有返回值
var phoneNumbers = (string[])EventBus<int>.PublishAll(Id.GetPhoneNumber);
```

## 日志

`Log` 提供 `V` / `D` / `I` / `W` / `E` 五个级别的日志，默认输出到控制台。

`D` 级别标记了 `Conditional("DEBUG")`。

可以通过 `ILogger` 自定义输出方式，并配置日志级别、线程标识与时间戳。

```csharp
Log.I($"加载资源: {path}");
Log.D("调试信息，仅 DEBUG 构建时输出");
Log.W("可用磁盘空间不足");
Log.E("参数为 null");

Log.Level = LogLevel.I;                               // 只记录 I / W / E 级别
Log.WithCurrentThreadId = true;                       // 附带当前线程标识
Log.DateTimeOffsetFormat = LogDateTimeOffsetFormat.S; // 附带 ISO 8601 时间戳
Log.Logger = new MyLogger();                          // 切换自定义实现

public sealed class MyLogger : ILogger
{
    public void Log(object value, LogLevel logLevel)
    {
        // 写入文件、上传服务器等
    }
}
```

## 实用的集合

### 双端队列

`Deque<T>` 使用环状缓冲区实现，从头部与尾部增删均为 O (1)。实现 `IList<T>` / `IReadOnlyList<T>`，支持按索引读写，容量不足时自动增长。

```csharp
var deque = new Deque<int>();
deque.EnqueueLast(10);  // [10]
deque.EnqueueFirst(20); // [20, 10]
deque.EnqueueLast(30);  // [20, 10, 30]

var head = deque.DequeueFirst(); // 20
var tail = deque.DequeueLast();  // 30

// 不移除地查看头部
if (deque.TryPeekFirst(out var first)) 
{
    // ...
}

// 队空时安全取出
if (deque.TryDequeueLast(out var last)) 
{
    // ...
}

deque.Insert(0, 99);
deque.RemoveAt(0);
deque[0] = 42; // 按索引读写
deque.Reverse();
deque.TrimExcess(); // 收缩容量到接近实际元素数
var snapshot = deque.ToArray();
```

### 二叉堆

`BinaryHeap<T>` 是按比较器维护的二叉堆（默认最小堆），适合优先队列场景。支持批量添加、取出顶部元素、按值移除与清空。

```csharp
var heap = new BinaryHeap<int>();
heap.Add(3); // [3]
heap.Add(1); // [1, 3]
heap.AddRange(new[] { 2, 0, 5 }); // [0, 1, 2, 3, 5]

var count = heap.Count; // 5

heap.Peek();    // 0
heap.Take();    // 0
heap.Remove(5); // 按值移除
heap.Clear();

// 自定义优先级：传入比较器
var binaryHeap = new BinaryHeap<Item>(
    Comparer<Item>.Create(
        (a, b) => Comparer<int>.Default.Compare(a.Priority, b.Priority)
    )
);
```

### 树与节点

`Node` 是通用树节点：维护父节点、子节点列表、根节点、层级与版本号。支持增删子节点、判断祖先与后代关系；子类可重写受保护钩子以约束结构变化。

```csharp
var root = new Node();
var a = new Node();
var b = new Node();
root.Add(a);
root.Add(b);
a.Add(new Node());

root.IsRoot;   // true
b.IsLeaf;      // true
a.Level;       // 1
root.Root;     // 根节点自身
root.IsParentOf(b);   // true
b.IsChildOf(root);    // true
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

// 使用对象初始化器，底层实际调用 Add 方法
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

#### 树的遍历

`Node.GetEnumerator()` 枚举直接子节点；`GetEnumerator(TreeEnumOrder)` 按指定次序遍历整棵树，枚举期间结构发生变化会抛出异常（版本校验保证安全）。

```csharp
// 只枚举直接子节点
foreach (Node child in root)
{
    // ...
}

// 深度优先、从左到右
foreach (Node node in root.GetEnumerator(TreeEnumOrder.DepthFirstDlr))
{
    // ...
}

// 广度优先、从左到右
foreach (Node node in root.GetEnumerator(TreeEnumOrder.BreadthFirstLr))
{
    // ...
}
```

`TreeEnumOrder` 提供 7 种次序：`Default`（直接子节点）、`BreadthFirstLr`、`BreadthFirstRl`、`DepthFirstDlr`、`DepthFirstDrl`、
`DepthFirstLrd`、`DepthFirstRld`。

底层枚举器（`LrEnumerator`、`RlEnumerator`、`DlrEnumerator`、`DrlEnumerator`、`LrdEnumerator`、`RldEnumerator`
及其基类）也公开可用，可传入任意的"取子节点"函数来遍历任何树形数据。

### 有限状态机

`StateMachine<T>` 是通用有限状态机，`T` 为状态的标识符类型。支持进入/退出回调、立即切换与延迟切换、切换期间的状态保护。

```csharp
// 建议使用 Type 作为状态的标识符类型

public sealed class IdleState : IState<Type>
{
    IState<Type>.Id => typeof(IdleState);

    void IState<Type>.OnEnter(StateMachine<Type> stateMachine, IState<Type> from)
    {
        Console.WriteLine("Drink coffee.");
        stateMachine.ScheduleTransitionTo(typeof(WorkingState));
    }

    void IState<Type>.OnExit(StateMachine<Type> stateMachine, IState<Type> to)
    {
    }
}

public sealed class WorkingState : IState<Type>
{
    private int _current = 0;

    public int _total = 100;

    Type IState<Type>.Id => OperationId.Working;

    void IState<Type>.OnEnter(StateMachine<Type> stateMachine, IState<Type> from)
    {
        Console.WriteLine*("Start working.");
        _current++;
        if (_current < _total)
        {
            stateMachine.ScheduleTransitionTo(typeof(IdleState));
        }
        else
        {
            stateMachine.ScheduleTransitionTo(null);
        }
    }

    void IState<Type>.OnExit(StateMachine<Type> stateMachine, IState<Type> to)
    {
        if (_current < _total)
        {
            Console.WriteLine*("Have a rest.");
        }
        else
        {
            Console.WriteLine*("Stop working.");
        }
    }
}

var stateMachine = new StateMachine<Type>();
stateMachine.AddState(new IdleState());
stateMachine.AddState(new WorkingState());
stateMachine.ScheduleTransitionTo(Type.Idle);

Console.WriteLine(stateMachine.CurrentState); // null
// 持续更新状态机，直到中止
while (stateMachine.Update())
{
    Console.WriteLine(stateMachine.CurrentState); // IdleState 或 WorkingState
}
Console.WriteLine(stateMachine.CurrentState); // null
```

| 方法                       | 允许被谁调用   | 何时生效      |
|----------------------------|----------------|---------------|
| `TransitionTo`             | 状态机的持有者 | 立即          |
| `TransitionToNull`         | 状态机的持有者 | 立即          |
| `ScheduleTransitionTo`     | 任意           | 下一次 Update |
| `ScheduleTransitionToNull` | 任意           | 下一次 Update |

#### 黑板

`Blackboard` 是一个固定以 `string` 为键的字典，作为状态机各状态之间传递数据的配套工具。

```csharp
var blackboard = new Blackboard();
blackboard.SetValue("hp", 100);

var hp = blackboard.GetValue<int>("hp");
if (blackboard.TryGetValue("hp", out var hp1))
{
    Console.WriteLine(hp1);
}

blackboard.Remove("hp");
blackboard.Clear();

// 与状态机配合
stateMachine.Blackboard.SetValue("coin", 100);
```

### 修复器

`Fixer` 继承 `Node`，把"需要修复的树"建模为一棵节点树：
每个节点提供"是否已就绪"的判定与"异步修复"的操作，并带优先级（小的先处理）。按优先级递归修复整棵树，直到所有节点就绪。

一个简单的例子：

- 假设你正在 Unity 中开发笔记功能
- 有一个笔记窗口（以下简称为 `A`），它有关闭按钮
- 在 `A` 下，有一个图片列表子窗口（以下简称为 `B`）和一个笔记正文子窗口（以下简称为 `C`）
- 假设用户正在 `B` 中上传图片，但仍在上传中，上传的异步任务为 `B.uploadTask`
- 假设用户修改了笔记正文，点击了 `C` 的保存按钮，上传新的笔记正文到服务器，但仍在请求中，请求的异步任务为 `C.saveTask`
- 这时候用户点击了 `A` 关闭按钮
- 检查 `A.Fixer` 的值，它本身是 `true`，但它的子级 `B.Fixer.IsFixed == false` `C.Fixer.IsFixed == false`，所以
  `A.Fixer.IsFixed == false`
- 弹出一个对话框，让用户选择是要等待所有异步任务完成，还是立即关闭 `A`
- 假设用户选择了 `等待所有异步任务完成`
- 执行 `await A.Fixer.FixAsync()`，并设置一个合理的超时时间
- 当方法返回值，再次检查 `A.Fixer.IsFixed` 的值，如果为 `true`，就可以关闭 `A` 了
- 如果 `A.Fixer.IsFixed == false`，则可以再弹窗提醒用户 `上传失败/保存失败`，并阻止这次关闭 `A` 的操作，用户可以自行重试
  `上传图片/保存笔记正文`

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
        // 弹出对话框，让用户选择一个解决方案
        int result;
        {
            var dialog = new Dialog();
            dialog.Show(
                "仍有未保存的工作，是否继续？",
                new Option[]
                {
                    new Option("保存并关闭", () => 0),
                    new Option("关闭", () => 1),
                    new Option("取消", () => 2)
                }
            );
            result = await dialog.WaitForSelectionAsync();
            dialog.Close();
        }
        switch (result)
        {
            // 保存并关闭
            case 0:
                bool isFixed;
                bool isCanceled;
                fullScreenMask.SetActive(true); // 挡住整个屏幕，防止用户这时候点击界面元素
                try
                {
                    // 相当于先执行 await fixerA.FixAsync()，然后返回最新的 fixerA.IsFixed 的值
                    // 可以传入 CancellationToken，以指定超时时间
                    isFixed = await fixerA.EnsureFixedAsync();
                    isCanceled = false;
                }
                catch (OperationCanceledException e)
                {
                    isFixed = false;
                    isCanceled = true;
                }
                finally
                {
                    fullScreenMask.SetActive(false); // 别忘了这个
                }
                if (isCanceled)
                {
                    Log.W("上传图片/保存笔记正文请求超时，请重试");
                }
                else if (isFixed)
                {
                    Log.I("上传图片/保存笔记正文成功");
                    Close();
                }
                else
                {
                    Log.W("上传图片/保存笔记正文失败，请重试");
                }
                break;
            // 关闭
            case 1:
                // 在新的上下文中等待 _uploadTask 和 _saveTask，确保它们的异常会被捕获
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
            // 取消
            case 2:
                // 用户点击了取消，什么都不做
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result));
        }
    }
}
```

### 枚举器工具

`EnumeratorEnumerable` / `EnumeratorEnumerable<T>` 把已有的 `IEnumerator` 包装为 `IEnumerable`，便于 `foreach` 与 LINQ；
`NullEnumerator` / `NullEnumerator<T>` 是空对象模式，不产生任何元素。

```csharp
IEnumerator<int> enumerator = GetExistingEnumerator();             // 已有的枚举器
IEnumerable<int> enumerable = new EnumeratorEnumerable<int>(enumerator);
foreach (var item in enumerable) { }   // 直接迭代

// 空序列（单例）
IEnumerable<int> empty = new EnumeratorEnumerable<int>(NullEnumerator<int>.Instance);
```

## 集合扩展

### 数组扩展方法

`ArrayExtensions` 为数组提供洗牌、查找、转换等扩展方法。查找与转换支持"带状态参数"的重载，在需要捕获上下文时可以避免闭包分配。

```csharp
int[] numbers = { 1, 2, 3, 4, 5 };
numbers.ShuffleInPlace();                     // 就地洗牌

int firstEven = numbers.Find((n, _) => n % 2 == 0, null);       // 2
int index      = numbers.FindIndex((n, _) => n % 2 == 0, null); // 1

// 带状态参数的重载，需要捕获上下文时无需闭包
int firstBig  = numbers.Find((n, state) => n > state, 3);        // 第一个大于 3 的元素
int lastIndex = numbers.FindLastIndex((n, state) => n > state, 3);

string[] texts = numbers.ConvertAll((n, _) => n.ToString(), null);
```

### 列表扩展方法

`ListExtensions` 提供与数组扩展对应的列表版本，另可按条件批量删除。

```csharp
List<int> list = new() { 1, 2, 3, 4, 5 };
list.ShuffleInPlace();

int removed = list.RemoveAll((n, _) => n % 2 == 0, null);   // 删除全部偶数，返回删除数量
int found   = list.FindLast((n, _) => n > 2, null);         // 5
```

### 序列索引检索

`EnumerableExtensions` 提供了 `System.Linq.Enumerable` 没有提供的序列操作：对任意 `IEnumerable<T>` 按值或按谓词查找其
**索引**。

```csharp
IEnumerable<string> sequence = GetNames();   // 任意序列
int i = sequence.IndexOf("target");          // 第一个匹配的索引，找不到为 -1
int j = sequence.LastIndexOf("target");
int k = sequence.FindIndex(s => s.StartsWith("A"));
```

### 扩展方法与快速路径

以上方法均以扩展方法的形式对 `IEnumerable<T>` 提供。当输入实际是数组或 `List<T>` 时会自动走 `Array.IndexOf` /
`List.IndexOf` 等快速路径。

```csharp
string[] array = { "a", "b", "c" };
int idx = array.IndexOf("b");    // 数组同样可以直接调用，走快速路径
```

## 排序

### 快速排序

`QuickSort` 对 `IList<T>`（数组、`List<T>` 等）就地排序，支持指定范围、自定义比较器与键值数组同步排序。

```csharp
List<int> list = new() { 5, 3, 8, 1, 9 };
QuickSort.Sort(list);                                              // 默认比较器
QuickSort.Sort(list, Comparer<int>.Default);                       // 指定比较器
QuickSort.Sort(list, 1, 3);                                        // 仅排序部分范围

var items = new List<Item>();
QuickSort.Sort(items, Comparer<Item>.Create((a, b) => a.Price.CompareTo(b.Price)));   // 自定义比较器
QuickSort.Sort(keys, values);                                      // 键与值同步排序
```

### Tim 排序

`TimSort` 提供与 `QuickSort` 相同的能力（就地排序、范围、自定义比较器、键值同步排序），适合对接近有序的序列表现更稳定的场景。

```csharp
List<int> list = new() { 5, 3, 8, 1, 9 };
TimSort.Sort(list);                                // 默认比较器
TimSort.Sort(list, Comparer<int>.Default);         // 指定比较器
```

### 比较器工具

- `ComparerExtensions.Reversed<T>()`：反转比较结果
- `ReversedComparer<T>`：反转比较器，提供 `Default`
- `FunctorComparer<T>`：用 `Comparison<T>` 委托构造比较器
- `HashCodeComparer<T>`：按 `GetHashCode` 比较

```csharp
IComparer<int> descending = Comparer<int>.Default.Reversed();
IComparer<Item> byName = new FunctorComparer<Item>((a, b) => string.Compare(a.Name, b.Name));
QuickSort.Sort(items, byName);
```

## 对象池

对象池系列 API（`Pool<T>`、`IPool<T>`、`IPooledObjectPolicy<T>`）用于复用高频创建/销毁的对象，减少 GC 压力。`Pool<T>`
线程安全，归还时可自定义"是否可复用"的判定与复位逻辑。

### 预定义的池

`PredefinedPools` 提供常用类型的现成池，按需借用、用完归还即可：

- `ByteArrayLength4096`（`byte[4096]`）、`MemoryStream`、`Stopwatch`、`StringBuilder`
- `PredefinedPools<T>`：`ArrayLength2` / `ArrayLength4` / `ArrayLength8`、`HashSet<T>`、`List<T>`、`Queue<T>`、`Stack<T>`、
  `Deque<T>`
- `PredefinedPools<T1, T2>`：`Dictionary<TKey, TValue>`

```csharp
var builder = PredefinedPools.StringBuilder.Get();
try
{
    builder.Append("...");
    // 使用 builder
}
finally
{
    PredefinedPools.StringBuilder.Return(builder);   // 归还并复位
}
```

### UsingScope 自动借还

各 `UsingScope` 类把"获取 → 使用 → 归还"封装进 `IDisposable`，配合 `using` 语句自动完成借用与归还：

`ListUsingScope`、`DictionaryUsingScope`、`HashSetUsingScope`、`QueueUsingScope`、`StackUsingScope`、`DequeUsingScope`、
`ArrayLength2UsingScope`、`ArrayLength4UsingScope`、`ArrayLength8UsingScope`、`ByteArray4096UsingScope`、
`MemoryStreamUsingScope`、`StopwatchUsingScope`、`StringBuilderUsingScope`

```csharp
using (var scope = new ListUsingScope<int>(out var list))
{
    list.Add(1);
    list.Add(2);
    // 离开作用域后自动归还
}

using var stringBuilderScope = new StringBuilderUsingScope(out var sb);
sb.Append("...");
```

自定义对象池：实现 `IPooledObjectPolicy<T>` 的创建、取出、归还与销毁策略，交给 `Pool<T>` 管理。

```csharp
public sealed class BulletPolicy : IPooledObjectPolicy<Bullet>
{
    public Bullet Create() => new Bullet();

    public void Get(Bullet obj) { obj.Active = true; }    // 取出时激活

    public bool Return(Bullet obj)                        // 返回 true 表示可回收
    {
        obj.Active = false;                               // 归还时复位
        return true;
    }

    public void Dispose(Bullet obj) { }                   // 池销毁或拒绝回收时释放
}

IPool<Bullet> pool = new Pool<Bullet>(new BulletPolicy(), maximumRetained: 64);
Bullet bullet = pool.Get();
pool.Return(bullet);
```

## 可等待表达式

基于 C# awaitable 约定的自定义 awaiter，用于 `async` 方法中的上下文切换。

- `NullAwaitable`：立即完成，用于消除 CS1998（async 方法缺少 await）警告
- `SynchronizationContextAwaitable`：切换到指定的 `SynchronizationContext`（如主线程）
- `ThreadPoolThreadAwaitable`：切换到线程池线程

```csharp
public async Task HeavyWorkAsync()
{
    // 切到线程池，避免阻塞主线程
    await new ThreadPoolThreadAwaitable(CancellationToken.None);
    DoHeavyWork();

    // 切回主线程（主线程上保存的同步上下文）
    await new SynchronizationContextAwaitable(_mainContext);
    ApplyResult();   // 在这里可以安全执行需要主线程的操作
}

public async Task PlaceholderAsync()
{
    await new NullAwaitable();   // 消除 CS1998 警告
    // 尚未实现的逻辑
}
```

## 多线程与任务

### 任务工具类

`TaskUtility` 提供任务异常处理、结果转发与同步执行延续等辅助：

- `BeginAwait`：在新上下文中等待任务
- `ThrowIfFaultedOrCanceled`：任务出错抛出底层异常，取消则抛 `TaskCanceledException`
- `GetBaseException`：获取任务失败的根源异常
- `HandleFaultsAndCancellation`：把任务的完成/故障/取消转发给 `TaskCompletionSource`
- `ContinueWithSynchronously`：创建同步执行的延续任务

```csharp
Task job = RunAsync();

TaskUtility.ContinueWithSynchronously(job, t => OnJobCompleted());   // 同步执行延续

var tcs = new TaskCompletionSource<MyResult>();
TaskUtility.HandleFaultsAndCancellation(job, tcs);   // 自动传播结果/异常/取消
MyResult result = await tcs.Task;

try
{
    TaskUtility.ThrowIfFaultedOrCanceled(job);
}
catch (Exception e)
{
    // 处理失败或取消
}
```

### 异步屏障

`AsyncBarrier` 阻塞参与者，直到指定数量的参与者都发出来到信号后才一起继续，适合多任务阶段同步。

```csharp
var barrier = new AsyncBarrier(3);   // 3 个参与者

// 三个并发任务各自执行：
await barrier.SignalAndWait();       // 等待其余参与者全部到达
// 全部到达后各任务继续
```

### `CancellationTokenSourceUtility`

`CancellationTokenSourceUtility.IsDisposed` 检测 `CancellationTokenSource` 是否已被释放（官方 API 没有公开的检测手段）。

```csharp
if (CancellationTokenSourceUtility.IsDisposed(cts))
{
    // 已释放，进行相应处理
}
```

## 随机数工具

- `RandomUtility`：全局共享的线程安全随机数（`RandomUtility.Shared`）
- `P(probability)`：按概率返回布尔值
- `Choose` / `TryChoose`：按权重从集合中随机选取一个元素（可指定范围）
- `GetChosenIndex`：按权重返回被选中的索引
- `ThreadSafeRandom`：线程安全的 `Random` 子类；`NextDoubleIncludingOne` 返回 `[0, 1]` 闭区间

```csharp
if (RandomUtility.P(0.3)) { /* 30% 概率 */ }

var items   = new List<string> { "普通", "稀有", "史诗" };
var weights = new List<double> { 0.7, 0.2, 0.1 };
string chosen = RandomUtility.Choose(items, weights);    // 按权重选取
bool ok = RandomUtility.TryChoose(items, weights, out string picked);
int index = RandomUtility.GetChosenIndex(weights);

double d = RandomUtility.Shared.NextDoubleIncludingOne();   // [0, 1] 闭区间
```

## 枚举工具类

`EnumUtility<TEnum>` 提供枚举的常见反射与校验操作，结果已静态缓存，性能好。

```csharp
foreach (string name in EnumUtility<MyEnum>.Names) { }     // 全部名称
MyEnum[] values = EnumUtility<MyEnum>.Values;              // 全部值
int count = EnumUtility<MyEnum>.Count;
Type underlying = EnumUtility<MyEnum>.UnderlyingType;
bool isBitwise = EnumUtility<MyEnum>.IsBitwise;            // 是否标志位枚举

bool defined = EnumUtility<MyEnum>.IsDefined(MyEnum.Value1);

FieldInfo field = EnumUtility<MyEnum>.GetFieldInfo("Value1");
bool obsolete = EnumUtility<MyEnum>.IsObsolete(MyEnum.Value1);   // 是否标记了 Obsolete
```

## IO 工具类与 `Stream`、`TextReader` 扩展

- `IOUtility`：查询磁盘剩余空间、不足时抛出专用异常、创建指定长度的空文件
- `StreamExtensions`：`CopyToFrugally` / `CopyToFrugallyAsync`，复用缓冲池复制流，减少分配
- `TextReaderExtensions`：`SkipWhiteSpaces`，跳过连续空白字符
- 配套异常：`FileTooLargeException`、`NotEnoughAvailableFreeSpaceOnDriveException`

```csharp
long free = IOUtility.GetAvailableFreeSpaceOnDrive(@"D:\");
IOUtility.ThrowIfNotEnoughAvailableFreeSpaceOnDrive(@"D:\", 1024 * 1024);   // 不足则抛异常

IOUtility.CreateEmptyFile(@"D:\data.bin", 1024 * 1024);   // 创建 1MB 空文件

using (var src = File.OpenRead("in.bin"))
using (var dst = File.Create("out.bin"))
{
    src.CopyToFrugally(dst);   // 复用缓冲池复制
    // 异步版本：await src.CopyToFrugallyAsync(dst, cancellationToken);
}

using var reader = new StringReader("   hello");
reader.SkipWhiteSpaces();   // 跳过前导空白
```

## 时间测量与安全计数

- `ValueStopwatch`：值类型秒表，避免装箱与堆分配
- `CountIncrementSafeHandler`：带最大次数约束的计数，防止循环/递归次数溢出

```csharp
var sw = ValueStopwatch.StartNew();
RunExpensiveWork();
sw.Stop();
long ms = sw.ElapsedMilliseconds;

var counter = new CountIncrementSafeHandler(10000);
do
{
    counter.Increment();   // 超过上限时抛 UnexpectedException
    // ...
} while (condition);
```

## 插值与缓动

`Interpolation` 枚举提供 31 种缓动模式（Linear、In/Out/InOut 的
Sine、Quad、Cubic、Quart、Quint、Expo、Circ、Back、Elastic、Bounce）；`InterpolationUtility` 提供插值、反插值与模式转换。

```csharp
double t = 0.5;   // 权重 [0, 1]

double value = InterpolationUtility.Interpolate(0.0, 1.0, t, Interpolation.InOutCubic);
double eased = InterpolationUtility.Transform(t, Interpolation.OutBack);      // 转成缓动权重
double weight = InterpolationUtility.InverseLinearInterpolate(0.0, 10.0, 7.5); // 反插值求权重
```

## 内存与位操作

- `Memory`：`unsafe` 指针级内存操作（复制、移动、填充、清零、逐字节比较）
- `BitUtility`：`UnsignedRightShift` 无符号右移（`int` / `uint` / `long` / `ulong`），弥补 C# 9 缺少 `>>>` 运算符

```csharp
unsafe
{
    byte[] source = new byte[1024];
    byte[] target = new byte[1024];
    fixed (byte* pSource = source)
    fixed (byte* pTarget = target)
    {
        Memory.Copy(pTarget, pSource, (ulong)source.Length);   // 复制
        Memory.Set(pTarget, 0xFF, (ulong)target.Length);       // 填充
        int cmp = Memory.Compare(pTarget, pSource, (ulong)source.Length);   // 比较
        Memory.Clear(pTarget, (ulong)target.Length);           // 清零
    }
}

uint u = BitUtility.UnsignedRightShift(0x80000000u, 1);   // 0x40000000
```

## 其他小型实用类

### 十六进制字符解析

`HexCharParseUtility` 提供十六进制字符的解析（`0-9`、`A-F`、`a-f`）。

```csharp
byte b = HexCharParseUtility.Parse('F');                  // 15；非法字符抛 ArgumentOutOfRangeException
byte b2 = HexCharParseUtility.ParseNoCheck('F');          // 无检查的快速版本
bool ok = HexCharParseUtility.TryParse('G', out byte v);  // false
```

### 临时 ID 生成

`TempIdGenerator` 生成带前缀/后缀的临时 ID，中间为 `Guid` 的 `N` 格式字符串，并提供格式匹配检查。

```csharp
var generator = new TempIdGenerator("obj_", "");
string id = generator.NewTempId;     // 例如 "obj_3f2a9c1e..."
bool matched = generator.Match(id);  // 检查 ID 是否符合本生成器格式
```

### 类型名友好格式化

`TypeUtility.GetNicelyFormattedName` 输出可读的类型名：内置类型映射为 C# 关键字，并处理数组、可空、嵌套与泛型等形态。

```csharp
string s = TypeUtility.GetNicelyFormattedName(typeof(List<int>));   // System.Collections.Generic.List<int>
string t = TypeUtility.GetNicelyFormattedName(typeof((int, string))); // (int,string)
```

### Invocation 系列

把一段逻辑包装为可延迟执行、可传递的调用对象；`OneTimeInvocation` 保证整段逻辑只真正执行一次（线程安全）。

```csharp
Invocation a = new InvocationAction(() => Console.WriteLine("执行"));
a.Invoke();                                  // 立即执行

Invocation<int> b = Invocation<int>.FromResult(42);   // 构造时固定返回值
int result = b.Invoke();                     // 42

Invocation once = new OneTimeInvocation(a);
once.Invoke();   // 第一次真正执行
once.Invoke();   // 之后不再执行
```

### 散列码组合

`HashHelper.CombineHashCodes` 把多个值组合为散列码，提供 2~16 个参数的重载与 `params` 数组版本。

```csharp
int hash = HashHelper.CombineHashCodes(a, b, c);
int hash2 = HashHelper.CombineHashCodes(values);   // params int[]
```

### 显式空结果与空对象

`VoidResult` 显式表示"没有结果"，适合 `TaskCompletionSource<VoidResult>` 等只关心完成/取消/异常的信号场景；
`NullDisposable` 是 `IDisposable` 的空对象实现（单例）。

```csharp
var done = new TaskCompletionSource<VoidResult>();
done.TrySetResult(default);   // 完成信号

using IDisposable noop = NullDisposable.Instance;   // 需要 IDisposable 但又无事可做
```

### 字符串扩展

`StringExtensions.ReplaceBackslashWithSlash` 把反斜杠替换为正斜杠，常用于路径规范化。

```csharp
string path = @"C:\data\file.txt".ReplaceBackslashWithSlash();   // "C:/data/file.txt"
```

### 其他

- `UnexpectedException`：表示发生了预期之外的情况
- `CommentAttribute`：给不支持 XML 注释的成员写说明的特性
- `Environment`：`IsSingleThreadEnvironment` 运行时环境标志
- `Constant`：常用常量（`G9` / `G17` 浮点往返格式、RFC 5322 邮箱正则、大量 Unicode 字符常量等）

```csharp
if (Aurora.Environment.IsSingleThreadEnvironment)
{
    // 单线程环境下的优化路径
}
```

## 许可证

[MIT License](LICENSE)