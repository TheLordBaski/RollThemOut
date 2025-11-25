# Troubleshooting & FAQ - Roll Them Out

## 🔧 Common Issues and Solutions

### Player Issues

#### ❌ Player doesn't move
**Symptoms**: WASD/Arrow keys don't make the player move

**Solutions**:
1. ✅ Check Rigidbody is attached to player
2. ✅ Verify Rigidbody constraints aren't freezing position
3. ✅ Ensure Move Force is set (try 10 or higher)
4. ✅ Check Mass isn't too high (try base mass of 1)
5. ✅ Verify no other scripts are overriding velocity

**Debug Steps**:
```csharp
// Add PhysicsDebugVisualizer to see velocity
// Check Unity Console for input values
Debug.Log($"Move Input: {moveInput}");
```

---

#### ❌ Player spins uncontrollably
**Symptoms**: Player rotates wildly even with no input

**Solutions**:
1. ✅ Increase Angular Drag on Rigidbody (try 3-5)
2. ✅ Check for multiple conflicting forces
3. ✅ Verify items aren't creating unbalanced torque
4. ✅ Reduce torque multiplier in PlayerController

**Fix**:
```csharp
// In PlayerController, reduce this value:
rb.AddTorque(torqueDirection * moveForce * 0.5f); // Try 0.2 instead
```

---

#### ❌ Player moves too slow with attached items
**Symptoms**: Player becomes immobile after collecting items

**Solutions**:
1. ✅ Reduce item mass values (try 0.5-1.0)
2. ✅ Increase Move Force (try 15-20)
3. ✅ Adjust speed calculation in LimitSpeed()
4. ✅ Check Max Speed isn't too low

**Balancing**:
```
Light Item:  0.3 - 0.5 mass
Medium Item: 0.5 - 1.0 mass
Heavy Item:  1.0 - 2.0 mass
```

---

### Item Issues

#### ❌ Items don't attract to player
**Symptoms**: Items just sit on the ground, no magnetic pull

**Solutions**:
1. ✅ Verify Item Layer is created and assigned
2. ✅ Check Magnetic Range (increase to 5-10 for testing)
3. ✅ Ensure items have Rigidbody component
4. ✅ Verify itemLayer mask is set in PlayerController
5. ✅ Check items aren't kinematic

**Debug**:
```csharp
// In PlayerController.DetectNearbyItems()
Debug.Log($"Found {nearbyColliders.Length} items");
```

---

#### ❌ Items fall through ground
**Symptoms**: Items spawn and immediately fall through the floor

**Solutions**:
1. ✅ Add Collider to ground plane
2. ✅ Set Collision Detection to Continuous on item Rigidbody
3. ✅ Ensure ground layer collides with Item layer
4. ✅ Increase item spawn height

**Fix**:
```csharp
// In ItemSpawner, increase spawn height:
Vector3 spawnPosition = spawnCenter + new Vector3(randomCircle.x, 2f, randomCircle.y); // Was 1f
```

---

#### ❌ Items don't attach on collision
**Symptoms**: Items bounce off player instead of attaching

**Solutions**:
1. ✅ Verify items have AttachableItem-derived component
2. ✅ Check OnCollisionEnter is being called
3. ✅ Ensure collision detection isn't disabled
4. ✅ Verify layer collision matrix allows collision

**Debug**:
```csharp
// In PlayerController.OnCollisionEnter()
Debug.Log($"Collision with: {collision.gameObject.name}");
```

---

#### ❌ Too many items lag the game
**Symptoms**: FPS drops when many items are spawned

**Solutions**:
1. ✅ Reduce max items in ItemSpawner (try 15-20)
2. ✅ Increase spawn interval
3. ✅ Implement object pooling
4. ✅ Use simpler colliders (sphere/capsule vs mesh)

**Performance Settings**:
```csharp
[SerializeField] private int maxItemsInWorld = 15; // Reduce this
[SerializeField] private float spawnInterval = 5f;  // Increase this
```

---

### Weapon Issues

#### ❌ Weapons don't fire
**Symptoms**: Attached weapons don't shoot at enemies

**Solutions**:
1. ✅ Create "Enemy" layer and assign to enemies
2. ✅ Set Enemy Layer in WeaponItem inspector
3. ✅ Ensure enemies have Enemy tag
4. ✅ Check Detection Radius is large enough
5. ✅ Verify Auto Fire is enabled

