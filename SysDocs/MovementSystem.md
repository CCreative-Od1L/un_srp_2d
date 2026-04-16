# Movement System — Design Specification

> Feature: 2D top-down character movement pipeline  
> Scene: `PlayerMovement.unity`  
> Status: Implemented

---

## Overview

A generic, reusable movement system supporting different entity types (Player, Enemy, Projectile, etc.) via Strategy pattern. Follows Clean Architecture with VContainer DI.

### Data Flow

```
InputActionAsset (.inputactions file)
  → InputActionProvider (MonoBehaviour, IInputProvider)
    → InputMovementStrategy (IMovementStrategy)
      → MovementProcessor (pure C#, IMovementStateProvider)
        → MovementController (MonoBehaviour → Rigidbody2D)
        → SpriteDirectionHandler (reads IMovementStateProvider)
        → Future: Animation, Audio systems
```

---

## Components

### Core/Contracts

| Interface | Purpose |
|---|---|
| `IInputProvider` | Abstract input source, exposes `Vector2 MoveDirection` |
| `IInputConfig` | Player input preferences, exposes `float Deadzone` |
| `IMovementStrategy` | Provides `MovementInput` (direction + magnitude) for a given frame; enables analog stick sensitivity |
| `IMovementParamsProvider` | Provides movement parameters, reservable for decorator system |
| `IMovementStateProvider` | Exposes current `MovementState` for consumers |
| `ICameraInput` | Abstract camera input source, exposes zoom mode and direction states |

### MovementState (readonly struct)

| Field | Type | Description |
|---|---|---|
| `Velocity` | `Vector2` | Intended velocity (Processor-calculated) |
| `Direction` | `Vector2` | Normalized direction, persists last valid direction when idle |
| `Speed` | `float` | Magnitude of Velocity |
| `IsMoving` | `bool` | Speed > MinMoveSpeed (based on intended velocity) |
| `ActualVelocity` | `Vector2` | Read back from Rigidbody2D (actual physics result, 1-frame delay) |
| `IsActuallyMoving` | `bool` | ActualVelocity.magnitude > MinMoveSpeed |

### MovementInput (readonly struct)

| Field | Type | Description |
|---|---|---|
| `Direction` | `Vector2` | Normalized direction vector |
| `Magnitude` | `float` | 0.0–1.0, analog stick pressure level after deadzone remapping |

> **Analog Stick Sensitivity:** `Magnitude` is computed via `InverseLerp(Deadzone, 1.0, rawMagnitude)`, which remaps the deadzone-squashed range back to [0,1]. Light push = low magnitude = lower target speed; full push = magnitude 1.0 = full MaxSpeed.

### MovementParams (ScriptableObject)

| Field | Type | Description |
|---|---|---|
| `MaxSpeed` | `float` | Maximum movement speed |
| `Acceleration` | `float` | Speed increase per second |
| `Deceleration` | `float` | Speed decrease per second |
| `MinMoveSpeed` | `float` | Below this value, velocity snaps to zero; eliminates idle/walk flicker during deceleration tail |

### InputConfig (ScriptableObject, IInputConfig)

| Field | Type | Description |
|---|---|---|
| `Deadzone` | `float` | Input magnitude below this → treated as zero; player-configurable preference |

### Infrastructure Implementations

| Class | Base | Registered In |
|---|---|---|
| `InputActionProvider` | MonoBehaviour, `IInputProvider` | Scene Scope (`RegisterComponentInHierarchy`) |

### Feature/Movement Implementations

| Class | Base | Role |
|---|---|---|
| `MovementProcessor` | Pure C#, `IMovementStateProvider` | Calculates velocity via `Vector2.MoveTowards`, stores `MovementState` |
| `MovementController` | MonoBehaviour | Drives `Processor.Tick()` in `FixedUpdate`, writes `Rigidbody2D.velocity`, reads back `ActualVelocity` (next-frame) |
| `SpriteDirectionHandler` | MonoBehaviour | Reads `IMovementStateProvider`, sets `SpriteRenderer.flipX` in `LateUpdate` |
| `InputMovementStrategy` | `IMovementStrategy` | Reads `IInputProvider` + `IInputConfig`, applies 8-dir normalization and deadzone |
| `DefaultMovementParamsProvider` | `IMovementParamsProvider` | Wraps `MovementParams` ScriptableObject, passthrough |
| `InputConfig` | ScriptableObject, `IInputConfig` | Player preference asset for deadzone |

