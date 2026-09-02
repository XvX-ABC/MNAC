# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

本文档基于对项目代码的实际检查编写。MNAC（迷你装甲核心）是一款受《装甲核心》系列启发的第三人称 3D 动作射击游戏，使用 Unity 引擎开发。开发者强调该项目"大框架已迭代两版、功能多为早期骨架、注释稀少"，因此代码中并存新旧两套系统、命名空间混乱。**2026-09 已做过一轮整理**：确认死代码/废弃原型归档到根 `_Archive/`，多个错拼目录改名对齐命名空间，`Tess.AI` 等坏前缀归位（详见文末"整理记录"）。但同名类多处并存、跨 asmdef 的 `States/Composable` 与 `Locomotion_New` 实验等历史遗留仍在。动手改代码前务必先阅读目标文件确认它属于哪套系统。

## 版本与技术栈（已核对）

- **Unity 编辑器**: **2022.3.62f3c1 (LTS)**（`ProjectSettings/ProjectVersion.txt`；注意不是 Unity 6）
- **渲染管线**: URP `com.unity.render-pipelines.universal` 16.0.6
- **输入系统**: `com.unity.inputsystem` 1.7.0（新输入系统）
- **关键包**: Cinemachine 2.10.3、Timeline 1.8.6、Visual Effect Graph 16.0.6、`com.unity.ai.navigation` 1.1.7、`com.unity.test-framework` 1.3.9
- **第三方插件**: Behavior Designer（AI 行为树）、Final IK（RootMotion，IK）、DOTween、OccaSoftware Crosshairs、GUIPack-Clean&Minimalist

## 程序集（asmdef）结构

代码按 asmdef 划分为独立程序集；**游戏粘合层**（`Characters/`、`Behaviours/`、`AI/` 等大部分目录）没有 asmdef，编译进默认 `Assembly-CSharp`。命名程序集之间通过 asmdef `references` 中的 **GUID** 互相引用。

| 程序集 | 目录 | 内容 |
| --- | --- | --- |
| `MNAC.States` | `Assets/Scripts/States/` | 分层状态机系统 |
| `MNAC.Animation` | `Assets/Scripts/Animations/` | Playables API 封装 |
| `MNAC.Interaction` | `Assets/Scripts/Interaction/` | 交互、影响、目标锁定 |
| `MNAC.TPhysics` | `Assets/Scripts/TPhysics/` | 环境与运动物理 |
| `MNAC.UI` | `Assets/Scripts/UI/` | UI 核心 |
| `MNAC.Utilities` | `Assets/Scripts/Utilities/` | 黑板、可组合组件、时间轴 |
| `MNAC.Weapon` | `Assets/Scripts/Weapons/` | 模块化武器系统 |
| `MNAC.TPhysics.Tests` | `Assets/Scripts/TPhysics/Tests/` | 物理单元测试（仅 Editor，NUnit） |
| `MNAC.Utilities.Tests` | `Assets/Scripts/Utilities/Tests/` | 工具单元测试（仅 Editor，NUnit） |
| `MNAC.StatesNew` | `Assets/Scripts/States_New/` | 状态机库重构（新，纯 C#，与旧 `MNAC.States` 并行，尚未被引用） |
| `ConfigFramework` | `Assets/Scripts/ConfigFramework/` | 配置框架（Luban 风格） |
| `ConfigFramework.Editor` | `Assets/Scripts/ConfigFramework/Editor/` | 配置导出工具（仅 Editor） |

测试程序集用 `overrideReferences: true` + `nunit.framework.dll`，受 `UNITY_INCLUDE_TESTS` 定义约束，仅 Editor 平台编译。**注意**：asmdef 中存在两个悬空 GUID 引用（`d8b63aba…` 与 `6055be8e…`），不影响现有编译，但新增引用时别照抄。

## 关键系统（实际类名均已核对）

### 1. 状态机（`Assets/Scripts/States/`，`MNAC.States`）

全部类为**泛型**设计。核心继承链：
- `StateMachineBase<TState, TContext>`：基础状态机
- `PlayableStateMachine<T>` → 状态 `PlayableStateBase<T>`、过渡 `PlayableTransition<T>`
- `BlendingTransition<T>`：带固定退出时间、起始偏移、打断的混合过渡
- `WithCallbackPlayableState<T>`：提供 OnEnter/OnUpdate/OnExit 回调
- `SubStatemachineState<T>`：分层（子）状态机支持

UI 侧存在配套的 `*_MonoComponent<T>` 与 `*_SO<T>` 变体（MonoBehaviour 包装与 ScriptableObject 定义），同一套逻辑三种载体。

