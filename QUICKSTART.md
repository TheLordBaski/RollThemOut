# Quick Setup Guide - Chrono-Sniper Prototype

This is a quick reference guide for setting up the Chrono-Sniper prototype scene.

## Minimum Viable Setup (5 Minutes)

### Step 1: Create Core Managers
1. Create empty GameObject: "GameManagers"
2. Add components:
   - GameManager
   - TimeController
   - BouncePointManager
   - ReplayManager
   - UIManager

### Step 2: Create Player
1. Create empty GameObject: "Player" at (0, 1, 0)
2. Add PlayerController component
3. Create child "Camera" GameObject
   - Add Camera component
   - Position: (0, 0.6, 0) local
   - Tag: MainCamera
4. Assign camera to PlayerController.cameraTransform

### Step 3: Create Prefabs

**Bullet Prefab:**
- Create Sphere, scale (0.2, 0.2, 0.2)
- Add BulletController
- Tag: "Bullet"
- Save to Assets/Prefabs/Bullet.prefab

**BouncePoint Prefab:**
- Create Sphere, scale (0.3, 0.3, 0.3)
- Add BouncePoint
- Emissive cyan material
- Save to Assets/Prefabs/BouncePoint.prefab

### Step 4: Create Enemies (5x)
- Create Capsule
- Add Enemy component
- Add Rigidbody (Is Kinematic = true)
- Tag: "Enemy"
- Red material
- Position at different locations

### Step 5: Create Ricochet Objects
- Create Cubes/Planes
- Tag: "Ricochet"
- Add Collider
- Metallic material
- Position strategically

### Step 6: Create Floor & Walls
- Floor: Plane at Y=0
- Walls: Cubes around perimeter

### Step 7: Wire Up References
In GameManager:
- Bullet Prefab → Bullet.prefab
- Bullet Spawn Point → Player/Camera transform
- Total Enemies → 5

In PlayerController:
- Camera Transform → Player/Camera
- Bounce Point Prefab → BouncePoint.prefab
- Aim Layer Mask → Everything (or specific layers)

### Step 8: Setup UI (Optional but Recommended)
1. Create Canvas
2. Add UIManager to Canvas
3. Create child Panels for each state
4. Assign panels to UIManager

### Step 9: Create Replay Camera
- Duplicate Main Camera
- Rename "ReplayCamera"
- Disable initially
- Assign to ReplayManager.replayCamera

### Step 10: Tags & Layers
Required Tags:
- Enemy
- Ricochet
- Bullet

### Step 11: Press Play!
- Use mouse to look around
- Left click on "Ricochet" surfaces to place bounce points
- Press SPACE to fire
- Watch the magic happen!

## Common Issues & Fixes

**"NullReferenceException on Start"**
→ Check all singleton managers exist in scene

**"Bullet doesn't move"**
→ Verify Rigidbody on bullet prefab, Use Gravity = OFF

**"Can't place bounce points"**
→ Surfaces need "Ricochet" tag and colliders

**"Time doesn't pause"**
→ Verify TimeController exists in scene

**"UI not appearing"**
→ Create Canvas with UIManager component

## Testing the Prototype

1. **Test Bounce Points:**
   - Place 3-4 bounce points
   - Verify cyan line shows trajectory
   - Right-click removes last point

2. **Test Bullet:**
   - Fire bullet with SPACE
   - Should follow bounce points
   - Should kill enemies on contact

3. **Test Win Condition:**
   - Kill all 5 enemies
   - Kill cam replay should play
   - Win screen appears

4. **Test Lose Condition:**
   - Miss some enemies
   - Bullet hits non-ricochet surface
   - Lose screen appears

## Next Steps

See full README.md for:
- Detailed architecture explanation
- Extension possibilities
- Advanced features
- Performance optimization

---

**Ready to build your first level!** 🎯
