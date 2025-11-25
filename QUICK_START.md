# Quick Start Guide - Roll Them Out

## 🚀 Getting Started in 5 Minutes

### Step 1: Create Basic Scene
1. Open Unity
2. Create new scene (File → New Scene → Basic)
3. Save scene as "GameScene"

### Step 2: Create Ground
1. Right-click Hierarchy → 3D Object → Plane
2. Rename to "Ground"
3. Scale: X=10, Y=1, Z=10
4. Position: (0, 0, 0)

### Step 3: Create Player
1. Right-click Hierarchy → 3D Object → Sphere
2. Rename to "Player"
3. Position: (0, 1, 0)
4. Scale: (1, 1, 1)
5. Add Component → Rigidbody
6. Add Component → PlayerController script
7. Add Component → PhysicsDebugVisualizer (optional, for testing)
8. Tag: Set to "Player" (create if doesn't exist)

### Step 4: Setup Layers
1. Edit → Project Settings → Tags and Layers
2. Add these layers:
   - Layer 6: "Item"
   - Layer 7: "Enemy"

### Step 5: Setup Player Controller
1. Select Player in Hierarchy
2. In PlayerController component:
   - Rb: Drag Player's Rigidbody here
   - Item Layer: Select "Item" layer

### Step 6: Setup Camera
1. Select Main Camera
2. Add Component → CameraController
3. Drag Player transform to Target field
4. Adjust Offset: (0, 10, -10) for good top-down view

### Step 7: Create Game Manager
1. Right-click Hierarchy → Create Empty
2. Rename to "GameManager"
3. Add Component → GameManager script
4. Drag Player to Player field

### Step 8: Create Simple Weapon Prefab
1. Right-click Hierarchy → 3D Object → Cube
2. Rename to "BasicCannon"
3. Scale: (0.3, 0.3, 0.5)
4. Add Component → Rigidbody
   - Mass: 2
5. Add Component → WeaponItem script
   - Set Weapon Type: Cannon
   - Set Mass: 2
   - Set Fire Rate: 1
   - Set Recoil Force: 10
6. Set Layer: "Item"
7. Add a material/color (optional)
8. Drag to Project folder to create prefab
9. Delete from Hierarchy

### Step 9: Create Simple Armor Prefab
1. Right-click Hierarchy → 3D Object → Cube
2. Rename to "BasicArmor"
3. Scale: (0.4, 0.4, 0.2)
4. Add Component → Rigidbody
   - Mass: 1
5. Add Component → ArmorItem script
   - Set Armor Type: LightArmor
   - Set Mass: 1
   - Set Defense Bonus: 5
6. Set Layer: "Item"
7. Add a material/color (optional)
8. Drag to Project folder to create prefab
9. Delete from Hierarchy

### Step 10: Create Item Spawner
1. Right-click Hierarchy → Create Empty
2. Rename to "ItemSpawner"
3. Add Component → ItemSpawner script
4. In Inspector:
   - Add elements to Weapon Prefabs list
   - Drag BasicCannon prefab
   - Add elements to Armor Prefabs list
   - Drag BasicArmor prefab
   - Spawn Radius: 15
   - Spawn Interval: 3

### Step 11: Create Enemy Prefab (Optional)
1. Right-click Hierarchy → 3D Object → Capsule
2. Rename to "BasicEnemy"
3. Add Component → Rigidbody
4. Add Component → Enemy script
5. Tag: "Enemy"
6. Layer: "Enemy"
7. Add a red material
8. Drag to Project folder to create prefab
9. Delete from Hierarchy

### Step 12: Create Enemy Spawner (Optional)
1. Right-click Hierarchy → Create Empty
2. Rename to "EnemySpawner"
3. Add Component → EnemySpawner script
4. Add BasicEnemy prefab to Enemy Prefabs list
5. Drag Player to Player field

### Step 13: Configure Physics Collision Matrix
1. Edit → Project Settings → Physics
2. Layer Collision Matrix:
   - ✅ Item collides with: Default, Player, Ground
   - ✅ Enemy collides with: Default, Player, Ground, Item
   - ✅ Player collides with: Everything

### Step 14: Test!
1. Press Play
2. Use WASD or Arrow Keys to move
3. Items should spawn and be attracted to you
4. Roll over items to attach them
5. Watch your mass increase and physics change!

## 🎮 Controls
- **WASD / Arrow Keys**: Move
- Items automatically attach on collision

## 🔧 Troubleshooting

### Player doesn't move
- Check Rigidbody is attached
- Check Move Force is set (default: 10)
- Verify constraints aren't freezing position

### Items don't attract to player
- Verify Item Layer is set correctly
- Check Magnetic Range (default: 3)
- Ensure items have Rigidbody components

### Items don't attach
- Verify collision detection isn't set to None
- Check layer collision matrix
- Ensure items have AttachableItem-derived component

### Camera doesn't follow
- Verify Target is assigned in CameraController
- Check camera isn't parented to anything

## 🎨 Next Steps

### Make it Look Better
1. Add materials/textures to objects
2. Create particle effects for weapon fire
3. Add trail renderers to projectiles
4. Create visual feedback for attachments

### Add More Gameplay
1. Create more weapon types
2. Add power-ups
3. Implement wave system
4. Add UI with TextMeshPro
5. Create boss enemies

### Polish
1. Add sound effects
2. Implement screen shake on weapon fire
3. Add explosion effects
4. Create main menu
5. Add pause functionality

## 📝 Tips for Testing

1. **Add PhysicsDebugVisualizer** to player to see:
   - Velocity arrows
   - Center of mass
   - Mass display

2. **Test Different Builds**:
   - All cannons on one side (spinning!)
   - Rocket boosters pointing backwards (speed!)
   - Mix of heavy and light items (chaos!)

3. **Adjust Values**:
   - Increase magnetic range for easier collection
   - Increase item mass for more dramatic physics
   - Increase spawn rate for chaos mode

4. **Performance Testing**:
   - Many items might slow down game
   - Consider object pooling for items/enemies
   - Limit max attached items if needed

## 🐛 Common Issues

**Issue**: Player rolls too much
**Fix**: Increase Angular Drag on Rigidbody (try 2-5)

**Issue**: Items fall through ground
**Fix**: Increase collision detection to Continuous on item Rigidbodies

**Issue**: Player moves too slow with items
**Fix**: Increase Move Force or adjust max speed calculation in PlayerController

**Issue**: Weapons don't fire
**Fix**: Create Enemy layer and assign to Enemy Layer field in WeaponItem

---

**You're ready to roll! Have fun! 🎲**

