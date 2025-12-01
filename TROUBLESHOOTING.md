# Troubleshooting Guide - Chrono-Sniper Prototype

Common issues and their solutions when setting up the Chrono-Sniper prototype in Unity.

---

## Script Compilation Issues

### ❌ Error: "The type or namespace 'ChronoSniper' could not be found"
**Solution:**
- All scripts must be in `Assets/Scripts/` directory
- Unity needs to compile scripts before use
- Wait for compilation to complete (watch bottom-right corner)
- If issue persists: Assets > Reimport All

### ❌ Error: "Type 'GameManager' already exists"
**Solution:**
- Only one instance of each script should exist
- Check for duplicate files
- Delete duplicates and reimport

### ❌ Error: "linearVelocity is not a member of Rigidbody"
**Solution:**
- You're using Unity 2022 or earlier
- Replace `linearVelocity` with `velocity` in BulletController.cs
- Or upgrade to Unity 6000.x (recommended)

---

## Runtime Errors

### ❌ NullReferenceException: Object reference not set to instance
**Most Common Causes:**

1. **Missing Manager in Scene**
   - Check GameManagers GameObject has all components
   - GameManager, TimeController, BouncePointManager, ReplayManager, UIManager

2. **Missing References**
   - Check GameManager has bullet prefab assigned
   - Check GameManager has bullet spawn point assigned
   - Check PlayerController has camera transform assigned
   - Check PlayerController has bounce point prefab assigned
   - Check UIManager has all panel references assigned

3. **Missing UI Elements**
   - Create Canvas if it doesn't exist
   - Ensure UIManager is on Canvas GameObject
   - Create all required panels

**Debug Steps:**
```
1. Open Window > Analysis > Console
2. Double-click error to see source line
3. Add Debug.Log before error line to check null values
4. Verify references in Inspector
```

---

## Gameplay Issues

### ❌ Bullet doesn't move when fired
**Possible Causes:**
1. Rigidbody missing on bullet prefab
   - Add Rigidbody component
   - Set Use Gravity to OFF
   - Set Collision Detection to Continuous

2. Time is paused
   - Check Time.timeScale in Console: `Debug.Log(Time.timeScale)`
   - Should be 1.0 during execution
   - Verify TimeController exists

3. Bullet spawn point not set
   - Assign Player/Camera transform to GameManager.bulletSpawnPoint

### ❌ Can't place bounce points
**Possible Causes:**
1. No objects with "Ricochet" tag
   - Select ricochet surface
   - Tag dropdown > Add Tag > "Ricochet"
   - Assign tag to object

2. No colliders on ricochet surfaces
   - Add Box/Mesh/Sphere Collider to ricochet objects

3. Wrong LayerMask on PlayerController
   - Set Aim Layer Mask to include ricochet object layers
   - Default: "Everything"

4. Camera not assigned
   - Check PlayerController.cameraTransform is assigned
   - Should be the Camera child object of Player

### ❌ Trajectory line not visible
**Possible Causes:**
1. Line not being created
   - PlayerController creates it in Start()
   - Check Console for errors

2. Line color matches background
   - Change Trajectory Color to contrasting color (cyan recommended)

3. Line width too small
   - Increase Trajectory Line Width to 0.1 or more

### ❌ Bullet doesn't bounce at bounce points
**Possible Causes:**
1. Bounce points not set
   - BouncePointManager must be in scene
   - PlayerController passes points before firing

2. Bullet moving too fast
   - Reduce bullet speed in BulletController
   - Default 20 units/second should work

3. Bounce point trigger distance too small
   - In BulletController.CheckBouncePoints(), distance is 0.5
   - Increase if needed: `if (distanceToNext < 1.0f)`

### ❌ Enemies don't die when hit
**Possible Causes:**
1. Enemies missing "Enemy" tag
   - Select enemy GameObject
   - Set Tag to "Enemy"

2. Enemy component missing
   - Add Enemy component to enemy GameObjects

3. Collider missing or is trigger
   - Add Collider (not trigger) to enemies
   - Ensure Is Trigger is OFF

4. Bullet passing through
   - Bullet moving too fast
   - Enemy collider too small
   - Bullet Collision Detection should be "Continuous"

