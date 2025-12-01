# Scene Hierarchy Example

This document shows the recommended GameObject hierarchy for the Chrono-Sniper prototype.

```
Scene: ChronoSniper
├── GameManagers (Empty GameObject)
│   ├── GameManager (Component)
│   ├── TimeController (Component)
│   ├── BouncePointManager (Component)
│   ├── ReplayManager (Component)
│   └── UIManager (Component)
│
├── Player (Empty GameObject) [Position: 0, 1, 0]
│   ├── PlayerController (Component)
│   └── Camera (Camera GameObject) [Position: 0, 0.6, 0 local]
│       ├── Camera (Component)
│       └── TrajectoryLine (Created at runtime by PlayerController)
│
├── ReplayCamera (Camera GameObject)
│   └── Camera (Component) [Disabled initially]
│
├── Enemies (Empty GameObject - for organization)
│   ├── Enemy_1 (Capsule)
│   │   ├── Enemy (Component)
│   │   ├── Rigidbody (Is Kinematic: true)
│   │   ├── CapsuleCollider
│   │   └── Tag: "Enemy"
│   ├── Enemy_2 (Capsule)
│   ├── Enemy_3 (Capsule)
│   ├── Enemy_4 (Capsule)
│   └── Enemy_5 (Capsule)
│
├── Ricochet Objects (Empty GameObject - for organization)
│   ├── SteelBeam_1 (Cube) - Tag: "Ricochet"
│   ├── SteelBeam_2 (Cube) - Tag: "Ricochet"
│   ├── Pan_1 (Cylinder) - Tag: "Ricochet"
│   └── Mirror_1 (Plane) - Tag: "Ricochet"
│
├── Environment (Empty GameObject - for organization)
│   ├── Floor (Plane) [Position: 0, 0, 0]
│   ├── Wall_North (Cube)
│   ├── Wall_South (Cube)
│   ├── Wall_East (Cube)
│   └── Wall_West (Cube)
│
├── Lighting
│   ├── Directional Light
│   └── (Optional) Point Lights for atmosphere
│
└── Canvas (UI)
    ├── UIManager (Component)
    │
    ├── PlanningPanel (Panel)
    │   └── InstructionsText (Text)
    │       └── Text: "LEFT CLICK - Place Bounce Point\nRIGHT CLICK - Remove Last Point\nSPACE - Fire Bullet"
    │
    ├── ExecutingPanel (Panel) [Disabled initially]
    │   └── EnemiesKilledText (Text)
    │       └── Text: "Enemies: 0/5"
    │
    ├── ReplayPanel (Panel) [Disabled initially]
    │   └── ReplayText (Text)
    │       └── Text: "KILL CAM REPLAY"
    │
    ├── WinPanel (Panel) [Disabled initially]
    │   ├── WinText (Text)
    │   │   └── Text: "MISSION COMPLETE!"
    │   └── RestartButton (Button)
    │       └── Text: "Restart"
    │
    └── LosePanel (Panel) [Disabled initially]
        ├── LoseText (Text)
        │   └── Text: "MISSION FAILED"
        └── RestartButton (Button)
            └── Text: "Restart"
```

## Prefabs Folder Structure

```
Assets/Prefabs/
├── Bullet.prefab
│   ├── Sphere (0.2, 0.2, 0.2)
│   ├── BulletController (Component)
│   ├── Rigidbody (Use Gravity: OFF, Continuous)
│   ├── SphereCollider
│   ├── TrailRenderer (Optional)
│   └── Tag: "Bullet"
│
└── BouncePoint.prefab
    ├── Sphere (0.3, 0.3, 0.3)
    ├── BouncePoint (Component)
    ├── Material (Emissive Cyan)
    └── MeshRenderer
```

## Component Reference Assignments

### GameManager Component
- **Total Enemies**: 5
- **Replay Delay**: 1
- **Bullet Prefab**: Assets/Prefabs/Bullet.prefab
- **Bullet Spawn Point**: Player/Camera Transform

