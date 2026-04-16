# un_srp_2d

Unity 2D 实践项目，用于逐步学习和掌握 Unity 2D 开发技术。

> **项目定位**：个人学习用途，不适用于生产环境。

---

## 技术栈

| 项目 | 版本 |
|---|---|
| Unity | 2022.3.62f3c1 (LTS) |
| 渲染管线 | URP 14.0.12 (2D Renderer) |
| 输入系统 | New Input System 1.14.2 |
| 依赖注入 | VContainer 1.17.0 |
| 异步操作 | UniTask 2.5.10 |
| 摄像机系统 | Cinemachine 2.10.3 |

---

## 项目结构

```
Assets/
├── Inputs/              # InputActionAsset 及生成的 C# 类
├── Scenes/              # Unity 场景文件（每个场景对应一个练习主题）
├── Scripts/
│   ├── Core/            # 领域层（无 Unity 依赖）
│   │   └── Contracts/   # 接口与抽象
│   ├── Features/        # 功能模块
│   │   ├── Camera/      # 摄像机系统
│   │   └── Movement/    # 移动系统
│   ├── Infrastructure/  # 基础设施层
│   │   ├── Adapters/    # Unity API 适配器
│   │   └── DI/          # VContainer 生命周期作用域
│   └── Shared/          # 跨模块工具
└── Settings/            # URP 配置

SysDocs/                 # 各系统详细设计文档
```

### 架构原则

- **Clean Architecture**：`Core` 层零 Unity 依赖；`Infrastructure` 和 `Features` 依赖 `Core`，不反向依赖
- **VContainer DI**：场景级 Scope + 实体级 Child Scope
- **策略模式**：移动系统通过 `IMovementStrategy` 接口支持不同输入来源（玩家输入 / AI）
- **命名空间**：`UnSrp2d.{路径}`，例如 `UnSrp2d.Core.Contracts`

---

## 已实现系统

### 移动系统 (`PlayerMovement` 场景)

完整的 2D 角色移动管线：

```
InputActionAsset → InputActionProvider → InputMovementStrategy
  → MovementProcessor → MovementController → Rigidbody2D
```

- New Input System（支持 WASD / 方向键 / 手柄左摇杆）
- 手柄模拟摇杆灵敏度（Deadzone + Magnitude 归一化）
- 平滑加减速（`Vector2.MoveTowards` + 可配置加速度/减速度）
- 面向方向（`SpriteRenderer.flipX`）
- 调试 Gizmos（速度矢量可视化）

### 摄像机跟随 + 缩放 (`PlayerMovement` 场景)

- Cinemachine Virtual Camera + Framing Transposer 平滑跟随
- Cinemachine Confiner 2D 场景边界约束
- 摄像机缩放：按住模式键（Tab / L1）再用 Q/E 或 RT/LT 调整

---

## 练习场景

| 场景 | 内容 | 状态 |
|---|---|---|
| `SampleScene` | 默认 URP 2D 场景 | 存在 |
| `PlayerMovement` | 移动系统 + 摄像机跟随 + 摄像机缩放 | 已实现 |
| *(待创建)* | NPC AI：巡逻移动 + 碰撞响应 | 规划中 |

---

## 详细设计文档

- `SysDocs/MovementSystem.md` — 移动系统完整设计规范
- `SysDocs/CameraFollowSystem.md` — 摄像机跟随 + 缩放系统设计规范
- `PROJECT_REFERENCE.md` — 项目整体参考（项目结构、场景索引、约定规范）

---