### ❌ Bullet bounces randomly instead of following path
**Possible Causes:**
1. Using default physics instead of planned path
   - This happens when hitting ricochet surfaces without bounce points
   - Ensure bounce points are placed on intended surfaces

2. Bounce point normals incorrect
   - Bounce points should face outward from surface
   - Check surface normal direction

### ❌ Game doesn't end (Win/Lose)
**Possible Causes:**
1. Enemy count mismatch
   - Check GameManager.totalEnemies matches actual enemy count
   - Or leave at default (auto-detects on Start)

2. Bullet never stops
   - Check maxLifetime in BulletController (default 30 seconds)
   - Bullet should hit non-ricochet surface or timeout

3. OnBulletStopped not called
   - Verify bullet has collider
   - Check collision detection is not disabled

---

## UI Issues

### ❌ UI not showing
**Possible Causes:**
1. Canvas missing
   - Create UI > Canvas

2. UIManager not on Canvas
   - Add UIManager component to Canvas GameObject

3. Panels not created
   - Create Panel GameObjects as children of Canvas
   - Name them: PlanningPanel, ExecutingPanel, etc.

4. Panels not assigned
   - Assign panels in UIManager Inspector
   - Check all references are set

5. Canvas settings incorrect
   - Render Mode: Screen Space - Overlay
   - Canvas Scaler: Scale With Screen Size (recommended)

### ❌ Text not visible
**Possible Causes:**
1. Text color matches background
   - Change text color to contrasting color

2. Text size too small
   - Increase font size to 24 or larger

3. Text outside panel bounds
   - Check RectTransform is within panel
   - Use anchors to position properly

### ❌ Restart button doesn't work
**Possible Causes:**
1. Button not assigned
   - Assign button to UIManager.restartButton

2. OnClick event not set
   - UIManager sets this in Start()
   - Check Console for errors

3. Scene not in Build Settings
   - File > Build Settings
   - Add current scene to Scenes in Build

---

## Replay System Issues

### ❌ Replay camera doesn't activate
**Possible Causes:**
1. ReplayCamera not assigned
   - Create duplicate of main camera
   - Assign to ReplayManager.replayCamera

2. ReplayCamera not disabled initially
   - Uncheck GameObject in hierarchy
   - Should be disabled before play

3. Recording not started
   - Verify BulletController calls ReplayManager.StartRecording
   - Check ReplayManager exists in scene

### ❌ Replay camera doesn't follow bullet
**Possible Causes:**
1. No frames recorded
   - Bullet must move for frames to record
   - Check recordedFrames.Count in Debug

2. Camera settings incorrect
   - Adjust Camera Distance (default 5)
   - Adjust Camera Height (default 2)
   - Increase Camera Smooth Speed for faster following

### ❌ Replay doesn't play after win
**Possible Causes:**
1. Win condition not triggered
   - Must kill all enemies
   - Check enemy count matches GameManager.totalEnemies

2. Replay delay too long
   - Reduce replayDelay in GameManager (default 1 second)

---

## Physics Issues

### ❌ Bullet falls down
**Solution:**
- Rigidbody Use Gravity should be OFF
- BulletController sets this in Awake()

### ❌ Bullet bounces erratically
**Possible Causes:**
1. Multiple colliders interfering
   - Ensure bullet has only one collider

2. Ricochet surface has no collider
   - Add collider to ricochet surfaces

3. Bounciness too high
   - Check Physics Material on surfaces
   - Set Bounciness to 0 (script handles bouncing)

### ❌ Enemies fly away on death
**Solution:**
- This is normal (ragdoll effect)
- Reduce Ragdoll Force in Enemy component
- Or set to 0 for no ragdoll

### ❌ Bullet goes through walls
**Solution:**
- Set Collision Detection to "Continuous" on bullet Rigidbody
- Ensure walls have colliders
- Increase wall collider thickness

---

## Performance Issues

### ❌ Low framerate
**Possible Causes:**
1. Too many objects in scene
   - Reduce complexity
   - Use occlusion culling

2. Expensive operations in Update()
   - Most scripts optimize this already
   - Check custom scripts if added

