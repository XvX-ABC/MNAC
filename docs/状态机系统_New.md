# MNAC 新状态机库 `MNAC.StatesNew`

> 项目：MNAC（迷你装甲核心）
> 位置：`Assets/Scripts/States_New/`，程序集 `MNAC.StatesNew`，命名空间 `MNAC.StatesNew`
> 状态：**新旧并行**——旧库 `Assets/Scripts/States/`（`MNAC.States`）保持不动，本库为干净重写版。
> 背景：基于旧库 `docs/状态机系统.md` 的评审与代码逐行核对，重写并修复其已知问题。

---

## 1. 一句话介绍

`MNAC.StatesNew` 是旧 `MNAC.States` 的**重写版**：泛型分层状态机 + Timeline 化过渡，**纯 C# 类**（无 Mono/SO 载体），分层保留 Timeline（Core 纯逻辑、Playable 扩展）。转移表改由**状态机持有**，修复了旧库双份缓存、异常空转、初始态不确定、悬空转移等一批问题。

## 2. 与旧库的差异总览

| 维度 | 旧库 `MNAC.States` | 新库 `MNAC.StatesNew` |
|---|---|---|
| 转移表归属 | 存在**状态**上（每状态出边数组） | 存在**状态机**上（`List<Transition<T>>`） |
| 驱动方式 | `OnUpdate()` 内部用 `Time.deltaTime` | `Update(float deltaTime)` 调用方显式传 dt |
| 初始状态 | `HashSet.First()` 不确定 | `SetInitialState()` 显式设置 |
| 异常体系 | 定义了但原样 rethrow（空转） | 真正包装 `StateEnter/Exit/UpdateException` |
| 载体 | 类 + Mono + SO 三件套 | **只要纯 C# 类** |
| 中断机制 | 浅拷贝目标出边数组 | 机器转移表按 `source==目标` 查询 |
| 命名 | `Statemachine` 拼写混乱 | 统一 `StateMachine` |
| Playable 状态生命周期 | 手动推进 Timeline | 默认 `OnEnter=Restart / OnUpdate=推进 / OnExit=End` |

## 3. 目录结构

```
Assets/Scripts/States_New/
├── MNAC.StatesNew.asmdef        引用 MNAC.Utilities（GUID e77376900b62a3d41a6e000c035ec369）
├── Core/                        纯逻辑、零 Timeline 依赖
│   ├── IState.cs                IState<T>: Name/Id/Enabled/Context/OnEnter/OnUpdate(dt)/OnExit
│   ├── StateBase.cs             StateBase<T>: 抽象生命周期，无转移存储
│   ├── EmptyState.cs            no-op 占位状态
│   ├── Transition.cs            Transition<T>: Source/Destination/Triggers(AND短路)/IsTriggered/OnTriggered
│   ├── StateMachine.cs          StateMachine<T>: AddState/RemoveState(清理边)/SetInitialState/AddTransition/ChangeStateTo/Update(dt)
│   └── StateMachineException.cs StateNotRegistered/StateEnter/StateExit/StateUpdateException
├── Playable/                    依赖 Timeline
│   ├── InterruptionMode.cs      enum { None, Next }
│   ├── IPlayableState.cs        + Timeline + 六过渡回调
│   ├── IPlayableTransition.cs   + Timeline + InterruptionMode + Source/Destination
│   ├── PlayableStateBase.cs     默认生命周期自动驱动 Timeline
│   ├── PlayableTransition.cs    自建 Timeline，挂六回调 + durationEvent
│   ├── BlendingTransitionOptions.cs  可序列化过渡参数
│   ├── BlendingTransition.cs    offset / fixedExitTime，锁定同一 Timeline 实例
│   └── PlayableStateMachine.cs  TransitionState 重构 + 中断查询 + 机器当状态
├── WithCallback/
│   ├── IWithCallbackPlayableState.cs
│   ├── WithCallbackPlayableState.cs
│   └── WithCallbackPlayableStateMachine.cs
├── SubStateMachine/
│   ├── SubStateMachineState.cs
│   ├── WithCallbackStateMachineState.cs
│   ├── PlayableStateMachineState.cs
│   └── SubStateMachineTransition.cs
└── Animation/
    ├── AnimationStateBase.cs    持 Animator + OnAnimationIK 钩子
    └── AnimationStateMachine.cs 转发 OnAnimatorIK
```

## 4. 快速上手（示例）

```csharp
using MNAC.StatesNew;

// 状态：继承 WithCallbackPlayableState（或 PlayableStateBase / StateBase）
class IdleState : WithCallbackPlayableState
{
    public IdleState() : base("idle", 1f) { }
}

// 机器：WithCallbackPlayableStateMachine<object>（顶层最常用）
var sm = new WithCallbackPlayableStateMachine<object>("movement");
var idle = new IdleState();
var run  = new RunState();
sm.AddState(idle);
sm.AddState(run);
sm.SetInitialState(idle);

// 转移：带时长 = 可播放过渡；零时长 = 直切
sm.AddTransition(idle, run, duration: 0.5f, trigger: () => input.Speed > 0);
sm.AddTransition(run, idle, duration: 0.3f, trigger: () => input.Speed <= 0);

// 驱动：显式传 dt（MonoBehaviour Update 里 Time.deltaTime，FixedUpdate 里 Time.fixedDeltaTime）
sm.Update(Time.deltaTime);
```