### 2. 角色控制器（`Assets/Scripts/Characters/Humanoid/`）

`HumanoidController : CharacterComponent`（`internal`）是主控制器，通过 `RequiredComponents` 装配：`HumanoidInputComponent`、`UICore`、`TargetLockerBase`、`EnvironmentCore`、`LegsController`、`LocomotionCore`、左右 `ArmController`、`WeaponBackpack`。角色状态机为 `CharacterBehavioursStatemachine`（正常态 / 死亡态）。

### 3. 手臂/武器行为（`Assets/Scripts/Behaviours/Arms/` 与 `Assets/Scripts/Characters/Humanoid/Arms/`）

- 接口 `IArmedArmBehaviour` → 基类 `ArmedArmBehaviourBase`（**不是** `ArmedWeaponArmBehaviourBase`）
- ScriptableObject 定义：`ArmedArmBehaviourBase_SO : StateComponentNode_SO`
- 控制器：`ArmedArmBehaviourController<T>`
- 两种具体实现：
  - `ArmedLauncherArmBehaviour_SO`（`…Arms.Weapons.Launchers`）：基于 Final IK 的持枪瞄准 + 状态动画过渡
  - `ArmedSwordArmBehaviour_SO`（`…Arms.Weapons.Sword`）：近战，动作激活武器相关功能
- 相关文件命名有历史包袱：如文件 `ArmedWeaponArmBehaviourBase.cs` 实际声明 `ArmedArmBehaviourBase`，`IArmedWeaponArmBehaviour.cs` 实际声明 `IArmedArmBehaviour`，以**文件内声明的类型**为准。

### 4. 运动系统（两层结构）

- **物理层** `MNAC.TPhysics.Locomotion.LocomotionCore`：模块责任链，基于 `ILocomotionModule` 与共享 `Context`，模块通过 `AddModule_InsertByPriority` 按优先级插入（有对应 NUnit 测试）。扩展点：`LocomotionModuleBase`（非泛型版本，位于 `TPhysics/Locomotion/`）。
- **人形层** `Assets/Scripts/Characters/Humanoid/Locomotion/LocomotionCore : HumanoidComponent`：包装物理核心，外加 `LocomotionStatemachine`（`LocomotionStateBase`）驱动状态：`BoostingState`、`QuickBoostingState`、`JumpLocomotionState`、`WalkingState`、`MovementState`、`RotationByPlayerLocomotion` 等。
- 脚部地面适应基于 Final IK：`SimpleFootIK`（`Behaviours/Foots/`）+ `LegsController`（`Characters/Humanoid/Legs/`）。
- `Assets/Scripts/Locomotion_New/` 是泛型重构实验（`LocomotionModuleBase<TContext>`），尚无具体模块，勿当作正式系统。

### 5. 动画（`Assets/Scripts/Animations/`，`MNAC.Animation`）

Playables API 封装：`AnimationPlayablePartBase`（可播放部件）、`AnimationPlayablePartTree : MTree`（节点树）、`OutputSetting`（`internal`，权重控制）。

### 6. 交互/影响（`Assets/Scripts/Interaction/`，`MNAC.Interaction`）

- `InfluenceCore` + `IInfluence`：影响传递核心，`Health`、`IKnockback`、`Stun` 等实现 `IInfluence`。
- `TargetLockerBase<T>`：目标锁定（屏幕空间检测）。
- **注意**：目录已改为 `Interaction/Influences/`，但 `MNAC.Interaction.Influences`（复数）与 `MNAC.Interaction.Influence`（单数）两套命名空间仍并存，还存在第三套 `MNAC.Characters.Interaction`。改代码前先确认 import 的是哪一套。

### 7. AI（`Assets/Scripts/AI/`）

- `AICore : MonoBehaviour`、`AIComponent_Mono`、`AITargetLocker`。
- 自定义行为树动作：`AIActionBase : Action`（Behavior Designer 的 Action），位于 `BTExtensions/`。
- AI 接入方式是向 `HumanoidController` 提供 AI 版输入与目标锁定组件替换玩家输入。
- **命名空间已统一为 `MNAC.AI…`**（曾混有 `Tess.AI`、`Assets.Tests.Scripts.AI(.BTExtensions)`，已归位）。注意同名 `LockTarget`/`TargetLocker` 类仍在 `MNAC.AI`、`MNAC.Characters`、`MNAC.Interaction` 多处并存，改动前 grep 确认。

### 8. 武器（`Assets/Scripts/Weapons/`，`MNAC.Weapon`）

