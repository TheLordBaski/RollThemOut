# 🎲 Roll Them Out

## A Physics-Based Magnetic Ball Survivor Game

> "Become a giant rolling ball of guns, spikes, and chaos!"

---

## 🎮 What Is This?

**Roll Them Out** is a unique take on the bullet-heaven/survivor genre where you control a **magnetic core** that physically attracts and attaches weapons and armor. Unlike games with predetermined slots (Brotato) or fixed orbits (Vampire Survivors), items attach based on **real physics collisions**, creating emergent and hilarious gameplay.

### The Core Hook
- 🧲 **Magnetic Attraction**: Items are pulled toward you
- 💥 **Physics-Based Attachment**: Items stick where they collide
- ⚖️ **Dynamic Balance**: Heavy items affect your movement
- 🎯 **Auto-Firing Weapons**: Focus on positioning, not aiming
- 🌪️ **Emergent Chaos**: Create unique, unpredictable builds

---

## ✨ Key Features

### 🎯 Magnetic Collection System
- Items drop on the ground during gameplay
- Your core automatically attracts nearby items
- Roll over items to attach them to your body
- No inventory limits - become as big as you want!

### ⚙️ Advanced Physics Simulation
- **Mass System**: Each item adds weight, affecting speed
- **Center of Mass**: Shifts dynamically based on attachments
- **Recoil Forces**: Weapons push you when firing
- **Torque Effects**: Unbalanced builds make you spin
- **Momentum**: Heavier = slower but more unstoppable

### 🔫 Weapon Variety
- **Cannon**: Heavy, slow, massive recoil
- **Machine Gun**: Fast fire rate, steady recoil
- **Rocket Launcher**: Propels you backward when firing!
- **Laser**: Lightweight, continuous beam
- **Shotgun**: Wide spread, moderate recoil

### 🛡️ Armor Types
- **Light Armor**: Basic protection, minimal weight
- **Heavy Armor**: High defense but slows you down
- **Spikes**: Damage enemies on contact
- **Shield**: Blocks projectiles (directional)
- **Rocket Booster**: Constant thrust - chaos mode!

---

## 📁 Project Structure

```
RollThemOut/
├── Assets/
│   └── Scripts/
│       ├── PlayerController.cs          # Main player controls
│       ├── AttachableItem.cs           # Base class for items
│       ├── WeaponItem.cs               # Weapon implementation
│       ├── ArmorItem.cs                # Armor implementation
│       ├── ItemSpawner.cs              # Spawns items
│       ├── EnemySpawner.cs             # Spawns enemies
│       ├── Enemy.cs                    # Enemy AI
│       ├── GameManager.cs              # Game state management
│       ├── CameraController.cs         # Dynamic camera
│       ├── Projectile.cs               # Weapon projectiles
│       └── PhysicsDebugVisualizer.cs   # Debug tools
│
├── GAME_DESIGN.md          # Complete design document
├── QUICK_START.md          # Step-by-step setup guide
├── FILES_SUMMARY.md        # All scripts explained
└── README.md               # This file
```

---

## 🚀 Quick Start

### Prerequisites
- Unity 2022.3 LTS or later
- Basic understanding of Unity Editor

### Setup (5 Minutes)
1. Open the project in Unity
2. Follow the [QUICK_START.md](QUICK_START.md) guide
3. Create basic prefabs for player, items, and enemies
4. Press Play and start rolling!

### Minimum Scene Setup
```
✅ Ground plane (10x10)
✅ Player sphere with PlayerController + Rigidbody
✅ Main Camera with CameraController
✅ GameManager (empty GameObject)
✅ ItemSpawner (empty GameObject)
✅ Weapon prefabs (cubes with WeaponItem)
✅ Armor prefabs (cubes with ArmorItem)
```

**Full setup guide**: See [QUICK_START.md](QUICK_START.md)

---

## 🎮 Controls

| Input | Action |
|-------|--------|
| WASD / Arrow Keys | Move the magnetic core |
| (Automatic) | Items attach on collision |
| (Automatic) | Weapons fire at enemies |

---

## 🎯 Gameplay Loop

```
🎲 Start as small magnetic sphere
    ↓
🧲 Items spawn and are attracted to you
    ↓
💥 Roll over items to attach them
    ↓
⚖️ Mass increases, physics changes
    ↓
🔫 Weapons auto-fire, creating recoil
    ↓
🌪️ Become giant chaotic ball of destruction
    ↓
🏆 Survive as long as possible!
```

