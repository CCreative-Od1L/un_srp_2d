# Camera Follow System — Design Specification

> Feature: 2D camera follow with smooth damping and scene bounds  
> Scene: `PlayerMovement.unity`  
> Status: Implemented  
> Implementation: Cinemachine (Unity Official Package)

---

## Overview

Camera system that smoothly follows the player with configurable damping and respects scene boundaries using Unity's Cinemachine package.

---

## Requirements

| # | Requirement | Solution |
|---|---|---|
| 1 | Smooth follow with velocity change on start/stop | Cinemachine Virtual Camera Damping |
| 2 | Camera constrained within scene bounds | Cinemachine Confiner 2D |
| 3 | Keep player centered on screen | Virtual Camera Follow target |

---

## Package

| Package | Version | Purpose |
|---|---|---|
| `com.unity.cinemachine` | 2.10.3 | Official camera system for Unity |

---

## Scene Setup

### Hierarchy Structure

```
Scene Root
├── InputSystem
│   ├── InputActionProvider
│   └── PlayerMovementLifetimeScope
│
├── Player
│   ├── SpriteRenderer
│   ├── Rigidbody2D (Interpolate: Interpolate)
│   ├── BoxCollider2D
│   ├── MovementController
│   ├── SpriteDirectionHandler
│   └── EntityLifetimeScope
│
├── Main Camera
│   └── (Remove Camera component, keep only Transform)
│
├── CM vcam1 (Cinemachine Virtual Camera)
│   ├── Follow → Player (Transform)
│   ├── Lens → Orthographic Size: 5
│   └── Body → Framing Transposer
│
└── CameraBounds (Empty GameObject)
    ├── CompositeCollider2D (defines scene boundary)
    └── CameraBoundsDebug (MonoBehaviour, draws Gizmos)
```

---

## Component Configuration

### 1. Player Rigidbody2D

| Property | Value | Description |
|---|---|---|
| **Interpolate** | `Interpolate` | **Critical** - Prevents camera jitter by smoothing between physics frames |

### 2. Cinemachine Virtual Camera

| Property | Value | Description |
|---|---|---|
| **Follow** | Player Transform | Target to follow |
| **Lens - Orthographic Size** | 5 | Camera zoom level |
| **Body** | Framing Transposer | 2D follow behavior |
| **Body - Tracked Object Offset** | (0, 0, 0) | Offset from target |
| **Body - Lookahead Time** | 0 | Predict target movement |
| **Body - Lookahead Smoothing** | 0 | Smoothing for lookahead |
| **Body - X/Y Damping** | 1~3 | Smooth follow strength (higher = smoother) |
| **Body - Screen X/Y** | 0.5, 0.5 | Keep target centered |
| **Body - Dead Zone Width/Height** | 0 | No dead zone (always follow) |

### 3. Cinemachine Confiner 2D (Extension)

Add as extension to Virtual Camera:

| Property | Value | Description |
|---|---|---|
| **Bounding Shape 2D** | Collider2D | Scene boundary collider |
| **Damping** | 0 | Damping at corners |
| **Slowing Distance** | 0 | Slow down near edges |

### 4. Camera Bounds Collider

| Property | Value |
|---|---|
| **Is Trigger** | true |
| **Geometry Type** | Polygons (if CompositeCollider2D) |
| **Body Type** | Static |

**Important:** Collider size must be larger than camera viewport:
```
最小边界宽度 = Orthographic Size × 2 × 屏幕宽高比
最小边界高度 = Orthographic Size × 2

例：Orthographic Size = 5, 宽高比 16:9
最小边界 ≈ 17.8 × 10
```

---

## Damping Configuration Guide

Damping controls smoothness of camera movement:

| Damping Value | Behavior |
|---|---|
| 0 | Instant follow (no smoothing) |
| 0.5 ~ 1 | Quick response, slight lag |
| 1 ~ 3 | Smooth, natural feel (recommended) |
| 5+ | Very smooth, noticeable delay |

**For start/stop smoothness:**
- Higher damping = smoother acceleration/deceleration
- Use same value for X and Y for consistent feel
- Z damping irrelevant for 2D orthographic

---

## Troubleshooting

### Camera Jitter during Movement

**Cause:** Physics updates (FixedUpdate) and rendering updates (Update) run at different frequencies, causing visual jitter.

**Solution:** Set `Rigidbody2D.Interpolate` to `Interpolate`

```
Player → Rigidbody2D → Interpolate: Interpolate
```

This makes Rigidbody2D smooth between physics frames for rendering.

---

## How It Works

```
Player Movement
      ↓
Virtual Camera detects target position change
      ↓
Damping applies smooth interpolation
      ↓
Confiner 2D clamps camera to bounds
      ↓
Final camera position
```

**Edge Behavior:**
- When player approaches boundary, camera stops moving
- Player can continue moving toward edge (no longer centered)
- Camera resumes following when player moves away from edge

---

## Debug Visualization

Use `Collider2DGizmos` component to visualize any Collider2D bounds in Scene view and during runtime.

### Features

| Mode | Description |
|---|---|
| **Scene View** | Draws bounds when object selected (`OnDrawGizmosSelected`) |
| **Runtime** | Optional draw during play mode for debugging |

### Usage

1. Add `Collider2DGizmos` component to any GameObject with Collider2D
2. Assign the Collider2D to the `Collider` field
3. Select the object to see bounds in Scene view

---

## Files

| File | Purpose |
|---|---|
| `Assets/Scripts/Shared/Debug/Collider2DGizmos.cs` | Generic Collider2D debug visualization |

---

## Future Extensions

| Feature | Cinemachine Support |
|---|---|
| Camera shake | ✅ Cinemachine Impulse Listener |
| Multiple camera targets | ✅ Target Group |
| Camera transitions/blending | ✅ Cinemachine Brain blending |
| Pixel-perfect camera | ✅ Integration with Pixel Perfect package |