3. Quality settings too high
   - Edit > Project Settings > Quality
   - Reduce shadows/anti-aliasing for testing

### ❌ Replay lags
**Solution:**
- Reduce Replay Speed in ReplayManager
- Limit recording to shorter timeframes
- Reduce Camera Smooth Speed

---

## Editor Issues

### ❌ Can't find scripts in component dropdown
**Solution:**
- Wait for compilation (bottom-right corner)
- Scripts must be in Assets folder
- Restart Unity Editor if needed

### ❌ Scripts show "missing" in Inspector
**Solution:**
- Script was renamed or moved
- Recreate component assignment
- Or restore original script location

### ❌ Prefab references lost
**Solution:**
- Prefabs must be in Assets/Prefabs/ or similar
- Don't use GameObjects from scene as prefabs
- Create actual prefab: Drag GameObject to Project window

---

## Input Issues

### ❌ Mouse look doesn't work
**Possible Causes:**
1. Cursor not locked
   - PlayerController locks cursor in Start()
   - Press ESC to unlock (by design)
   - Click in game view to re-lock

2. Camera not assigned
   - Check PlayerController.cameraTransform

3. Mouse sensitivity too low
   - Increase Mouse Sensitivity in PlayerController

### ❌ Space key doesn't fire bullet
**Possible Causes:**
1. Already fired
   - Only one bullet per game
   - Restart to fire again

2. Wrong game state
   - Must be in Planning state
   - Check current state in GameManager

### ❌ Clicks don't place bounce points
**Possible Causes:**
1. Not hitting ricochet surface
   - Only "Ricochet" tagged objects work

2. Already at max bounce points
   - Default limit is 10
   - Remove with right-click first

---

## Build Issues

### ❌ Scene not included in build
**Solution:**
- File > Build Settings
- Add Open Scenes
- Or drag scene from Project to list

### ❌ Missing references in build
**Solution:**
- Ensure all prefabs are in Assets/
- Check all references in Managers
- Test in standalone build

---

## Debug Tips

### Enable Debug Logging
Add to any script to debug:
```csharp
void Update() {
    Debug.Log($"Current State: {GameManager.Instance.CurrentState}");
    Debug.Log($"Time Scale: {Time.timeScale}");
    Debug.Log($"Enemies Killed: {GameManager.Instance.EnemiesKilled}");
}
```

### Check Singleton Instances
In Console window:
```csharp
// Check if managers exist
Debug.Log(GameManager.Instance != null);
Debug.Log(TimeController.Instance != null);
Debug.Log(UIManager.Instance != null);
```

### Visualize Raycasts
In PlayerController, add to Update():
```csharp
Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
Debug.DrawRay(ray.origin, ray.direction * maxRayDistance, Color.red);
```

### Check Collisions
In BulletController, add to OnCollisionEnter:
```csharp
Debug.Log($"Bullet hit: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
```

---

## Still Having Issues?

1. **Check Console** (Window > General > Console)
   - Red errors must be fixed
   - Yellow warnings are usually okay

2. **Verify Setup** against SCENE_HIERARCHY.md
   - Double-check all references
   - Ensure all managers exist
   - Verify tags are set

3. **Start Fresh**
   - Create new scene
   - Follow QUICKSTART.md exactly
   - Test each step

4. **Test Components Individually**
   - Test TimeController alone
   - Test PlayerController camera rotation
   - Test bullet physics separately
   - Test enemy hit detection

---

## Quick Diagnostic Checklist

Run through this list if something isn't working:

- [ ] All scripts compiled without errors
- [ ] GameManagers GameObject exists with all 5 components
- [ ] Player GameObject with PlayerController and Camera child
- [ ] Bullet prefab exists with BulletController and Rigidbody
- [ ] BouncePoint prefab exists
- [ ] 5 Enemies with Enemy component and "Enemy" tag
- [ ] Ricochet surfaces with "Ricochet" tag and colliders
- [ ] Canvas with UIManager and all panels
- [ ] ReplayCamera exists and is assigned
- [ ] All references in Managers are assigned
- [ ] Tags (Enemy, Ricochet) exist in Tag Manager
- [ ] Scene is saved

If all checked and still issues, review error messages in Console for specific problems.
