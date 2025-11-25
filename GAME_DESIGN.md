# Roll Them Out - Game Design Document

## Core Concept
A physics-based action game where you control a magnetic core that attracts and attaches weapons and armor. The unique twist: attached items affect your physics realistically, creating emergent gameplay moments.

## The Hook
Instead of predetermined slots (like Brotato) or fixed orbits (like Vampire Survivors), your character is a magnetic core that physically accumulates items, creating a unique snowball effect.

## Key Features

### 1. Magnetic Attraction System
- Items drop on the ground during gameplay
- Your magnetic core automatically pulls nearby items toward you
- Items attach when they collide with your body
- No inventory limits - become as big as you can!

### 2. Physics-Driven Gameplay
The game's core innovation is **realistic physics simulation**:

#### Mass & Movement
- Each attached item adds its mass to your total mass
- Heavier players move slower but have more momentum
- The center of mass shifts based on where items attach
- Unbalanced attachment (heavy cannon on left) causes listing and turning bias

#### Recoil & Thrust
- Heavy weapons create recoil when firing
- Rocket launchers propel you backward
- Rocket boosters provide constant thrust in their facing direction
- Multiple weapons firing at once create chaotic movement

#### Balance & Control
- Items attach at the collision point, creating asymmetric builds
- Heavy items on one side make you roll in circles
- Strategic placement creates unique movement patterns
- You can become completely uncontrollable with the right (wrong?) setup

### 3. Weapon System

#### Weapon Types
1. **Cannon**
   - Very heavy (high mass impact)
   - Slow fire rate
   - Heavy recoil pushes player backward
   - High damage

2. **Machine Gun**
   - Medium weight
   - Fast fire rate
   - Slight continuous recoil
   - Moderate damage

3. **Rocket Launcher**
   - Extremely heavy
   - Slow fire rate
   - Massive recoil - propels player significantly
   - Explosive damage

4. **Laser**
   - Lightweight
   - Continuous beam
   - Minimal recoil
   - Moderate damage

5. **Shotgun**
   - Medium weight
   - Wide spread
   - Moderate recoil
   - High close-range damage

All weapons auto-fire at the nearest enemy within detection range.

### 4. Armor System

#### Armor Types
1. **Light Armor**
   - Low mass
   - Small defense bonus
   - Minimal speed impact

2. **Heavy Armor**
   - High mass
   - Large defense bonus
   - Significantly slows player

3. **Spikes**
   - Medium mass
   - Damages enemies on contact
   - Deals damage when rolling into enemies

4. **Shield**
   - Medium mass
   - Blocks projectiles from one direction
   - Directional protection

5. **Rocket Booster**
   - Light mass
   - Provides constant thrust in facing direction
   - Can cause uncontrolled acceleration
   - Attaching multiple boosters = chaos!

### 5. Emergent Gameplay Scenarios

The physics system creates unique moments:

- **The Spinning Death Ball**: Attach rocket boosters pointing different directions, creating a spinning vortex of destruction
- **The Lopsided Cannon**: Heavy cannon on one side makes you constantly circle, requiring you to time attacks
- **The Rocket Ride**: Rocket launcher fires backward, launching you into enemy crowds
- **The Unstoppable Boulder**: So much mass that you can't turn, only barrel forward
- **The Balanced Build**: Carefully attach items symmetrically for precise control

## Scripts Overview

### Core Systems

#### PlayerController.cs
- Handles player movement (WASD/Arrow keys)
- Physics-based rolling movement
- Magnetic attraction to nearby items
- Item attachment/detachment system
- Dynamic center of mass calculation
- Speed reduction based on total mass

#### AttachableItem.cs (Base Class)
- Base class for all attachable items
- Handles attachment/detachment lifecycle
- Manages physics properties
- Abstract method for item-specific effects

#### WeaponItem.cs
- Inherits from AttachableItem
- Auto-targeting system
- Weapon-specific firing patterns
- Recoil application to player
- Detection and engagement ranges

#### ArmorItem.cs
- Inherits from AttachableItem
- Defense bonuses
- Speed modifiers
- Special effects (spikes damage, booster thrust)
- Collision-based damage (spikes)

### Gameplay Systems