### PlayerController Component
- **Mouse Sensitivity**: 2
- **Max Vertical Angle**: 80
- **Camera Transform**: Player/Camera
- **Max Ray Distance**: 100
- **Aim Layer Mask**: Everything (or specific layers)
- **Bounce Point Prefab**: Assets/Prefabs/BouncePoint.prefab
- **Max Bounce Points**: 10
- **Trajectory Color**: Cyan (R:0, G:255, B:255)
- **Trajectory Line Width**: 0.05

### ReplayManager Component
- **Replay Speed**: 1
- **Replay Camera**: ReplayCamera
- **Camera Distance**: 5
- **Camera Height**: 2
- **Camera Smooth Speed**: 5

### UIManager Component (on Canvas)
- **Planning Panel**: Canvas/PlanningPanel
- **Executing Panel**: Canvas/ExecutingPanel
- **Replay Panel**: Canvas/ReplayPanel
- **Win Panel**: Canvas/WinPanel
- **Lose Panel**: Canvas/LosePanel
- **Instructions Text**: Canvas/PlanningPanel/InstructionsText
- **Enemies Killed Text**: Canvas/ExecutingPanel/EnemiesKilledText
- **Restart Button**: Canvas/WinPanel/RestartButton (or LosePanel/RestartButton)

### BulletController Component (in Prefab)
- **Speed**: 20
- **Max Lifetime**: 30
- **Destroy Delay**: 2
- **Trail Renderer**: (optional) TrailRenderer component

### Enemy Component (on each enemy)
- **Alive Color**: Red (R:255, G:0, B:0)
- **Dead Color**: Gray (R:128, G:128, B:128)
- **Death Effect Prefab**: (optional)
- **Ragdoll Force**: 5

### BouncePoint Component (in Prefab)
- **Normal Color**: Cyan (R:0, G:255, B:255)
- **Bounce Color**: Yellow (R:255, G:255, B:0)
- **Bounce Effect Duration**: 0.3

## Material Setup

### Enemy Material
- **Name**: EnemyMaterial
- **Color**: Red (R:255, G:0, B:0)
- **Metallic**: 0
- **Smoothness**: 0.5

### Ricochet Surface Material
- **Name**: RicochetMaterial
- **Color**: Silver/Steel (R:192, G:192, B:192)
- **Metallic**: 0.8
- **Smoothness**: 0.9

### Bounce Point Material
- **Name**: BouncePointMaterial
- **Color**: Cyan (R:0, G:255, B:255)
- **Emission**: Enabled
- **Emission Color**: Cyan
- **Emission Intensity**: 2

### Bullet Material
- **Name**: BulletMaterial
- **Color**: Yellow (R:255, G:255, B:0)
- **Emission**: Enabled (optional)
- **Emission Color**: Yellow
- **Emission Intensity**: 1

## Tag Configuration

Ensure these tags exist (Edit > Project Settings > Tags and Layers):
1. **Enemy** - For all enemy GameObjects
2. **Ricochet** - For surfaces that bullets can bounce off
3. **Bullet** - For the bullet prefab
4. **MainCamera** - For the player's camera (default Unity tag)

## Layer Configuration (Optional but Recommended)

Create these layers for better physics control:
1. **Enemy** (Layer 6)
2. **Ricochet** (Layer 7)
3. **Bullet** (Layer 8)
4. **Player** (Layer 9)

### Layer Collision Matrix
(Edit > Project Settings > Physics)
- Bullet should collide with: Enemy, Ricochet, Default
- Enemy should collide with: Bullet, Default
- Ricochet should collide with: Bullet, Default
- Player should collide with: Default only

## Testing Checklist

After setup, verify:
- [ ] All GameManagers components are on single GameObject
- [ ] Player camera is child of Player and assigned correctly
- [ ] Bullet prefab has Rigidbody with Use Gravity OFF
- [ ] All 5 enemies have Enemy component and "Enemy" tag
- [ ] Ricochet objects have "Ricochet" tag and colliders
- [ ] UI Canvas has all panels as children
- [ ] UIManager has all panel references assigned
- [ ] GameManager has bullet prefab and spawn point assigned
- [ ] PlayerController has camera and bounce point prefab assigned
- [ ] ReplayCamera exists and is assigned to ReplayManager
- [ ] Tags (Enemy, Ricochet, Bullet) exist in Tag Manager

---

**When everything is connected, press Play and test the prototype!**