**Required Setup**:
```
1. Tag enemy as "Enemy"
2. Set enemy layer to "Enemy"
3. Assign Enemy layer to weapon's enemyLayer field
```

---

#### ❌ Weapon recoil too strong
**Symptoms**: Player gets launched across map when weapons fire

**Solutions**:
1. ✅ Reduce Recoil Force in WeaponItem (try 2-5)
2. ✅ Increase player mass
3. ✅ Reduce item mass
4. ✅ Lower fire rate

**Balancing**:
```
Cannon:    Recoil 5-10, Fire Rate 0.5
MachineGun: Recoil 1-2,  Fire Rate 5
Rocket:    Recoil 15-20, Fire Rate 0.3
```

---

### Camera Issues

#### ❌ Camera doesn't follow player
**Symptoms**: Camera stays in one place

**Solutions**:
1. ✅ Assign player to Target field in CameraController
2. ✅ Verify CameraController script is enabled
3. ✅ Check camera isn't parented to anything
4. ✅ Ensure Smooth Speed isn't 0

---

#### ❌ Camera too close/far
**Symptoms**: Can't see player properly

**Solutions**:
1. ✅ Adjust Offset in CameraController
2. ✅ Disable Dynamic Zoom for fixed distance
3. ✅ Adjust Min/Max Distance values

**Good Offsets**:
```
Top-Down:      (0, 15, -15)
Isometric:     (10, 10, -10)
Side View:     (15, 5, 0)
```

---

### Enemy Issues

#### ❌ Enemies don't spawn
**Symptoms**: No enemies appear in game

**Solutions**:
1. ✅ Check enemy prefabs are assigned in EnemySpawner
2. ✅ Verify Player reference is set
3. ✅ Ensure spawn interval isn't too long
4. ✅ Check max enemies limit

---

#### ❌ Enemies don't chase player
**Symptoms**: Enemies just stand still

**Solutions**:
1. ✅ Verify player reference in Enemy script
2. ✅ Check Detection Range is large enough
3. ✅ Ensure Move Speed isn't 0
4. ✅ Verify Rigidbody constraints allow movement

---

### Physics Issues

#### ❌ Everything explodes on contact
**Symptoms**: Objects fly apart when touching

**Solutions**:
1. ✅ Reduce collision force in Physics settings
2. ✅ Use Fixed Joints instead of Configurable
3. ✅ Increase Joint Break Force
4. ✅ Lower item masses

**Physics Settings**:
```
Edit → Project Settings → Physics
- Default Contact Offset: 0.01
- Default Solver Iterations: 10
- Bounce Threshold: 2
```

---

#### ❌ Items vibrate/jitter when attached
**Symptoms**: Attached items shake or vibrate

**Solutions**:
1. ✅ Set item Rigidbody to kinematic when attached
2. ✅ Increase Fixed Timestep (Edit → Project Settings → Time)
3. ✅ Use Interpolate on Rigidbodies
4. ✅ Increase solver iterations

**Fix**:
```csharp
// In AttachableItem.OnAttached()
rb.isKinematic = true;  // Make sure this is set
rb.interpolation = RigidbodyInterpolation.Interpolate;
```

---

## ❓ Frequently Asked Questions

### Game Design

**Q: How do I balance the game?**
A: Start with these ratios:
- Light items: 0.5 mass, high spawn rate
- Heavy items: 2.0 mass, low spawn rate
- Player base mass: 1.0
- Max speed: 5.0 m/s

**Q: What's a good item limit?**
A: Start with 20-30 attached items max for playability. Beyond that, physics simulation can become chaotic (which might be fun!).

**Q: Should weapons auto-fire or manual?**
A: Auto-fire is recommended to keep focus on movement and positioning, which is the core mechanic. Manual fire works too but changes the gameplay significantly.

---

### Technical

**Q: Why use Fixed Joints instead of parenting?**
A: Fixed Joints provide realistic physics interactions while maintaining the connection. Simple parenting would ignore physics forces like recoil.

**Q: How is center of mass calculated?**
A: Weighted average of all attached items:
```
COM = Σ(item.position × item.mass) / Σ(total mass)
```

**Q: Can I use 2D instead of 3D?**
A: Yes! Convert to Rigidbody2D, use 2D colliders, and adjust movement to XY plane instead of XZ.