## 5. 迁移指南（旧 → 新）

> 目标：把游戏代码从 `using MNAC.States` 迁到 `using MNAC.StatesNew`，行为对齐。

| 旧写法 | 新写法 |
|---|---|
| `machine.OnUpdate()` | `machine.Update(Time.deltaTime)`（FixedUpdate 传 `Time.fixedDeltaTime`） |
| 状态内手动 `timeline.OnUpdate(Time.deltaTime)` | **删除**，基类 `PlayableStateBase.OnUpdate` 已自动推进（防双重推进） |
| 依赖 `states.First()` 当初始态 | 显式 `SetInitialState(...)` |
| `InterruptionSource` | `InterruptionMode` |
| `AddTransitionFor(state, dest, trigger)` | `AddTransition(state, dest, trigger)` 或完整重载 `AddTransition(state, dest, duration, trigger, durationEvent, interruptionMode)` |
| `WithCallbackPlayableStatemachine<object>` | `WithCallbackPlayableStateMachine<object>` |
| `SubStatemachineState` | `SubStateMachineState` |
| `PlayableStatemachineState` | `PlayableStateMachineState` |
| `WithCallbackStatemachineState` | `WithCallbackStateMachineState` |
| `SubStatemachineTransition` | `SubStateMachineTransition` |
| 机器当状态，进入靠 `OnEnter` 转发 | 新语义：`OnEnter` 确定性进初始态，`OnExit` 退出当前子状态并清空（下次进入重新从初始态开始） |

## 6. 修复的问题清单（相对旧库）

1. **转移双份缓存不同步** → 转移表归机器，状态零转移存储。
2. **`BlendingTransition.Begin` 静默失效** → `UpdateLength(runningCheck:false)`，且锁定同一 Timeline 实例恢复。
3. **异常体系空转** → `ChangeState`/`Update` 真正包装，异常携带状态名。
4. **初始态不确定** → `SetInitialState()`。
5. **非 Playable 转移被吞** → `CreateTransition` 显式校验类型并抛异常；Playable 机器上非可播放转移走直切。
6. **自转移语义不一致** → 统一 no-op（`ChangeStateTo` 与转移路径都守卫）。
7. **`RemoveState` 留悬空转移** → 移除时清理所有 source/dest 关联边。
8. **中断浅拷贝数组** → `TransitionState` 在机器转移表上按 `source==目标` 查询。
9. **`TransitionState.OnExit` 二次触发 End 事件** → OnExit 改 no-op。
10. **子状态机包装生命周期不一致** → 三个包装统一进入序列。
11. **`Context` 广播 O(n) 每次赋值** → 保留广播但用 List；文档注明 setup 期设置。
12. **触发无短路** → `IsTriggered` 遇 false 提前返回。
13. **无用 using / 空 namespace / 拼写混乱** → 全部清理，命名统一。

## 7. 已知差异 / 注意点

- **`PlayableStateBase` 自动生命周期**：`OnEnter` 自动 `timeline.Restart()`、`OnUpdate` 自动推进、`OnExit` 若仍在播放则 `End()`（已播完不二次触发 EndAction）。子类 override 时可调 `base` 保留自动行为，或完全接管。
- **零时长转移**：`Timeline.Length==0` 时 `NormalizedTime` 恒为 1，一次 `Update` 即播完过渡事件并进入目标（旧库行为一致）。
- **`SubStateMachineState` / `SubStateMachineTransition` 的 targetState/destinationState 必须已注册到子机**。
- **中断只在目标状态带触发条件的出边中查找**（无条件出边不参与，避免立即打断当前过渡）。
- **机器当状态**：`StateMachine<T>` 实现 `IState<T>`，`OnEnter` 确定性进初始态、`OnUpdate` 推进本机、`OnExit` 退出当前子状态。子状态机包装类（`WithCallbackStateMachineState` 等）直接复用该语义。

## 8. 验证

- **编译**：Unity 2022.3 LTS 打开项目，确认 `MNAC.StatesNew` 无编译错误，旧 `MNAC.States` 不受影响。
- **可选单测**：新建 `Assets/Scripts/States_New/Tests/` 的 Editor 测试程序集，覆盖 Core（AddState/RemoveState 清理边、SetInitialState、自转移 no-op、短路、异常包装）与 Playable（零时长一次完成、Blending offset/fixedExitTime、中断路径）。
- **迁移验证**：挑一个真实用例（如 `TargetLocker`）切到新库跑 Play 模式比对行为。
