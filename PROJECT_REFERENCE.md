# Project Reference — un_srp_2d

> **Scope:** Personal practice project for Unity 2D development skills.  
> **Not intended for production use.**

## Purpose

Each **scene** in this project is dedicated to practicing a specific Unity game-development skill or implementing a particular feature. The goal is incremental, focused learning — one topic per scene.

---

## Environment

| Item | Value |
|---|---|
| Unity Version | 2022.3.62f3c1 (LTS) |
| Render Pipeline | URP 14.0.12 (2D Renderer) |
| .NET Workload | ManagedGame |
| IDE Support | Rider, Visual Studio |

## Key Packages

| Package | Version | Purpose |
|---|---|---|
| `com.unity.feature.2d` | 2.0.1 | 2D game feature set (sprites, tilemap, etc.) |
| `com.unity.render-pipelines.universal` | 14.0.12 | URP / 2D rendering |
| `com.unity.inputsystem` | 1.14.2 | New Input System |
| `com.unity.textmeshpro` | 3.0.9 | Text rendering |
| `com.cysharp.unitask` | 2.5.10 (git) | Async/await for Unity |
| `jp.hadashikick.vcontainer` | 1.17.0 (git) | Dependency Injection container |

---

## Project Structure

```
un_srp_2d/
├── Assets/
│   ├── Inputs/              # Input Action Assets & generated C# classes
│   ├── Scenes/              # Unity scene files (one per practice topic)
│   ├── Scripts/             # All C# source code (mirrors root Scripts/ structure)
│   │   ├── Bootstrap/       # App entry-point & initialization
│   │   ├── Core/            # Domain layer (framework-free)
│   │   │   ├── Contracts/   # Interfaces & abstractions
│   │   │   ├── Entities/    # Domain entities
│   │   │   └── UseCases/    # Business logic / interactors
│   │   ├── Features/        # Feature modules (per-scene logic)
│   │   │   ├── Camera/      # Camera zoom system (ZoomController + ZoomParams)
│   │   │   ├── Inventory/
│   │   │   ├── Level/
│   │   │   ├── Movement/    # Generic movement system (Strategy + Processor + Controller)
│   │   │   ├── Player/
│   │   │   └── UI/
│   │   ├── Infrastructure/  # External-facing implementations
│   │   │   ├── Adapters/    # Unity API wrappers & adapters
│   │   │   ├── Data/        # Persistence / data access
│   │   │   └── DI/          # VContainer lifetime scopes & registrations
│   │   └── Shared/          # Cross-cutting utilities & extensions
│   └── Settings/            # URP & renderer settings
│       ├── Renderer2D.asset
│       ├── UniversalRP.asset
│       └── Scenes/
├── Art/                     # Sprites, textures, visual assets
├── Audio/                   # Sound effects & music
├── Configs/                 # Game configuration / data assets
├── SysDocs/                 # Detailed system design documents
├── Packages/
│   ├── manifest.json
│   └── packages-lock.json
└── ProjectSettings/
```

### Architecture Notes

- **Clean Architecture** separation: `Core` has zero Unity dependencies; `Infrastructure` and `Features` depend on `Core`, not the other way around.
- **VContainer** is used for DI — scene-level Scope + entity-level child Scopes. Shared services in scene scope, entity-specific bindings in child scope.
- **UniTask** replaces coroutines for async operations.
- **New Input System** is enabled (old Input Manager is also present in ProjectSettings for backward compatibility). Input Action Assets use generated C# class + manual code management.
- **Feature folders** in `Scripts/Features/` hold generic, reusable systems (e.g., `Movement/`). `Shared/` is reserved for utilities, extensions, and global events.

---

## Scene Index

> Fill in this table as scenes are created. Each row = one practice session / feature.

| # | Scene Name | Skill / Feature Practiced | Status |
|---|---|---|---|
| 1 | `PlayerMovement` | 2D character movement pipeline (Input → Process → Physics) + Gamepad analog stick sensitivity + Camera zoom system | Implemented |
| 2 | *(to be created)* | | |

### Existing Scenes

| Scene | Notes |
|---|---|
| `SampleScene.unity` | Default URP 2D scene — may be reused or replaced |
| `PlayerMovement.unity` | Practice scene for movement flow, analog sensitivity, and camera zoom |

---

## Conventions

- **One scene → one practice topic.** Keep scenes self-contained.
- **Feature folders** in `Scripts/Features/` correspond to the scene or domain area they serve.
- **DI scopes** go in `Scripts/Infrastructure/DI/`; register per-scene dependencies there.
- **No production pressure** — prioritize learning, experimentation, and clean code over shipping.
- **Namespace convention** follows folder hierarchy under `Assets/Scripts/`, e.g. `UnSrp2d.Core.Contracts`, `UnSrp2d.Features.Movement`.
- **Detailed system docs** go in `SysDocs/` — one document per feature/system.