---

## MovementProcessor Logic

```
Tick(deltaTime, actualVelocity):
  input = strategy.GetMovementInput()          # MovementInput (direction + magnitude)
  targetVelocity = input.Direction * params.MaxSpeed * input.Magnitude
  maxDelta = (input.Magnitude > 0) ? params.Acceleration : params.Deceleration
  currentVelocity = MoveTowards(currentVelocity, targetVelocity, maxDelta * deltaTime)
  clamp currentVelocity magnitude to params.MaxSpeed
  if input.Magnitude <= 0 AND currentVelocity.magnitude < params.MinMoveSpeed:
    currentVelocity = zero
  if input.Direction != zero:
    lastDirection = input.Direction
  build MovementState with currentVelocity and actualVelocity
  return MovementState
```

> **Analog Sensitivity:** `input.Magnitude` scales `targetVelocity`, producing different max speeds for light vs. heavy stick pushes. Acceleration rate stays constant — only the target changes, naturally creating the "soft start" feel.

---

## Input Layer

| Decision | Choice |
|---|---|
| Input system | New Input System |
| Asset management | `.inputactions` file + `[SerializeField] InputActionAsset` reference |
| PlayerInput component | Not used — `InputActionProvider` manages lifecycle |
| Composite binding | 2D Vector Composite (WASD / Arrow keys / Gamepad left stick) |
| Read timing | Update: cache input value; FixedUpdate: consume cached value |
| Analog stick support | Magnitude remapped via `InverseLerp` after deadzone; enables pressure-sensitive movement |
| Control schemes | Keyboard (WASD/Arrows), Gamepad (left stick) |

### Action Maps

| Action Map | Actions | Purpose |
|---|---|---|
| `Player` | Move (Vector2) | Movement input |
| `Camera` | ZoomMode (Button), ZoomIn (Button), ZoomOut (Button) | Camera zoom control |

### Input Bindings

| Action | Keyboard | Gamepad |
|---|---|---|
| Move | WASD, Arrow Keys | Left Stick |
| ZoomMode | Tab | Left Shoulder (L1/LB) |
| ZoomIn | Q | Right Trigger (RT) |
| ZoomOut | E | Left Trigger (LT) |

---

## ActualVelocity Read-back Strategy

Strategy A: Next-frame read-back (implemented)

```
FixedUpdate:
  1. Read rb.velocity (actual from PREVIOUS physics step)
  2. Call processor.Tick(deltaTime, actualVelocity)
  3. Set rb.velocity = intended velocity
```

1-frame delay (~16ms @ 60fps) is imperceptible for all consumers (animation, audio, visual).

---

## DI Registration (VContainer)

### Scene Scope: `PlayerMovementLifetimeScope`

```csharp
protected override void Configure(IContainerBuilder builder)
{
    builder.RegisterComponentInHierarchy<InputActionProvider>()
        .As<IInputProvider>();

    builder.RegisterComponentInHierarchy<CameraInputProvider>()
        .As<ICameraInput>();

    builder.RegisterInstance<IInputConfig>(_inputConfig);
}
```

### Entity Scope (child, on Prefab)

```csharp
protected override void Configure(IContainerBuilder builder)
{
    var paramsProvider = new DefaultMovementParamsProvider(_movementParams);
    builder.RegisterInstance<IMovementParamsProvider>(paramsProvider);

    builder.Register<InputMovementStrategy>(Lifetime.Singleton)
        .As<IMovementStrategy>();

    builder.Register<MovementProcessor>(Lifetime.Singleton)
        .AsSelf()
        .AsImplementedInterfaces();

    builder.RegisterComponentInHierarchy<MovementController>();
    builder.RegisterComponentInHierarchy<SpriteDirectionHandler>();
}
```

### Injection Rules