---

### Customization

**Q: How do I add a new weapon type?**
A:
1. Open `WeaponItem.cs`
2. Add to `WeaponType` enum
3. Create new `Fire[WeaponName]()` method
4. Add case in `Fire()` switch statement

**Q: How do I add visual effects?**
A: 
1. Create particle system prefab
2. Add field in weapon/armor script
3. Instantiate in `Fire()` or `ApplyAttachedEffect()`

**Q: Can players detach items?**
A: Not by default, but you can add:
```csharp
// In PlayerController.Update()
if (Input.GetKeyDown(KeyCode.Space) && attachedItems.Count > 0)
{
    DetachItem(attachedItems[attachedItems.Count - 1]);
}
```

---

### Performance

**Q: Game runs slow with many items**
A:
1. Reduce max items spawned
2. Use object pooling
3. Simplify collision shapes
4. Lower physics update rate
5. Implement LOD system

**Q: How many items can the game handle?**
A: Depends on hardware, but typically:
- 30-50 attached items: Good performance
- 50-100 items: Moderate performance
- 100+ items: May need optimization

---

## 🐛 Debug Checklist

Before asking for help, verify:

- [ ] All required components are attached
- [ ] Layers are created and assigned correctly
- [ ] Tags are created and assigned correctly
- [ ] Prefabs have all required components
- [ ] Collision matrix allows proper collisions
- [ ] Inspector fields are set (not null)
- [ ] Unity Console shows no errors
- [ ] Scripts are properly saved
- [ ] Scene is saved

---

## 🔍 Debugging Tools

### 1. PhysicsDebugVisualizer
Add to player to see:
- Real-time velocity
- Center of mass
- Mass values
- Item count

### 2. Gizmos
All scripts draw debug info:
- Enable Gizmos in Scene view
- Select objects to see ranges
- Use for tuning values

### 3. Console Logging
Enable debug logs in scripts:
```csharp
Debug.Log($"Useful info: {value}");
Debug.DrawRay(position, direction, Color.red, 1f);
```

### 4. Profiler
For performance issues:
1. Window → Analysis → Profiler
2. Check Physics, Scripts, Rendering
3. Identify bottlenecks

---

## 💡 Pro Tips

### Development
1. **Test with PhysicsDebugVisualizer** first
2. **Use Gizmos** to visualize ranges and forces
3. **Start with low values** and increase gradually
4. **Save prefab variants** for different configurations
5. **Use version control** (Git) to track changes

### Balancing
1. **Make light items common**, heavy items rare
2. **Test extreme cases** (all cannons, all boosters)
3. **Player should feel powerful but challenged**
4. **Chaos is fun** but should be manageable
5. **Speed curve** should plateau, not drop to zero

### Polish
1. **Add sounds** for weapon fire, attachments
2. **Use particle effects** for visual feedback
3. **Screen shake** on heavy weapon fire
4. **Trail renderers** on projectiles
5. **UI feedback** for item collection

---

## 📞 Still Having Issues?

If you've checked everything above and still have problems:

1. **Check Unity Console** for error messages
2. **Verify Unity Version** (2022.3 LTS recommended)
3. **Review documentation** files
4. **Test with minimal setup** (just player and one item)
5. **Check values** in Inspector at runtime

---

## 🎯 Quick Reference Values

### Player
```
Move Force:    10-15
Max Speed:     5-7
Base Mass:     1
Magnetic Range: 3-5
Pull Force:     5-10
```

### Items
```
Light:  0.3-0.5 mass
Medium: 0.5-1.0 mass  
Heavy:  1.0-2.0 mass
Very Heavy: 2.0-3.0 mass
```

### Weapons
```
Fire Rate (shots/sec):
  Cannon: 0.5-1
  Machine Gun: 5-10
  Rocket: 0.3-0.5
  Laser: Continuous
  Shotgun: 1-2

Recoil Force:
  Cannon: 5-10
  Machine Gun: 1-2
  Rocket: 15-20
  Laser: 0-1
  Shotgun: 3-5
```

### Camera
```
Offset: (0, 10-15, -10 to -15)
Smooth Speed: 5-10
Min Distance: 5
Max Distance: 20
```

---

**Remember: Chaos is a feature, not a bug! 🎲**

