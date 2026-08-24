# MNAC — 配置数据工作流需求分析（Luban + Protobuf）

> 项目：MNAC（迷你装甲核心）
> 环境：Unity 6（URP 16.0.6）· C# · 现有配置为 ScriptableObject + JSON
> 目标：引入 **Luban 配置工具链 + Protobuf 序列化**，以 **Excel 为真源**，并支持**运行时热更**
> 技术选型：**Luban**（开源配置代码/数据生成器）+ **Google.Protobuf**（运行时反序列化）
> 版本锁定：**Luban v4.9.0**（2026-05-28 发布，当前最新稳定 release）
> 文档性质：需求分析 + 功能拆解（不涉及具体代码实现）

---

## 1. 现状分析

### 1.1 现有配置方式

| 类型 | 用途 | 位置 | 特点 |
|---|---|---|---|
| **ScriptableObject** | 角色/武器/运动/动画/状态定义 | `Assets/Resources/SO/`（17 个 `.asset`） | 可视化编辑、类型安全，但不适合策划批量配表、不便热更、字段改动需改代码 |
| **JSON** | 发射器等参数（`Assets.json`） | 走 AssetBundle 加载 | 无 schema 校验、易拼写错、逐字段易漂移 |

### 1.2 现有资源/配置加载基础设施（可复用）

| 类 | 位置 | 职责 |
|---|---|---|
| `LoadAssetBundleManager` | `Utilities/Assets/DataControl/` | 单例；从 `StreamingAssets` 同步/异步加载 AssetBundle、依赖解析、卸载 |
| `FileAssetLoader_Json<T>` | `Utilities/Assets/DataControl/` | JSON 配置加载器：编辑器直读文件，运行时从 AB 读 `TextAsset` 反序列化 |

关键结论：MNAC 已有一条「**JSON 文本 + AssetBundle 资源包**」的配置管线雏形，但：
- ❌ 真源不是 Excel（策划改表不友好）
- ❌ 序列化用 JSON 文本（无 schema 校验、无强类型、体积大、易出错）
- ❌ **没有真正的热更**：无「下载 → 版本校验 → 原子替换 → 运行时重载」闭环
- ❌ 导出/校验全靠手写，缺一套成熟的工具链

---

## 2. 技术选型：为什么用 Luban