---

## 🔬 Unique Physics Mechanics

### 1. Center of Mass Shifting
Attaching a heavy cannon to your left side shifts your center of mass, causing you to:
- Lean to that side
- Turn in circles when moving
- Be harder to control

### 2. Recoil-Based Movement
- Cannons push you backward when firing
- Multiple weapons firing = unpredictable movement
- Rocket launchers can launch you across the map!

### 3. Thrust Mechanics
- Rocket boosters provide constant thrust
- Direction depends on attachment point
- Multiple boosters = spinning chaos!

### 4. Mass-Based Gameplay
- Heavier = slower movement
- Heavier = more momentum
- Heavier = harder to stop
- Strategic decision: Power vs Control

---

## 🎨 Customization & Extension

### Easy to Modify
All scripts are:
- ✅ Fully commented
- ✅ Serialized fields for Inspector tweaking
- ✅ Modular architecture
- ✅ Extension-friendly

### Add New Content
**New Weapon Type:**
1. Open WeaponItem.cs
2. Add to WeaponType enum
3. Implement Fire[WeaponName] method
4. Done!

**New Armor Type:**
1. Open ArmorItem.cs
2. Add to ArmorType enum
3. Implement effect in ApplyAttachedEffect()
4. Done!

---

## 📚 Documentation

| Document | Description |
|----------|-------------|
| [GAME_DESIGN.md](GAME_DESIGN.md) | Complete game design document with all features explained |
| [QUICK_START.md](QUICK_START.md) | Step-by-step setup guide to get running in 5 minutes |
| [FILES_SUMMARY.md](FILES_SUMMARY.md) | Detailed breakdown of every script and its purpose |

---

## 🐛 Debugging Tools

### PhysicsDebugVisualizer
Attach to any GameObject to see:
- Velocity arrows
- Center of mass indicator
- Real-time physics stats
- Item count

### Gizmos
All scripts include debug visualization:
- Magnetic range (cyan sphere)
- Detection ranges (yellow sphere)
- Attack ranges (red sphere)
- Movement directions (arrows)

---

## 💡 Development Tips

### Testing Physics
1. Attach PhysicsDebugVisualizer to player
2. Create items with varying masses
3. Test attachment positions
4. Watch center of mass shift

### Balancing
- **Too fast?** Increase drag or reduce moveForce
- **Items fly away?** Increase magneticPullForce
- **Can't turn?** Reduce mass values
- **Too easy?** Increase enemy spawn rate

### Performance
- Use object pooling for projectiles
- Limit max attached items if needed
- Consider LOD for visual effects

---

## 🎯 Future Expansion Ideas

### Gameplay
- [ ] Wave-based progression system
- [ ] Boss enemies with unique mechanics
- [ ] Power-ups and temporary buffs
- [ ] Item rarity system (common/rare/legendary)
- [ ] Combo system for item synergies

### Features
- [ ] Item "throw" mechanic to remove unwanted items
- [ ] Environmental hazards
- [ ] Multiple playable cores with different abilities
- [ ] Achievement system
- [ ] Leaderboards

### Polish
- [ ] Particle effects for weapons
- [ ] Sound design
- [ ] Screen shake
- [ ] Main menu UI
- [ ] Tutorial system

### Advanced
- [ ] Multiplayer support
- [ ] Procedural level generation
- [ ] Save/load build configurations
- [ ] Replay system

---

## 🔧 Technical Details

### Unity Version
- Minimum: Unity 2022.3 LTS
- Recommended: Unity 2023.2 or later

### Dependencies
- Unity Physics (Built-in)
- TextMeshPro (for UI)
- Input System (optional, can use legacy)

### Performance Targets
- 60 FPS on mid-range hardware
- Handles 50+ attached items
- 20+ enemies on screen

---

## 🤝 Contributing

This is a game development learning project. Feel free to:
- Experiment with the code
- Create new weapon/armor types
- Add visual effects
- Optimize performance
- Share your creations!

---

## 📝 License

This project is for educational and portfolio purposes.
Feel free to use, modify, and learn from the code.

---

## 🎮 Let's Roll!

Ready to create chaos? Follow the [QUICK_START.md](QUICK_START.md) guide and start building your magnetic ball of destruction!

**Questions?** Check the documentation files.
**Bugs?** That's just emergent gameplay! 😄

---

**Made with Unity** 🎮 | **Powered by Physics** ⚙️ | **Driven by Chaos** 🌪️