组件化、ScriptableObject 驱动的武器框架：
- 基类：`IWeapon` → `Weapon : MonoBehaviour`（抽象）；`WeaponType` 枚举 `Launcher` / `Sword`。
- 发射器：`ILauncher` → `Launcher : Weapon`（`internal abstract`，内部状态机 Idle/DelayLaunch/Launching/Reload），具体实现 `MachineGun : Launcher`（基于 `ProjectilePoolSource<Bullet>` 的对象池射击）。
- 剑：`ISword` → `Sword : Weapon`（`SwordTipTrigger` 判定命中，`SwordActionType` 动作表）。
- 投掷物：`IProjectile` → `Projectile` → `Bullet : Projectile, IBullet`；对象池 `ProjectilePool<T>`（`UnityEngine.Pool.ObjectPool<T>`）。
- 装载/背包：`MNAC.Weapons.WeaponBackpack`、`WeaponManager`；**注意**存在第二个 `MNAC.Characters.Weapons.WeaponBackpack`（`internal CharacterComponent`，包装前者），`HumanoidController` 依赖的是后者。
- 投掷物目录为 `Projectiles/`（曾拼为 `Projectils_New/`，已改名对齐 ns `MNAC.Weapons.Projectiles`）；`Resources/Weapons_Obsolete/` 是旧版发射器资产。

## 开发命令

### 打开与运行
- 用 Unity **2022.3 LTS** 打开项目根目录（URP 16）。
- 主场景：`Assets/Scenes/Gameplay_0.unity`。测试场景按功能分组在 `Assets/Scenes/{Tests,Character,Weapons,UI}/` 下。

### 测试
- **单元测试（NUnit，EditMode）**：`Window → General → Test Runner` 运行 `MNAC.TPhysics.Tests`、`MNAC.Utilities.Tests`；或命令行（需本机 Unity 编辑器路径）：
  ```bash
  Unity -batchmode -runTests -projectPath . -testPlatform EditMode -testResults results.xml
  ```
- **运行期自测脚本**：`*_Test.cs` MonoBehaviour（如 `Weapons/Launcher/MachineGun/MachineGun_Test.cs`、`UI/UICore_Test.cs`、`Weapons/Sword/Sword_Test.cs`），挂到场景 GameObject 上在 Play 模式验证。
- **主要测试场景**：`Tests/Tests_Base.unity`、`Character/AI/AI_Control.unity`、`Weapons/MachineGun_Test.unity`、`Weapons/Sworld_Test.unity`、`Character/TargetInteractionTest.unity`、`Character/BulletHitTest.unity`、`Character/DiedLocomotionTest.unity`、`UI/Indicator_Test.unity`。

### 构建
- `File → Build Settings`，目标平台 Windows / Android 皆可（包清单含 Android 模块）。

### 配置资源
角色/武器/运动配置均为 ScriptableObject，位于 `Assets/Resources/SO/Characters/C_0/`，实际类名（已核对）：
- `C_0Definitions_SO`（`C_0_Definitions.asset` 与 `C_0_AI_Definitions.asset`）
- `LocomotionDefinitions_SO`（`LocomotionDefinitions.asset` 与 `C_0_AI_LocomotionDefinitions.asset`）
- `ArmDefinitions_SO`（`LeftArmDefinitions.asset` / `RightArmDefinitions.asset`）
- `ArmedLauncherArmBehaviour_SO` / `ArmedLauncherArmBehavioursDefinitions_SO` / `ArmedLauncherArmAnimationDefinitions_SO`
- `ArmedSwordArmBehaviour_SO` / `ArmedSwordArmBehaviourDefinitions_SO` / `ArmedSwordArmAnimationDefinitions_SO`
- `TargetLockerDefinitions_SO`、`WeaponManager_Prefab`（`WeaponManager.asset`）
- 其余资源在 `Assets/Resources/` 下的 `Animations/`、`Audios/`、`Materials/`、`Modules/`（FBX 模型）、`Prefabs/`、`Textures/`。

## 代码库注意事项（实测发现的坑）