- **Pure C# classes**: Constructor injection (e.g., `MovementProcessor`, `InputMovementStrategy`)
- **MonoBehaviour**: Method injection with `[Inject]` attribute (e.g., `MovementController.Construct()`)
- `RegisterComponentInHierarchy<T>()` finds scene components and injects dependencies
- Child scope auto-resolves dependencies from parent scope

---

## Physics Configuration

| Property | Value |
|---|---|
| BodyType | Dynamic |
| GravityScale | 0 (top-down) |
| CollisionDetection | Continuous |
| Collider2D | Generic — type varies per entity |

---

## Debug

| Feature | Implementation |
|---|---|
| Gizmos | `OnDrawGizmosSelected` — blue: intended velocity, red: actual velocity, yellow: facing direction |

---

## Namespace Convention

Namespaces follow folder hierarchy under `Assets/Scripts/`:

| Path | Namespace |
|---|---|
| `Core/Contracts/` | `UnSrp2d.Core.Contracts` |
| `Features/Movement/` | `UnSrp2d.Features.Movement` |
| `Features/Npc/` | `UnSrp2d.Features.Npc` |
| `Infrastructure/Adapters/` | `UnSrp2d.Infrastructure.Adapters` |
| `Infrastructure/DI/` | `UnSrp2d.Infrastructure.DI` |

---

## Scene Setup

```
Scene Hierarchy:
├── InputSystem (GameObject)
│   ├── InputActionProvider (MonoBehaviour)
│   │   └── Action Asset → PlayerInputActions.inputactions
│   ├── CameraInputProvider (MonoBehaviour)
│   └── PlayerMovementLifetimeScope (MonoBehaviour)
│       └── Input Config → InputConfig.asset
│
├── Player (GameObject)
│   ├── SpriteRenderer
│   ├── Rigidbody2D (Dynamic, GravityScale=0)
│   ├── BoxCollider2D
│   ├── MovementController (MonoBehaviour)
│   ├── SpriteDirectionHandler (MonoBehaviour)
│   └── EntityLifetimeScope (MonoBehaviour)
│       ├── Movement Params → MovementParams.asset
│       └── Parent → PlayerMovementLifetimeScope (component reference)
│
├── Main Camera
│   └── (Transform only, no Camera component)
│
├── CM vcam1 (Cinemachine Virtual Camera)
│   ├── Follow → Player (Transform)
│   ├── Lens → Orthographic Size: 5
│   ├── Body → Framing Transposer
│   ├── CameraZoomController (MonoBehaviour)
│   └── CameraLifetimeScope (MonoBehaviour, child scope)
│       ├── Parent → PlayerMovementLifetimeScope
│       └── Zoom Params → CameraZoomParams.asset
│
└── CameraBounds
    ├── CompositeCollider2D
    └── Collider2DGizmos
```

---

## Reserved Extensions

| Extension | Reservation Point |
|---|---|
| Decorator system (buffs/debuffs) | Replace `IMovementParamsProvider` with `DecoratedMovementParamsProvider` |
| Runtime strategy switching | Controller holds `IMovementStrategy` reference, swappable |
| Movement lock (stun/freeze/cutscene) | Reserve `IsMovementLocked` / `CanMove` |
| Turn speed | Not implemented — avoids Sprite design and animation complexity |

## AI-driven Movement (Strategy Swap)

The `IMovementStrategy` contract is designed to be swappable. NPC entities reuse the entire movement pipeline by replacing `InputMovementStrategy` with `AIMovementStrategy`:

```
InputMovementStrategy   → Player movement  (reads IInputProvider)
AIMovementStrategy      → NPC movement     (reads IAIController)
```

Both implement `IMovementStrategy` and are registered in their respective entity-scoped LifetimeScopes (`EntityLifetimeScope` for Player, `NpcEntityLifetimeScope` for NPC).

Supporting interfaces:
- `IAIController` — `DesiredDirection` (Vector2) + `WantsToMove` (bool)
- `IPositionProvider` — `Position` (Vector2), implemented by `TransformPositionProvider` adapter

See `PROJECT_REFERENCE.md` NPC System section for full details.
