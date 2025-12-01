# Chrono-Sniper Game Prototype

## Overview
Chrono-Sniper is a turn-based action/puzzle game where players use a single bullet that can ricochet off objects to eliminate all enemies in a room. The game features time-stop mechanics, strategic bounce point placement, and a cinematic kill cam replay system.

**Genre:** Turn-Based Action / Puzzle  
**Core Mechanic:** One bullet, time manipulation, and ricochet physics  
**Win Condition:** Eliminate all 5 enemies with a single bullet shot

---

## Scene Setup Instructions

### 1. Create a New Scene or Use Existing Scene
- Open Unity Editor
- Create a new scene or open `Assets/Scenes/SampleScene.unity`

### 2. Add Core Game Managers
Create an empty GameObject called "GameManagers" and add the following components:
- **GameManager** - Core game state controller
- **TimeController** - Handles time pause/play mechanics
- **BouncePointManager** - Manages bounce point collection
- **ReplayManager** - Records and plays back kill cam
- **UIManager** - Controls UI panel visibility

#### GameManager Configuration:
- Assign `totalEnemies` = 5 (or match your enemy count)
- Create a bullet prefab (see step 4) and assign it to `bulletPrefab`
- Create an empty GameObject for bullet spawn point and assign to `bulletSpawnPoint`

### 3. Setup Player
Create an empty GameObject called "Player":
- Position at (0, 1, 0)
- Add **PlayerController** component
- Create a child GameObject called "Camera"
  - Add Camera component
  - Position at (0, 0.6, 0) relative to player
  - Assign this camera to PlayerController's `cameraTransform`
- Set `maxRayDistance` = 100
- Set `aimLayerMask` to include layers you want to interact with

### 4. Create Bullet Prefab
Create a new Sphere GameObject:
- Scale: (0.2, 0.2, 0.2)
- Add **BulletController** component
- Add **Rigidbody** (should auto-add)
  - Use Gravity: OFF
  - Collision Detection: Continuous
- Add **SphereCollider** (should auto-add)
  - Radius: 0.5
- Optionally add TrailRenderer for visual effect
- Tag: "Bullet"
- Save as prefab in Assets/Prefabs/

### 5. Create Bounce Point Prefab
Create a new Sphere GameObject:
- Scale: (0.3, 0.3, 0.3)
- Add **BouncePoint** component
- Add a material with emissive color (cyan recommended)
- Save as prefab in Assets/Prefabs/
- Assign this prefab to PlayerController's `bouncePointPrefab`

### 6. Create Enemies
For each enemy (create 5 total):
- Create a Capsule GameObject
- Position at desired location
- Scale: (1, 1, 1)
- Add **Enemy** component
- Add **Rigidbody**
  - Is Kinematic: TRUE (initially)
  - Use Gravity: ON
- Add **CapsuleCollider**
- Tag: "Enemy"
- Add a red material to visualize

### 7. Create Ricochet Surfaces
Create objects that bullets can bounce off:
- Create Cubes, Planes, or custom meshes
- Add Collider component
- Tag: "Ricochet"
- Add a metallic/reflective material (e.g., steel beams, pans)
- Position strategically in the scene

Examples:
- **Steel Beam:** Rotate a thin cube at an angle
- **Pan:** Use a cylinder with metallic material
- **Mirror:** Use a plane with reflective material

### 8. Create Environment
- Add floor (Plane at Y=0, tag: "Ground")
- Add walls (Cubes, no special tag needed)
- Add lighting (Directional Light)

### 9. Setup UI Canvas
Create UI > Canvas:
- Render Mode: Screen Space - Overlay
- Add **UIManager** component to Canvas

Create child panels (Panel UI elements):

**Planning Panel:**
- TextMeshPro: "LEFT CLICK - Place Bounce Point\nRIGHT CLICK - Remove Last Point\nSPACE - Fire Bullet"
- Assign to UIManager's `planningPanel`

**Executing Panel:**
- TextMeshPro for enemy counter
- Assign text to UIManager's `enemiesKilledText`
- Assign panel to UIManager's `executingPanel`

**Replay Panel:**
- TextMeshPro: "KILL CAM REPLAY"
- Assign to UIManager's `replayPanel`

**Win Panel:**
- TextMeshPro: "MISSION COMPLETE!"
- Button: "Restart"
- Assign button to UIManager's `restartButton`
- Assign panel to UIManager's `winPanel`

**Lose Panel:**
- TextMeshPro: "MISSION FAILED"
- Button: "Restart"
- Assign panel to UIManager's `losePanel`

### 10. Create Replay Camera
- Duplicate the main camera
- Rename to "ReplayCamera"
- Disable it initially
- Assign to ReplayManager's `replayCamera`
- Configure distance and height settings in ReplayManager

### 11. Configure Tags
Ensure the following tags exist (Edit > Project Settings > Tags):
- Enemy
- Ricochet
- Bullet
- Ground (optional)

### 12. Configure Layers (Optional but Recommended)
Create layers for better physics control:
- Enemy
- Ricochet
- Bullet
- Ground

---

## How to Play

1. **Planning Phase** (Time is Paused):
   - Look around with mouse
   - LEFT CLICK on "Ricochet" tagged surfaces to place bounce points
   - RIGHT CLICK to remove the last placed bounce point
   - A cyan trajectory line shows your planned path
   - Press SPACE to fire the bullet

