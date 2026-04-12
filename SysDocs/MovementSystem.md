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
| `IMovementStrategy` | Provides movement direction for a given frame |
| `IMovementParamsProvider` | Provides movement parameters, reservable for decorator system |
| `IMovementStateProvider` | Exposes current `MovementState` for consumers |

### MovementState (readonly struct)

| Field | Type | Description |
|---|---|---|
| `Velocity` | `Vector2` | Intended velocity (Processor-calculated) |
| `Direction` | `Vector2` | Normalized direction, persists last valid direction when idle |
| `Speed` | `float` | Magnitude of Velocity |
| `IsMoving` | `bool` | Speed > MinMoveSpeed (based on intended velocity) |
| `ActualVelocity` | `Vector2` | Read back from Rigidbody2D (actual physics result, 1-frame delay) |
| `IsActuallyMoving` | `bool` | ActualVelocity.magnitude > MinMoveSpeed |

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
  direction = strategy.GetDirection()
  targetVelocity = direction * params.MaxSpeed
  maxDelta = (direction != zero) ? params.Acceleration : params.Deceleration
  currentVelocity = MoveTowards(currentVelocity, targetVelocity, maxDelta * deltaTime)
  clamp currentVelocity magnitude to params.MaxSpeed
  if direction == zero AND currentVelocity.magnitude < params.MinMoveSpeed:
    currentVelocity = zero
  if direction != zero:
    lastDirection = direction
  build MovementState with currentVelocity and actualVelocity
  return MovementState
```

---

## Input Layer

| Decision | Choice |
|---|---|
| Input system | New Input System |
| Asset management | `.inputactions` file + `[SerializeField] InputActionAsset` reference |
| PlayerInput component | Not used — `InputActionProvider` manages lifecycle |
| Composite binding | 2D Vector Composite (WASD / Arrow keys / Gamepad left stick) |
| Read timing | Update: cache input value; FixedUpdate: consume cached value |
| 8-dir normalization | `inputDir.normalized` in Strategy layer |
| Deadzone | In `IMovementStrategy`, threshold from `IInputConfig` (player preference) |

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
| `Infrastructure/Adapters/` | `UnSrp2d.Infrastructure.Adapters` |
| `Infrastructure/DI/` | `UnSrp2d.Infrastructure.DI` |

---

## Scene Setup

```
Scene Hierarchy:
├── InputSystem (GameObject)
│   ├── InputActionProvider (MonoBehaviour)
│   │   └── Action Asset → PlayerInputActions.inputactions
│   └── PlayerMovementLifetimeScope (MonoBehaviour)
│       └── Input Config → InputConfig.asset
│
└── Player (GameObject)
    ├── SpriteRenderer
    ├── Rigidbody2D (Dynamic, GravityScale=0)
    ├── BoxCollider2D
    ├── MovementController (MonoBehaviour)
    ├── SpriteDirectionHandler (MonoBehaviour)
    └── EntityLifetimeScope (MonoBehaviour)
        ├── Movement Params → MovementParams.asset
        └── Parent → PlayerMovementLifetimeScope (component reference)
```

---

## Reserved Extensions

| Extension | Reservation Point |
|---|---|
| Decorator system (buffs/debuffs) | Replace `IMovementParamsProvider` with `DecoratedMovementParamsProvider` |
| Runtime strategy switching | Controller holds `IMovementStrategy` reference, swappable |
| Movement lock (stun/freeze/cutscene) | Reserve `IsMovementLocked` / `CanMove` |
| Turn speed | Not implemented — avoids Sprite design and animation complexity |