1. **命名空间与目录不一致（残余）**：命名空间里 `Launchers`（复数）vs 目录 `Launcher`、`Weapon`（单数）vs `Weapons`、`States` vs `States_New`、目录 `Player` vs 命名空间 `MNAC.Player(s)`。以 `using` 导入的实际命名空间为准。历史错拼目录（`Influense`→`Influences`、`Projectils_New`→`Projectiles`、`Launcher_New`→`Launcher`）已完成改名对齐。
2. **同名类多处并存**：`WeaponBackpack`、`UICore`、`EnvironmentCore`、`DeathState`、`NormalState`、`TargetLockerBase`、`LockTarget`、`Health`/`Stun`/`Knockback` 等都有两到三个版本，分布在 `MNAC.Weapons`/`MNAC.Characters`/`MNAC.Interaction`/`MNAC.AI` 等命名空间。改动前用 `grep` 确认引用的是哪个。
3. **确认死代码已归档到仓库根 `_Archive/`**（2026-09 清理）：`TrashCan/` 整目录与散落 `*_Obsolete.cs`、`Input_Obsolete/`、整文件注释幽灵等全部 `git mv` 移出 `Assets/`（Unity 不再编译，可从归档恢复）。归档时注意文件/类名不一致的坑（如文件名 `ArmedWeaponArmBehaviourControllerState.cs` 声明类 `ArmedArmBehaviourControllerState`，曾被误判为死代码）。仍活跃的旧/新并存：`Interaction/Targets/`（旧）vs `Targets_New/`；`TargetsCatcherBase_New.cs` 类体被注释（迁移未完成）；`Interaction/Targets/ITarget_Obsolete.cs` 与 `Catchers/GameObjsInScreenCatcher_Obsolete.cs` **名带 Obsolete 但仍在用，勿删**。
4. **大部分游戏类为 `internal`**：公开面主要在命名程序集中，粘合层类多为 internal。
5. **通用工具**：`MNAC.Utilities` 提供黑板（Blackboards）、可组合组件（Composable）、时间轴（Timeline）、`MTree` 结构；组件基类多形如 `ComponentBase_MonoComponent` / `*_SO` 三种载体（类 / Mono 包装 / ScriptableObject）。

## 已知问题（ReadMe.md 记录）

- 剑冲刺结束后角色会"踩空"般从空中下落。
- 剑挥砍时因朝向角度与挥砍动画原因打不中目标。
- 装备发射器手臂瞄准有几率不准，切换武器后恢复。
- 角色接入 AI 控制后产生意外旋转。

## 计划进度（`plans.md` 概要）

基础运动（地面/跳跃/空中/QB/手臂旋转及对应动画）已完成；武器直击与 Missile 已完成，Knife 与 Arts 待定；AI、Assembly、Mission 未开始。

## 代码风格

- PascalCase 公共成员 / 类型，camelCase 带下划线前缀的私有字段。
- 序列化字段用 `[SerializeField]`，组件引用在 Awake/Start 缓存。
- 行为配置尽量走 ScriptableObject；数据共享用黑板模式。

## 2026-09 代码整理记录（本分支 n_h，未合并 dev）

每次改动均经 `Unity -batchmode` 全量编译零错误后提交：
- **阶段 0**：Unity 首次导入——`States_New/`(23 文件)补全 meta；修复 `Locomotion_New/PhysicsLocomotionCore.cs` 三个潜伏编译错误（漏 `using MNAC.TPhysics`、readonly 字段重赋、字段类型具体类 vs 接口）。
- **阶段 1**：确认死代码 `git mv` 出 `Assets/` 到根 `_Archive/`（`TrashCan/`、各 `*_Obsolete.cs`、`Input_Obsolete/`、整文件注释幽灵、百度云残留 `.cfg`）。清理 6 处失效 `using MNAC.Input;`。纠正"文件名≠类名"误判（`ArmedWeaponArmBehaviourControllerState.cs` 实际是活跃类）。
- **阶段 2**：目录改名对齐命名空间：`Physics`→`TPhysics`、`Projectils_New`→`Projectiles`、`Influense`→`Influences`、`Launcher_New`→`Launcher`、`Behaviours/Arms/Animation`→`Animations`；修正 `States.asmdef`/`Weapons.asmdef` 的 `rootNamespace`；清理空目录；删空 `namespace Tests.States` 残渣。
- **阶段 3（命名空间规范化，符号级、行为不变）**：`Tess.AI`/`Assets.Tests.Scripts.AI(.BTExtensions)` → `MNAC.AI…`；`IWeaponControlInput` 归位到 `MNAC.Behaviours.Input`；全局命名空间的 `ComponentCantFindException`/`ComponentException` 包进 `MNAC.Utilities`。

**未做（需单独决策）**：`States/Composable` 目录在 `MNAC.States` 程序集却声明 `MNAC.Utilities.Composable`（跨程序集边界）；`Locomotion_New/` 泛型实验去留；`MNAC.Characters.Weapons` 与 `MNAC.Weapons` 同名运行时类合并（改行为，需运行时回归）；`Player`/`Players` 单复数统一；`States` vs `States_New` 迁移。