2. **Execution Phase** (Time is Playing):
   - Watch the bullet travel through your planned path
   - Bullet will ricochet at each bounce point
   - Enemies die when hit by the bullet
   - Bullet continues until it hits a non-ricochet surface

3. **Outcome**:
   - **Win:** All enemies eliminated → Kill Cam replay plays
   - **Lose:** Bullet stops before all enemies are killed

4. **Kill Cam Replay**:
   - Cinematic camera follows bullet trajectory
   - Shows the entire shot in real-time
   - Perfect for creating trailers and showcasing solutions

5. **Restart**:
   - Click restart button to try again

---

## Architecture Overview

### Core Systems

#### 1. Game State Management
**GameManager.cs** - Central controller using singleton pattern
- Manages game states: Planning, Executing, Replay, Win, Lose
- Tracks enemy kills and win/lose conditions
- Coordinates between all systems
- Handles scene reloading

#### 2. Time Control
**TimeController.cs** - Controls Unity's time scale
- Pauses time during planning phase (Time.timeScale = 0)
- Resumes time during execution (Time.timeScale = 1)
- Singleton pattern for global access

#### 3. Player Input & Control
**PlayerController.cs** - Handles player interaction
- First-person camera rotation
- Bounce point placement via raycasting
- Trajectory visualization with LineRenderer
- Bullet firing trigger

### Bullet System

#### BulletController.cs
- Physics-based movement using Rigidbody
- Collision detection for enemies and surfaces
- Automatic navigation to bounce points
- Notifies GameManager when stopped

#### BouncePoint.cs
- Stores position and surface normal
- Visual feedback when bullet bounces
- Used for bullet trajectory calculation

#### BouncePointManager.cs
- Singleton managing active bounce points
- Provides bounce point data to bullet
- Handles cleanup on reset

### Enemy System

#### Enemy.cs
- Simple health system (alive/dead state)
- Death notification to GameManager
- Visual feedback (color change)
- Optional ragdoll physics on death

### Replay System

#### ReplayManager.cs
- Records bullet position/rotation each frame during execution
- Stores data as BulletFrame list
- Plays back recording with cinematic camera
- Smooth camera following with configurable distance/height

### UI System

#### UIManager.cs
- Singleton managing all UI panels
- State-based panel visibility
- Updates enemy kill counter
- Handles restart functionality

---

## Code Architecture Principles

### Modularity
- Each system is independent and can be modified without affecting others
- Singleton pattern used for managers (ensures single instance and global access)
- Clear separation of concerns (input, physics, UI, state management)

### Extensibility
- Easy to add new game states to `GameState` enum
- Bounce point system can be extended with different types
- Enemy class can be inherited for different enemy types
- Replay system can be enhanced with slow-motion, multiple cameras, etc.

### Event-Driven
- Systems communicate through method calls on singletons
- GameManager acts as central event hub
- Minimal coupling between systems

---

## Extending the Game

### Adding New Features

**Different Bullet Types:**
- Inherit from `BulletController`
- Override `OnCollisionEnter` for special behaviors
- Add properties for speed, bounce count, penetration, etc.

**Enemy Variety:**
- Inherit from `Enemy` class
- Add movement patterns, shields, or special death effects
- Implement different point values

**Power-ups:**
- Create new GameObjects with trigger colliders
- Add components that modify bullet or game state
- Examples: Extra bounces, slow-motion, explosive bullets

**Multiple Levels:**
- Use SceneManager to load different scenes
- Store level progression in PlayerPrefs or save system
- Create level selection UI

**Advanced Replay:**
- Add multiple camera angles
- Implement slow-motion toggle
- Add rewind/fast-forward controls
- Export replay as video

---

## Performance Notes

- Bullet recording stores position/rotation per frame - for very long shots, consider frame skipping
- Disable unnecessary physics interactions through layer collision matrix
- Use object pooling for frequently spawned objects (death effects, bounce markers)

---

## Troubleshooting

**Bullet doesn't bounce:**
- Ensure surfaces have "Ricochet" tag
- Check bounce point placement is on ricochet surfaces
- Verify bounce points are positioned correctly

**Enemies not dying:**
- Verify enemies have "Enemy" tag
- Check Enemy component is attached
- Ensure colliders are not set to trigger

**Camera not moving:**
- Check camera is assigned to PlayerController
- Verify camera is child of player object
- Check mouse sensitivity settings

**UI not showing:**
- Verify Canvas has UIManager component
- Check panel assignments in UIManager
- Ensure panels are children of Canvas

**Time not pausing:**
- Check TimeController exists in scene
- Verify GameManager references TimeController
- Check Time.timeScale value in Play mode

---

## Credits

**Game Design:** Chrono-Sniper Prototype  
**Engine:** Unity 6000.2.9f1  
**Architecture:** Modular, singleton-based design  

---

## Future Improvements

- Add sound effects (bullet fire, ricochet, enemy death)
- Implement particle effects for impacts
- Add scoring system based on efficiency
- Create tutorial level
- Implement leaderboards
- Add more visual polish (materials, lighting, post-processing)
- Mobile touch controls support
- Save/load system for custom levels
