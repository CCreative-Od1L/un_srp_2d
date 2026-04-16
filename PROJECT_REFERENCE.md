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
| `com.unity.cinemachine` | 2.10.3 | Camera system |

---

## Project Structure

```
un_srp_2d/
├── Assets/
│   ├── Inputs/              # Input Action Assets & generated C# classes
│   ├── Scenes/              # Unity scene files (one per practice topic)
│   ├── Scripts/             # All C# source code
│   │   ├── Core/            # Domain layer (framework-free)
│   │   │   ├── Contracts/   # Interfaces & abstractions
│   │   │   ├── Entities/    # Domain entities
│   │   │   └── UseCases/    # Business logic / interactors
│   │   ├── Features/        # Feature modules (per-scene logic)
│   │   │   ├── Camera/      # Camera zoom control
│   │   │   ├── Movement/    # Generic movement system
│   │   │   └── Npc/         # NPC AI (Planned)
│   │   ├── Infrastructure/  # External-facing implementations
│   │   │   ├── Adapters/    # Unity API wrappers & adapters
│   │   │   ├── Data/        # Persistence / data access
│   │   │   └── DI/          # VContainer lifetime scopes & registrations
│   │   └── Shared/          # Cross-cutting utilities & extensions
│   └── Settings/            # URP & renderer settings
├── Art/                     # Sprites, textures, visual assets
├── Audio/                   # Sound effects & music
├── Configs/                 # Game configuration / data assets
├── SysDocs/                 # Detailed system design documents
├── Packages/
│   ├── manifest.json
│   └── packages-lock.json
└── ProjectSettings/
```

### Architecture Principles

- **Clean Architecture**: `Core` has zero Unity dependencies; `Infrastructure` and `Features` depend on `Core`, not vice versa
- **VContainer** for DI — scene-level Scope + entity-level child Scopes
- **UniTask** replaces coroutines for async operations
- **New Input System** with `.inputactions` assets and generated C# classes
- **Namespace convention**: `UnSrp2d.{FolderPath}` (e.g., `UnSrp2d.Core.Contracts`, `UnSrp2d.Features.Movement`)

---

## Scene Index

| # | Scene Name | Skill / Feature Practiced | Status |
|---|---|---|---|
| 1 | `SampleScene` | Default URP 2D scene | Existing |
| 2 | `PlayerMovement` | 2D character movement pipeline + Camera follow + Camera zoom | Implemented |
| 3 | *(to be created)* | NPC AI: patrol movement + collision response | Planned |

---

## Conventions

- **One scene → one practice topic.** Keep scenes self-contained.
- **Feature folders** in `Scripts/Features/` correspond to the scene or domain area they serve.
- **DI scopes** go in `Scripts/Infrastructure/DI/`; register per-scene dependencies there.
- **No production pressure** — prioritize learning, experimentation, and clean code over shipping.
- **Detailed system docs** go in `SysDocs/` — one document per feature/system.