[Luban](https://github.com/focus-creative-games/luban) 是一个**开源（MIT）的游戏配置解决方案**——不只是导表工具，而是「数据源 → 代码 + 数据」的生成器。它能直接覆盖我们原本要自研的一整块工具链：

| 需求 | Luban 现成能力 |
|---|---|
| Excel 读取 | 内建支持 Excel 族（csv/xls/xlsx/xlsm）、json、yaml 等 |
| 强类型 schema | 支持用表结构定义 + **OOP 类型继承**（可表达技能/行为树等复杂数据） |
| 代码生成 | 生成 C# 强类型类 + **protobuf schema（proto2/proto3）**，多语言可选 |
| 数据导出 | 导出 **protobuf binary / json**、msgpack、flatbuffers 等，每表独立文件 |
| 数据校验 | 内建 **ref 引用检查 / path 资源路径 / range 范围** 等校验 |
| 本地化 | 内建 i18n/l10n 机制（静态/动态文本、main-patch 多地区） |
| 热更兼容 | 生成代码无反射接口，兼容 hybridclr/ilruntime/xlua/puerts 等热更方案 |

**结论**：用 Luban 的 `code_protobuf3 + data_protobuf_bin` 导出，即可拿到「Excel → protobuf schema + C# 强类型代码 + 二进制数据」的完整产物，原本要自研的导出工具链（读 Excel、字段映射、校验、序列化、代码生成）**大部分由 Luban 承担**。

> Luban 负责**工具链**（编辑器/构建期：定义 → 校验 → 生成代码 → 导出数据）；
> 我们仍自研**运行时**（加载、合并、重载）与**热更下载/替换**（Unity 侧逻辑）。

---

## 3. 需求拆解（需要实现的功能）

按「Luban 工具链 / 导出接入 / 运行时加载 / 热更 / 集成改造」五层拆解，标注 **【Luban 提供】** 与 **【自研】**。

### 3.1 Luban 工具链层（配置为主，少量自研）

| 功能 | 归属 | 说明 |
|---|---|---|
| **F1 表结构定义** | Luban 提供 | 用 `__tables__.xlsx` / `__beans__.xlsx`（或 proto schema）定义表结构与字段，作为唯一 schema 真源 |
| **F2 Excel 读取** | Luban 提供 | 内建解析 xlsx/csv/xlsm 等 |
| **F3 代码生成** | Luban 提供 | `--gen_types code_protobuf3` 生成 C# 强类型类 + `.proto` schema |
| **F4 数据导出** | Luban 提供 | `--gen_types data_protobuf_bin` 导出 protobuf binary，每表一个独立数据文件 |
| **F5 数据校验** | Luban 提供 | 内建 ref/path/range 校验，导出失败即报错阻断 |
| **F6 本地化** | Luban 提供（可选） | 文本/时间本地化，MVP 可暂缓 |

### 3.2 导出接入层（自研薄封装）

| 功能 | 归属 | 说明 |
|---|---|---|
| **F7 导出脚本封装** | 自研 | 封装 `Luban.ClientServer.dll` 命令行，提供 Unity 菜单「一键导出」 |
| **F8 构建接入** | 自研 | 构建前自动触发 Luban 导出，产物入 `StreamingAssets` |

> Luban 命令行示例：`dotnet Luban.ClientServer.dll -j cfg -- -d <root_def> --input_data_dir <excel> --output_code_dir <code> --output_data_dir <data> --gen_types code_protobuf3,data_protobuf_bin -s client`

### 3.3 运行时加载层（自研）

| 功能 | 归属 | 说明 |
|---|---|---|
| **F9 ConfigManager** | 自研 | 单例；强类型访问 `Get<T>(id)` / `GetAll<T>()`；广播 `ConfigReloaded` 事件 |
| **F10 配置加载器** | 自研 | 用 `Google.Protobuf` 反序列化 Luban 导出的 `.bytes`（每表一个 `Table` 类，如 `TbItem`） |
| **F11 合并策略** | 自研 | 加载优先级：**热更目录 > 内置（StreamingAssets）** |
| **F12 重载通知** | 自研 | `ConfigReloaded` → 玩家 Stats 重算、武器/敌人配置重读、UI 刷新 |

### 3.4 热更层（自研）

| 功能 | 归属 | 说明 |
|---|---|---|
| **F13 版本清单** | 自研 | 维护 manifest（version + 每表 hash），对比本地与远程差异（Luban 每表独立文件，可做表粒度 diff） |
| **F14 下载配置** | 自研 | `UnityWebRequest` 下载差异表数据 |
| **F15 原子替换** | 自研 | 先写临时文件再 `rename` 覆盖，防半写/脏文件 |
| **F16 重载触发** | 自研 | 下载校验通过后触发 `ConfigReloaded`，运行时无感生效 |
| **F17 本地模拟热更** | 自研 | 「热更测试」按钮：放新配置进热更目录 → 运行时重载 → 数值生效，验证闭环 |

### 3.5 与现有系统集成 / 改造（自研）

| 功能 | 归属 | 说明 |
|---|---|---|
| **F18 加载器改造** | 自研 | `FileAssetLoader_Json<T>` → 增加 `FileAssetLoader_Proto<T>`，反序列化换成 protobuf，复用 `LoadAssetBundleManager` |
| **F19 资源引用改造** | 自研 | SO 里直接引用的 Prefab/资源 → 配置里改 `assetKey` 字符串，经 AssetBundle 加载（数据与资产解耦） |
| **F20 迁移/共存策略** | 自研 | 数值型配置迁 Luban（武器/运动/敌人/波次），复杂对象引用型暂留 SO，过渡期双轨 |

---

## 4. 数据流

```
Excel（策划真源）
  │  Luban.ClientServer（定义表结构 → 校验 → 生成代码 + 导出数据）
  ├─ 生成：.proto schema + C# 强类型代码（code_protobuf3）
  └─ 导出：protobuf binary 数据 .bytes（data_protobuf_bin），每表一个文件
  ▼
.bytes 产物
  ├─ 内置版本：StreamingAssets（随包发布）
  └─ 热更版本：persistentDataPath/ConfigPatch（manifest 校验后覆盖）
  ▼
ConfigManager（Google.Protobuf 反序列化 / 合并 / 重载通知）
  │  Get<T>(id) / GetAll<T>()
  ▼
运动 / 锁定 / 武器 / 敌人 / 组装 …（全部读配置，不写死数值）
```

---

## 5. 热更语义

| 项 | 说明 |
|---|---|
| **范围** | 配置热更（数值/零件/武器/敌人），不含代码热更 |
| **触发** | 启动时检查远程 manifest → 有差异则下载 → 原子替换 → `ConfigReloaded` 重载 |
| **重载** | 玩家 Stats 重算、武器/敌人配置重读、UI 刷新 |
| **版本** | manifest 带 version + 每表 hash，表粒度 diff，避免加载半写/脏文件 |
| **MVP 验证** | 「热更测试」按钮：放新配置进热更目录 → 运行时重载 → 数值生效 |
| **与 Luban 关系** | Luban 产出「按表独立的数据文件」，天然支持表粒度热更；「下载/替换/重载」的 Unity 运行时逻辑由自研热更层完成 |

---

## 6. 依赖库与工具

| 库/工具 | 用途 | 备注 |
|---|---|---|
| **Luban** | 配置代码/数据生成器（Excel→代码+数据） | 开源 MIT，`Luban.ClientServer.dll` |
| **Google.Protobuf** | 运行时反序列化 Luban 导出的二进制 | 官方，免费 |
| （可选）**protoc** | 仅当需自行再生成 proto 时用 | Luban 已生成 .proto，通常无需额外装 |

> 均兼容 Unity 6。现有 `LoadAssetBundleManager`、`FileAssetLoader_Json` 继续复用，无需替换。

---

## 7. 实施里程碑（每步可运行验证）

| 里程碑 | 内容 | 验收 |
|---|---|---|
| **M1 工具链打通** | 引入 Luban + Google.Protobuf；写 1 张示范表（如武器参数）→ Luban 导出 protobuf → ConfigManager 读到 | 控制台能 `Get<WeaponConfig>(id)` 取到 Excel 配的值 |
| **M2 加载器改造** | `FileAssetLoader_Proto<T>` 落地，替换 JSON 加载；配置走 AssetBundle | 运行时从 AB 加载 Luban 二进制成功 |
| **M3 迁移** | 数值型 SO（武器/运动/敌人）迁到 Luban + proto，`assetKey` 解耦 | 迁移后游戏行为与原来一致 |
| **M4 热更闭环** | manifest + 下载 + 原子替换 + 重载 + 本地模拟热更 | 热更测试按钮 → 改数值 → 运行时重载生效 |
| **M5 打磨** | 构建接入、校验增强、错误处理、异常回滚 | 导出/热更流程稳定，脏数据能阻断 |

---

## 8. 风险与对策

| 风险 | 影响 | 对策 |
|---|---|---|
| Luban 上手成本（表结构定义语法） | 前期慢 | M1 先用官方示例跑通，再迁移真实表 |
| 表结构/字段与代码漂移 | 导出失败/数据错 | Luban 表结构为唯一 schema，导出前强校验，字段不一致即报错 |
| 热更半写/脏文件 | 加载崩溃 | 原子替换（临时文件 + rename）+ manifest hash 校验 |
| SO → Luban 迁移量大 | 进度受阻 | M3 分表渐进迁移，数值型先迁、复杂引用型暂留 SO 双轨 |
| 热更重载引发引用失效 | 运行时异常 | 重载走事件通知，各模块统一重读，避免持有旧配置引用 |

---

## 9. 一句话总结

用 **Luban** 承担「Excel 读取 → 表结构定义 → 数据校验 → 生成 C# 代码 → 导出 protobuf 数据」的完整工具链（取代自研导表），运行时用 **Google.Protobuf + 自研 ConfigManager** 做强类型加载与重载通知，热更端自研「manifest 版本对比 + 下载 + 原子替换 + 重载」，在复用现有 `LoadAssetBundleManager`/`FileAssetLoader_Json` 框架的基础上，形成「改 Excel → Luban 导出 → 热更 → 运行时生效」的闭环。

---

## 参考

- [Luban 官方仓库 focus-creative-games/luban](https://github.com/focus-creative-games/luban)
- [Luban 生成代码/数据（generate_code_data）Wiki](https://github.com/focus-creative-games/luban/wiki/generate_code_data)
