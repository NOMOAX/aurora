# Aurora

![许可](https://img.shields.io/github/license/NOMOAX/aurora)
![版本](https://img.shields.io/badge/version-1.5.3-blue)
![最低 Unity 版本](https://img.shields.io/badge/Unity-2021.2%2B-blue)

适用于 Unity 的高性能、低内存消耗的 C# 工具包。

[English](README.md) | 中文

## 安装

1. 打开 Unity package manager。
2. 点击左上角的 `+` 按钮，然后选择 `Add package from git URL...`。
3. 填入 `https://github.com/NOMOAX/aurora.git` 并点击 `Add` 按钮。

## 托管代码剥离

`EventBus<T>` 通过反射访问 `ConcurrentDictionary<TKey, TValue>` 的私有方法 `TryRemoveInternal`。Unity 链接器在静态分析时无法看到这一引用，因此本包附带了一个 [link.xml](link.xml)，将该方法声明为根。

**Unity 只会读取 `Assets/` 目录下的 `link.xml`，因此本包内的这份不会被读取。** 需要手动加入项目：

- 如果你的项目中没有 `link.xml`，请将本包中的 `link.xml` 复制到 `<Unity 项目路径>/Assets` 文件夹下。
- 如果你的项目中已有 `link.xml`，请将这份的内容添加到已有的 `link.xml` 里。

```xml

<linker>
    <assembly fullname="mscorlib">
        <type fullname="System.Collections.Concurrent.ConcurrentDictionary`2">
            <method name="TryRemoveInternal" />
        </type>
    </assembly>
</linker>
```

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

EventBus<int>.Subscribe(Id.SendMessage, (Action<string>)OnMessageReceived); // 注册事件
EventBus<int>.Unsubscribe(Id.SendMessage, (Action<string>)OnMessageReceived); // 取消注册事件
EventBus<int>.Publish(Id.SendMessage, "Kevin", "Hello world!"); // 发布事件

// 如果事件的委托类型具有返回值，则可以在发布事件时一次性收集所有返回值
var phoneNumbers = (string[])EventBus<int>.PublishAll(Id.GetPhoneNumber);

// 等待事件发布
await EventBus<int>.WhenPublished(Id.SendMessage);

// 也可以传入 CancellationToken，在取消时结束等待
await EventBus<int>.WhenPublished(Id.SendMessage, cancellationToken);
```

`WhenPublished` 返回一个 `Task`，它在指定事件被 `Publish` / `PublishAll` 发布时完成。如果该事件在调用之前已经发布过，那么这次调用不会因为那次发布而完成。`Clear` 会让所有正在等待的 `Task` 完成。

## 日志

`Log` 提供 `V` / `D` / `I` / `W` / `E` 五个级别的日志，默认输出到控制台。

`D` 级别标记了 `Conditional("DEBUG")`。

可以通过 `ILogger` 自定义输出方式，并配置日志级别、线程标识与时间戳。

```csharp
Log.I($"加载资源: {path}");
Log.D("调试信息，仅 DEBUG 构建时输出");
Log.W("可用磁盘空间不足");
Log.E("参数为 null");

Log.Level = LogLevel.I; // 只记录 I / W / E 级别
Log.WithCurrentThreadId = true; // 附带当前线程标识
Log.DateTimeOffsetFormat = LogDateTimeOffsetFormat.S; // 附带 ISO 8601 时间戳
Log.Logger = new MyLogger(); // 切换自定义实现

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

`Deque<T>` 使用环状缓冲区实现，从头部与尾部增删均为 `O(1)`。实现 `IList<T>` / `IReadOnlyList<T>`，支持按索引读写，容量不足时自动增长。

```csharp
var deque = new Deque<int>();
deque.EnqueueLast(10); // [10]
deque.EnqueueFirst(20); // [20, 10]
deque.EnqueueLast(30); // [20, 10, 30]

var head = deque.DequeueFirst(); // 20
var tail = deque.DequeueLast(); // 30

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

heap.Peek(); // 0
heap.Take(); // 0
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

root.IsRoot; // true
b.IsLeaf; // true
a.Level; // 1
root.Root; // 根节点自身
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

`TreeEnumOrder` 提供 7 种次序：`Default`（直接子节点）、`BreadthFirstLr`、`BreadthFirstRl`、`DepthFirstDlr`、`DepthFirstDrl`、`DepthFirstLrd`、`DepthFirstRld`。

底层枚举器（`LrEnumerator`、`RlEnumerator`、`DlrEnumerator`、`DrlEnumerator`、`LrdEnumerator`、`RldEnumerator` 及其基类）也公开可用，可传入任意的"取子节点"函数来遍历任何树形数据。

### 有限状态机

`StateMachine<T>` 是通用有限状态机，`T` 为状态的标识符类型。支持进入/退出回调、立即切换与延迟切换、切换期间的状态保护。

```csharp
// 建议使用 System.Type 作为状态的标识符类型

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
        // 离开该状态时无需额外处理
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
// 持续更新状态机，直到中止
while (stateMachine.Update())
{
    Console.WriteLine(stateMachine.CurrentState); // IdleState 或 WorkingState
}
Console.WriteLine(stateMachine.CurrentState); // null
```

| 方法                       | 允许被谁调用       | 何时生效      |
|----------------------------|--------------------|---------------|
| `TransitionTo`             | 状态机的直接持有者 | 立即          |
| `TransitionToNull`         | 状态机的直接持有者 | 立即          |
| `ScheduleTransitionTo`     | 任意               | 下一次 Update |
| `ScheduleTransitionToNull` | 任意               | 下一次 Update |

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

var removed = blackboard.GetAndRemoveValue<int>("hp"); // 取值的同时移除

blackboard.Remove("hp");
blackboard.Clear();

// 与状态机配合
stateMachine.Blackboard.SetValue("coin", 100);
```

### 修复器

`Fixer` 继承 `Node`，把"需要修复的树"建模为一棵节点树：每个节点提供"是否已就绪"的判定与"异步修复"的操作，并带优先级（小的先处理）。按优先级递归修复整棵树，直到所有节点就绪。

一个简单的例子：

- 假设你正在开发笔记应用
- 有一个笔记窗口（以下简称为 `A`），它有关闭按钮
- 在 `A` 下，有一个图片列表子窗口（以下简称为 `B`）和一个笔记正文子窗口（以下简称为 `C`）
- 假设用户正在 `B` 中上传图片，但仍在上传中，上传的异步任务为 `B.uploadTask`
- 假设用户修改了笔记正文，点击了 `C` 的保存按钮，上传新的笔记正文到服务器，但仍在请求中，请求的异步任务为 `C.saveTask`
- 这时候用户点击了 `A` 关闭按钮
- 检查 `A.Fixer` 的值，它本身是 `true`，但它的子级 `B.Fixer.IsFixed == false` `C.Fixer.IsFixed == false`，所以 `A.Fixer.IsFixed == false`
- 弹出一个对话框，让用户选择是要等待所有异步任务完成，还是立即关闭 `A`
- 假设用户选择了 `等待所有异步任务完成`
- 执行 `await A.Fixer.FixAsync()`，并设置一个合理的超时时间
- 当方法返回值，再次检查 `A.Fixer.IsFixed` 的值，如果为 `true`，就可以关闭 `A` 了
- 如果 `A.Fixer.IsFixed == false`，则可以再弹窗提醒用户 `上传失败/保存失败`，并阻止这次关闭 `A` 的操作，用户可以自行重试 `上传图片/保存笔记正文`

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
            {
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
                catch (OperationCanceledException)
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
            }
            // 关闭
            case 1:
            {
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
            }
            // 取消
            case 2:
            {
                // 用户点击了取消，什么都不做
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

### 枚举器工具

`EnumeratorEnumerable` / `EnumeratorEnumerable<T>` 把已有的 `IEnumerator` 包装为 `IEnumerable`，便于 `foreach` 与 LINQ；`NullEnumerator` / `NullEnumerator<T>` 是空对象模式，不产生任何元素。

```csharp
IEnumerator<int> enumerator = GetExistingEnumerator(); // 已有的枚举器
IEnumerable<int> enumerable = new EnumeratorEnumerable<int>(enumerator);
foreach (var item in enumerable)
{
    // 直接迭代
}

// 空序列（单例）
IEnumerable<int> empty = new EnumeratorEnumerable<int>(NullEnumerator<int>.Instance);
```

## 集合扩展

### 数组扩展方法

`ArrayExtensions` 为数组提供洗牌、查找、转换等扩展方法。查找与转换支持"带状态参数"的重载，在需要捕获上下文时可以避免闭包分配。

```csharp
int[] numbers = { 1, 2, 3, 4, 5 };

// 判据通过 state 传入，避免闭包分配
int firstElementGreaterThanThree = numbers.Find((element, state) => element > state, 3); // 4
int firstElementIndexGreaterThanThree = numbers.FindIndex((element, state) => element > state, 3); // 3
int lastElementLessThanThree = numbers.FindLast((element, state) => element < state, 3); // 2
int lastElementIndexLessThanThree = numbers.FindLastIndex((element, state) => element < state, 3); // 1

numbers.ShuffleInPlace(); // 就地洗牌
```

### 列表扩展方法

`ListExtensions` 提供与数组扩展对应的列表版本，另可按条件批量删除。

```csharp
List<int> list = new() { 1, 2, 3, 4, 5 };

int firstElementGreaterThanThree = list.Find((element, state) => element > state, 3); // 4
int removedElementCount = list.RemoveAll((element, state) => element > state, 3); // 删除全部大于 3 的元素，返回删除数量

list.ShuffleInPlace(); // 就地洗牌
```

### 序列索引检索

`EnumerableExtensions` 提供了 `System.Linq.Enumerable` 没有提供的序列操作：对任意 `IEnumerable<T>` 按值或按谓词查找其 **索引**。

```csharp
IEnumerable<string> sequence = GetNames(); // 任意序列
int i = sequence.IndexOf("target"); // 第一个匹配的索引，找不到为 -1
int j = sequence.LastIndexOf("target");
int k = sequence.FindIndex(s => s.StartsWith("A"));
```

## 排序

### 快速排序

`QuickSort` 使用标准快速排序算法实现，但在内部避免了递归，因此不会因为待排序序列过长而栈溢出。

```csharp
List<int> list = new() { 5, 3, 8, 1, 9 };
QuickSort.Sort(list); // 默认比较器
QuickSort.Sort(list, Comparer<int>.Default); // 指定比较器
QuickSort.Sort(list, 1, 3); // 仅排序部分范围

var items = new List<Item>();
QuickSort.Sort(items, Comparer<Item>.Create((a, b) => a.Price.CompareTo(b.Price))); // 自定义比较器
QuickSort.Sort(keys, values); // 如果 keys 中的元素被交换了位置，也相应地交换 values 中对应位置的元素，从而确保 key 和 value 始终匹配
```

### Tim 排序

`TimSort` 是从 Java 源码中的原逻辑复刻而来。它提供与 `QuickSort` 相同的能力（就地排序、范围、自定义比较器、键值同步排序），速度稍慢于 `QuickSort`，但它是稳定的——相等元素在排序前后的相对顺序不会改变。

```csharp
List<int> list = new() { 5, 3, 8, 1, 9 };
TimSort.Sort(list); // 默认比较器
TimSort.Sort(list, Comparer<int>.Default); // 指定比较器
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

对象池系列 API（`Pool<T>`、`IPool<T>`、`IPooledObjectPolicy<T>`）用于复用高频创建/销毁的对象，减少 GC 压力。`Pool<T>` 线程安全，归还时可自定义"是否可复用"的判定与复位逻辑。

### 预定义的池

`PredefinedPools` 提供常用类型的现成池，按需借用、用完归还即可：

- `ByteArrayLength4096`（`byte[4096]`）、`MemoryStream`、`Stopwatch`、`StringBuilder`
- `PredefinedPools<T>`：`ArrayLength2`、`ArrayLength4`、`ArrayLength8`、`HashSet<T>`、`List<T>`、`Queue<T>`、`Stack<T>`、`Deque<T>`
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
    PredefinedPools.StringBuilder.Return(builder); // 归还并复位
}
```

### UsingScope 自动借还

各 `UsingScope` 类把"获取 → 使用 → 归还"封装进 `IDisposable`，配合 `using` 语句自动完成借用与归还：

`ListUsingScope`、`DictionaryUsingScope`、`HashSetUsingScope`、`QueueUsingScope`、`StackUsingScope`、`DequeUsingScope`、`ArrayLength2UsingScope`、`ArrayLength4UsingScope`、`ArrayLength8UsingScope`、`ByteArray4096UsingScope`、`MemoryStreamUsingScope`、`StopwatchUsingScope`、`StringBuilderUsingScope`

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
public sealed class PooledBulletPolicy : IPooledObjectPolicy<Bullet>
{
    public Bullet Create()
    {
        return new Bullet();
    }

    public void Get(Bullet obj)
    {
        // 取出时激活
        obj.Active = true;
    }

    public bool Return(Bullet obj)
    {
        if (obj == null)
        {
            return false; // 返回 false 表示不可回收
        }
        obj.Active = false; // 归还时复位
        return true; // 返回 true 表示可回收
    }

    public void Dispose(Bullet obj)
    {
        // 池销毁或拒绝回收时释放资源
        obj?.Dispose(); // 释放 Bullet 自身占用的资源
    }
}

IPool<Bullet> pool = new Pool<Bullet>(new PooledBulletPolicy(), maximumRetained: 64);
Bullet bullet = pool.Get();
pool.Return(bullet);
```

## 可等待表达式

基于 C# awaitable 约定的自定义 awaiter，用于 `async` 方法中的上下文切换。

- `NullAwaitable`：立即完成，用于消除 CS1998（async 方法缺少 await）警告；请只用于临时用途。
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
    ApplyResult(); // 在这里可以安全执行需要主线程的操作
}

public async Task PlaceholderAsync()
{
    await new NullAwaitable(); // 消除 CS1998 警告
    // TODO 尚未实现的逻辑
}
```

## 多线程与任务

### 任务工具类

`TaskUtility` 提供任务错误处理、状态转发与同步执行延续等辅助：

- `BeginAwait`：在新上下文中等待任务，不阻塞当前程序执行流
- `ThrowIfFaultedOrCanceled`：任务处于错误状态时抛出底层异常，处于取消状态时抛 `TaskCanceledException`
- `GetBaseException`：获取任务错误的根源异常
- `HandleFaultsAndCancellation`：当任务处于错误或取消状态时，相应地让 `TaskCompletionSource` 也进入错误或取消状态
- `ContinueWithSynchronously`：创建同步执行的延续任务

### 异步屏障

`AsyncBarrier` 阻塞参与者，直到指定数量的参与者都发出来到信号后才一起继续，适合多任务阶段同步。

```csharp
var barrier = new AsyncBarrier(3); // 3 个参与者

// 三个并发任务各自执行：
await barrier.SignalAndWait(); // 等待其余参与者全部到达
// 全部到达后各任务继续
```

### CancellationTokenSourceUtility

`CancellationTokenSourceUtility.IsDisposed` 检测 `CancellationTokenSource` 是否已被释放（官方 API 没有公开的检测手段）。

## 随机数工具

- `RandomUtility`：全局随机数工具（内部使用 `AuroraRandom.Instance`）
- `P(probability)`：按概率返回布尔值
- `Choose` / `TryChoose`：按权重从集合中随机选取一个元素，也可只在集合的一段范围内选取
- `GetChosenIndex`：按权重返回被选中的索引，参数形式同上
- `AuroraRandom`：`Random` 的派生类；`Instance` 是当前线程的实例（每个线程一个，避免多线程竞争）
    - `NextDoubleIncludingOne()`：返回 `[0, 1]` 闭区间的 `double`

```csharp
if (RandomUtility.P(0.3))
{
    // 30% 概率
}

var items = new List<string> { "普通", "稀有", "史诗" };
var weights = new List<double> { 0.7, 0.2, 0.1 };
var chosen = RandomUtility.Choose(items, weights); // 按权重选取
var ok = RandomUtility.TryChoose(items, weights, out var picked);
var index = RandomUtility.GetChosenIndex(weights);

// 只在集合的指定范围内选取
var chosenInRange = RandomUtility.Choose(items, weights, 1, 2);

var d = AuroraRandom.Instance.NextDoubleIncludingOne(); // [0, 1] 闭区间
```

## 枚举工具类

`EnumUtility<TEnum>` 提供枚举的常见反射与校验操作，结果已静态缓存，性能好。

```csharp
foreach (string name in EnumUtility<MyEnum>.Names)
{
    // 全部名称
}
MyEnum[] values = EnumUtility<MyEnum>.Values; // 全部值
int count = EnumUtility<MyEnum>.Count;
Type underlying = EnumUtility<MyEnum>.UnderlyingType;
bool isBitwise = EnumUtility<MyEnum>.IsBitwise; // 是否标志位枚举

bool isDefined = EnumUtility<MyEnum>.IsDefined(MyEnum.Value1);

FieldInfo fieldInfo = EnumUtility<MyEnum>.GetFieldInfo("Value1");
bool isObsolete = EnumUtility<MyEnum>.IsObsolete(MyEnum.Value1); // 是否标记了 Obsolete
```

## IO 工具类与 `Stream`、`TextReader` 扩展

- `IOUtility`：查询磁盘剩余空间、不足时抛出专用异常、创建指定长度的空文件
- `PathUtility`：路径字符串操作（替换路径分隔符、计算相对路径并给出两条路径的关系）
- `StreamExtensions`：`CopyToFrugally` / `CopyToFrugallyAsync`，复用缓冲池复制流，减少分配
- `TextReaderExtensions`：`SkipWhiteSpaces`，跳过连续空白字符
- 配套异常：`FileTooLargeException`、`NotEnoughAvailableFreeSpaceOnDriveException`

```csharp
long free = IOUtility.GetAvailableFreeSpaceOnDrive(@"D:\");
IOUtility.ThrowIfNotEnoughAvailableFreeSpaceOnDrive(@"D:\", 1024 * 1024); // 不足则抛异常

IOUtility.CreateEmptyFile(@"D:\data.bin", 1024 * 1024); // 创建 1MB 空文件

using (var src = File.OpenRead("in.bin"))
using (var dst = File.Create("out.bin"))
{
    src.CopyToFrugally(dst); // 复用缓冲池复制
    // 异步版本：await src.CopyToFrugallyAsync(dst, cancellationToken);
}

using var reader = new StringReader("   hello");
reader.SkipWhiteSpaces(); // 跳过前导空白
```

`PathUtility.GetRelativePath` 在返回相对路径的同时，通过 `PathRelationship` 告诉你两条路径的关系：`IsChildOf`（子级）、`IsEqualTo`（相等）、`IsNeitherChildOfNorEqualTo`（既非子级也不相等）、`AreUnrelated`（没有共同的根）。

```csharp
var relative = PathUtility.GetRelativePath(@"C:\A", @"C:\A\B\C", out var relationship);
// relative 是相对路径（如 "B\C"），relationship 是 PathRelationship.IsChildOf

var equal = PathUtility.GetRelativePath(@"C:\A", @"C:\A", out var relationship1);
// equal 是 "."，relationship1 是 PathRelationship.IsEqualTo

var unrelated = PathUtility.GetRelativePath(@"C:\A", @"D:\B", out var relationship2);
// unrelated 是 null，relationship2 是 PathRelationship.AreUnrelated

var normalized = PathUtility.ReplaceBackslashWithForwardSlash(@"C:\data\file.txt"); // "C:/data/file.txt"
```

## 时间测量与安全计数

- `ValueStopwatch`：值类型秒表，避免装箱与堆分配
- `CountIncrementSafeHandler`：当无法预料某个循环或递归要执行多少次时，给定一个合理的最大执行次数，超过上限即判定为异常

## 插值与缓动

`Interpolation` 枚举提供 31 种缓动模式（Linear、In/Out/InOut 的 Sine、Quad、Cubic、Quart、Quint、Expo、Circ、Back、Elastic、Bounce）；`InterpolationUtility` 提供插值、反插值与模式转换。

```csharp
double t = 0.5; // 权重 [0, 1]

double value = InterpolationUtility.Interpolate(0.0, 1.0, t, Interpolation.InOutCubic);
double eased = InterpolationUtility.Transform(t, Interpolation.OutBack); // 转成缓动权重
double weight = InterpolationUtility.InverseLinearInterpolate(0.0, 10.0, 7.5); // 反插值求权重
```

## 位操作

`BitUtility.UnsignedRightShift` 提供无符号右移（`int` / `uint` / `long` / `ulong`），即 `>>>` 运算符在 C# 11 之前的等价实现。

```csharp
var value = BitUtility.UnsignedRightShift(-1, 1); // 2147483647 (0x7FFFFFFF)；而 -1 >> 1 的结果是 -1
```

## 其他小型实用类

### 十六进制字符解析

`HexCharParseUtility` 提供十六进制字符的解析（`0-9`、`A-F`、`a-f`）。

```csharp
byte b = HexCharParseUtility.Parse('F'); // 15；非法字符抛 ArgumentOutOfRangeException
byte b2 = HexCharParseUtility.ParseNoCheck('F'); // 无检查的快速版本
bool ok = HexCharParseUtility.TryParse('G', out byte v); // false
```

### 临时 ID 生成

`TempIdGenerator` 生成带前缀/后缀的临时 ID，中间为 `Guid` 的 `N` 格式字符串，并提供格式匹配检查。

```csharp
var generator = new TempIdGenerator("obj_", "");
string id = generator.NewTempId; // 例如 "obj_3f2a9c1e..."
bool matched = generator.Match(id); // 检查 ID 是否符合本生成器格式
```

### 类型名友好格式化

`TypeUtility.GetNicelyFormattedName` 输出可读的类型名：内置类型映射为 C# 关键字，并处理数组、可空、嵌套与泛型等形态。

```csharp
string s = TypeUtility.GetNicelyFormattedName(typeof(List<int>)); // System.Collections.Generic.List<int>
string t = TypeUtility.GetNicelyFormattedName(typeof((int, string))); // (int,string)
```

### 英文工具

`EnglishUtility` 提供英文文本的小工具。

```csharp
var pluralized = EnglishUtility.Pluralize("apple", "apples", 3); // "apples"
var singularized = EnglishUtility.Pluralize("apple", "apples", 1); // "apple"
var ordinal = EnglishUtility.Th(21); // "st"，即 $"{21}{ordinal}" == "21st"
```

### Invocation 系列

把一段逻辑包装为可延迟执行、可传递的调用对象；`OneTimeInvocation` 保证整段逻辑只真正执行一次（线程安全）。

```csharp
Invocation a = new InvocationAction(() =>
{
    Console.WriteLine("执行");
});
a.Invoke(); // 立即执行

Invocation<int> b = Invocation<int>.FromResult(42); // 构造时固定返回值
int result = b.Invoke(); // 42

Invocation once = new OneTimeInvocation(a);
once.Invoke(); // 第一次真正执行
once.Invoke(); // 之后不再执行
```

### 哈希值组合

`HashHelper.CombineHashCodes` 把多个哈希值合并为一个哈希值，提供 2~16 个参数的重载与 `params` 数组版本。

```csharp
int hash = HashHelper.CombineHashCodes(a, b, c);
int hash2 = HashHelper.CombineHashCodes(values); // params int[]
```

### 空结果与空可释放对象

`VoidResult` 显式表示"没有结果"，适合 `TaskCompletionSource<VoidResult>` 等只关心完成/取消/异常的信号场景；`NullDisposable` 是 `IDisposable` 的空对象实现（单例）。

```csharp
var done = new TaskCompletionSource<VoidResult>();
done.TrySetResult(new VoidResult()); // 完成信号

// 某些编译条件下需要一个真实的 IDisposable，某些条件下只需形式占位
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

### 其他

- `UnexpectedException`：表示发生了预期之外的情况
- `CommentAttribute`：给不支持 XML 注释的成员写说明的特性，其说明可以在 IDE 中快速查看
- `Environment`：`IsSingleThreadEnvironment` 运行时环境标志
- `Constant`：常用常量
    - `Constant.String`：字符串常量，如 `G9` / `G17` 浮点往返格式
    - `Constant.Regex.EmailAddressRegex`：符合 RFC 5322 标准的邮箱地址正则表达式，已用 `RegexOptions.Compiled` 预编译，可直接复用，无需每次创建
    - `Constant.Character`：大量 Unicode 字符常量

```csharp
if (Aurora.Environment.IsSingleThreadEnvironment)
{
    // 单线程环境下的优化路径
}

// 邮箱地址校验
bool isEmailAddress = Constant.Regex.EmailAddressRegex.IsMatch(input);
```
