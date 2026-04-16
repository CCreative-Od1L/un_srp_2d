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
│   │   │   ├── Npc/         # NPC AI: patrol movement + collision response
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
| 2 | *(to be created)* | NPC AI: patrol movement + collision response | Planned |

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

---

## Key Interfaces (Core/Contracts)

| Interface | Purpose |
|---|---|
| `IInputProvider` | Exposes `MoveDirection` Vector2 |
| `IInputConfig` | Exposes `Deadzone` for input threshold |
| `IMovementStrategy` | Returns `MovementInput` (Direction + Magnitude) per frame |
| `IMovementParamsProvider` | Exposes `MovementParams` (MaxSpeed, Acceleration, etc.) |
| `IMovementStateProvider` | Exposes `MovementState` (Velocity, Direction, Speed, IsMoving, ActualVelocity) |
| `ICameraInput` | Exposes zoom input state (ZoomIn, ZoomOut, ZoomModeActive) |
| `IAIController` | Exposes AI desired direction and `WantsToMove` flag for NPC movement |
| `INpcResponse` | Handles collision response behavior on NPC |
| `IPositionProvider` | Exposes entity position for AI state updates |

## Key Types (Core/Contracts)

| Type | Description |
|---|---|
| `MovementState` | Immutable struct with Velocity, Direction, Speed, IsMoving, ActualVelocity, IsActuallyMoving |
| `MovementInput` | Struct with Direction and Magnitude |

---

## DI Architecture

### Lifetime Scopes

1. **PlayerMovementLifetimeScope** (Scene-level)
   - Registered on Scene's root GameObject
   - Provides: `IInputProvider`, `ICameraInput`, `IInputConfig`

2. **EntityLifetimeScope** (Entity-level, Player)
   - Registered on Player prefab (child of Scene Scope)
   - Provides: `IMovementStrategy` (InputMovementStrategy), `IMovementParamsProvider`, `MovementProcessor`

3. **CameraLifetimeScope** (Camera-level)
   - Registered on Camera GameObject
   - Provides: `CameraZoomController` component registration

4. **NpcEntityLifetimeScope** (Entity-level, NPC — Planned)
   - Registered on NPC prefab (child of Scene Scope)
   - Provides: `IMovementStrategy` (AIMovementStrategy), `IAIController` (PatrolAIController), `INpcResponse` (DialogueResponse)

---

## NPC System (Planned)

### Overview

NPC system adds autonomous entity movement and collision interaction, reusing the existing Movement pipeline by swapping `InputMovementStrategy` with `AIMovementStrategy`.

### Data Flow

```
IAIController (PatrolAIController)
  → AIMovementStrategy (IMovementStrategy implementation)
    → MovementProcessor (existing)
      → MovementController (existing)
        → Rigidbody2D

NpcCollisionHandler (OnCollisionEnter2D)
  → INpcResponse (DialogueResponse)
    → Debug.Log (dialogue text)
```

### Core Interfaces

| Interface | Purpose |
|---|---|
| `IAIController` | AI decision interface — returns `DesiredDirection` (Vector2) and `WantsToMove` (bool) |
| `INpcResponse` | Collision response — `Respond()` called when player collides with NPC |
| `IPositionProvider` | Position source for AI tick (returns `transform.position` as Vector2) |

### PatrolAIController

Pure C# AI controller implementing `IAIController`:

- **Anchor position**: set at Start from `transform.position`
- **Patrol radius**: configurable range around anchor
- **States**: `Idle` (random wait duration) → `Moving` (toward target point) → `Idle`
- **Target selection**: random point within patrol radius, clamped to valid range
- **Arrival threshold**: transitions to Idle when within configurable distance of target

### AIMovementStrategy

Implements `IMovementStrategy`, bridges AI decision to movement:

- Reads `IAIController.DesiredDirection` and `IAIController.WantsToMove`
- Returns `MovementInput.Zero` when `WantsToMove` is false
- Returns full-magnitude `MovementInput` in the desired direction otherwise
- Calls `_controller.Tick(deltaTime, position)` before reading direction

### NpcParams ScriptableObject

| Field | Description |
|---|---|
| `PatrolRadius` | Maximum distance from anchor for random target selection |
| `IdleDurationMin` | Minimum wait time between patrol segments |
| `IdleDurationMax` | Maximum wait time between patrol segments |
| `ArrivalThreshold` | Distance at which NPC considers itself arrived |
| `DialogueText` | Text printed to console on collision |

### Future Extensions

| Feature | Implementation Path |
|---|---|
| Chase behavior | New `ChaseAIController : IAIController`, swap in `NpcEntityLifetimeScope` |
| Dialogue UI panel | Replace `DialogueResponse` with UI-based `INpcResponse` |
| Interaction key (talk) | Add `IInteractable` interface, check input in `NpcCollisionHandler` |
| Behavior switching | `ICompositeAIController` that switches between sub-controllers |