#### GameManager.cs
- Singleton game state manager
- Score tracking
- Time tracking
- Item count tracking
- Player mass display
- Game over/restart logic

#### ItemSpawner.cs
- Spawns weapons and armor periodically
- Configurable spawn rate and radius
- Limits maximum items in world
- Random item selection

#### EnemySpawner.cs
- Spawns enemies around player
- Difficulty scaling over time
- Minimum/maximum spawn distances
- Enemy count management

#### Enemy.cs
- Basic enemy AI
- Chase player within detection range
- Attack when in range
- Health and damage system
- Rewards score on death

#### CameraController.cs
- Smooth camera following
- Dynamic zoom based on player mass
- Configurable offset and smoothing

## Setup Instructions

### 1. Scene Setup
1. Create a new scene
2. Add a plane for the ground (scaled to 50x50 or larger)
3. Create a sphere for the player (scale 1,1,1)
4. Add main camera

### 2. Player Setup
1. Select player sphere
2. Add Component → Rigidbody
3. Add Component → PlayerController script
4. Configure:
   - Move Force: 10
   - Max Speed: 5
   - Base Mass: 1
   - Magnetic Range: 3
   - Item Layer: (create "Item" layer and assign)

### 3. Camera Setup
1. Select Main Camera
2. Add Component → CameraController script
3. Assign player transform to Target field

### 4. Game Manager Setup
1. Create empty GameObject named "GameManager"
2. Add GameManager script
3. Assign player reference

### 5. Spawner Setup
1. Create empty GameObject named "ItemSpawner"
2. Add ItemSpawner script
3. Create empty GameObject named "EnemySpawner"
4. Add EnemySpawner script

### 6. Create Item Prefabs

#### Weapon Prefab
1. Create cube (scale: 0.3, 0.3, 0.5)
2. Add Rigidbody
3. Add WeaponItem script
4. Configure weapon type and properties
5. Set layer to "Item"
6. Save as prefab

#### Armor Prefab
1. Create cube (scale: 0.4, 0.4, 0.2)
2. Add Rigidbody
3. Add ArmorItem script
4. Configure armor type and properties
5. Set layer to "Item"
6. Save as prefab

#### Enemy Prefab
1. Create capsule
2. Add Rigidbody
3. Add Enemy script
4. Set tag to "Enemy"
5. Save as prefab

### 7. Layer Setup
1. Create new layers:
   - "Item" (for weapons/armor)
   - "Enemy" (for enemies)
2. Set layer collision matrix (Edit → Project Settings → Physics)
   - Items should collide with everything
   - Enemies should collide with Player and Items

### 8. Tags Setup
1. Create tags:
   - "Player"
   - "Enemy"
2. Assign "Player" tag to player sphere

## Gameplay Tips

### For Players
- Start by collecting balanced items on both sides
- Heavy items slow you down but provide more firepower
- Rocket boosters can help or hinder depending on placement
- Spikes are great for melee-style play
- Too many items = loss of control!

### For Developers
- Adjust magnetic range to change difficulty
- Tweak item mass values to balance physics
- Modify spawn rates for pacing
- Add visual effects for polish
- Consider adding item "throw" mechanic to remove unwanted items

## Future Expansion Ideas

1. **Wave System**: Increasing enemy waves with bosses
2. **Item Rarities**: Common, rare, legendary items with different colors
3. **Combo System**: Rewards for specific item combinations
4. **Upgrade System**: Improve attached items over time
5. **Environmental Hazards**: Lava, spikes, moving platforms
6. **Power-ups**: Temporary effects (shield, speed boost, magnet boost)
7. **Item Throw Mechanic**: Remove unwanted items by flinging them at enemies
8. **Multiplayer**: Compete to become the biggest ball
9. **Custom Builds**: Save and load specific item configurations
10. **Achievements**: Unlock rewards for unique builds or playstyles

## Technical Notes

- Unity version: 2022.3 or later recommended
- Uses new Input System (InputSystem_Actions.inputactions)
- Physics-based movement requires consistent framerate
- Consider using FixedUpdate for all physics calculations
- Center of mass calculation is crucial for proper physics

## Credits
Built with Unity Engine
Physics simulation using Unity's PhysX engine

---

**Have fun rolling into chaos!** 🎮🎲

