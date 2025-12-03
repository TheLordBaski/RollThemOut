# FirstPersonController - Rigidbody Implementation

## Overview
The FirstPersonController has been converted from using Unity's CharacterController to using Rigidbody physics. This implementation includes support for time-independent movement that works even when `Time.timeScale` is set to 0 (for time-stopping game mechanics).

## Key Changes

### 1. Component Requirements
- **Changed:** `RequireComponent(typeof(CharacterController))` → `RequireComponent(typeof(Rigidbody))`
- The controller now requires a Rigidbody component instead of CharacterController

### 2. Rigidbody Configuration
The Rigidbody is automatically configured in the `Start()` method with optimal settings:
- **Interpolation:** `Interpolate` - for smooth movement
- **Collision Detection:** `Continuous` - for reliable collision detection
- **Constraints:** `FreezeRotation` - prevents the player capsule from tipping over

### 3. Time-Independent Movement
All time-based calculations now use `Time.unscaledDeltaTime` instead of `Time.deltaTime`:
- Movement speed calculations
- Rotation velocity
- Jump and gravity timers
- Acceleration/deceleration

This ensures the player can move even when `Time.timeScale = 0`.

### 4. Physics-Based Movement
- **Movement:** Uses `Rigidbody.velocity` for horizontal movement
- **Rotation:** Uses `Rigidbody.MoveRotation()` for player rotation
- **Gravity & Jumping:** Directly modifies Rigidbody vertical velocity
- **Ground Check:** Unchanged (already used Physics.CheckSphere)

### 5. Update Loop Changes
- **Update():** Handles camera rotation for immediate input response
- **FixedUpdate():** Handles all physics operations (movement, jumping, ground checks)
- **LateUpdate():** No longer used (kept as empty method for compatibility)

## Setup Instructions

### Required Components
Your player GameObject needs these components:
1. **Rigidbody** (required)
2. **PlayerInput** (required for New Input System)
3. **StarterAssetsInputs** (required)
4. **Capsule Collider** (or similar collider)

### Rigidbody Settings
The script automatically configures the Rigidbody, but you can also set these manually:
- **Mass:** 1 (default, adjust as needed)
- **Drag:** 0 (recommended for FPS movement)
- **Angular Drag:** 0.05 (default)
- **Use Gravity:** false (handled by script)
- **Interpolation:** Interpolate (set by script)
- **Collision Detection:** Continuous (set by script)
- **Constraints:**
  - Freeze Rotation X ✓ (set by script)
  - Freeze Rotation Y ✓ (set by script)
  - Freeze Rotation Z ✓ (set by script)

### Collider Setup
- Use a **Capsule Collider** matching the player dimensions
- Ensure the `groundedRadius` parameter matches your collider's radius
- Set appropriate `groundLayers` in the inspector

## Testing Time-Independent Movement

### Using TimeScaleController (Included)
A `TimeScaleController` script has been provided for testing:

1. Add `TimeScaleController` component to any GameObject in your scene
2. Press **T** to toggle time freeze (Time.timeScale = 0)
3. Press **R** to reset time to normal
4. The player should continue to move and respond to input even when time is frozen

### Manual Testing
```csharp
// Freeze time
Time.timeScale = 0f;

// Player should still:
// - Move with WASD
// - Look around with mouse
// - Jump with Space
// - Sprint with Shift

// Resume time
Time.timeScale = 1f;
```

## Differences from CharacterController

### Advantages
✓ Works with `Time.timeScale = 0` for time-stopping mechanics
✓ Fully integrated with Unity's physics system
✓ Can interact with other Rigidbodies naturally
✓ More realistic physics-based movement
✓ Smoother interpolation

### Considerations
- Movement feels more physics-based (has momentum)
- May require physics material for ground friction
- Slightly different feel than CharacterController
- Must use FixedUpdate for physics operations

## Troubleshooting

### Player is sliding/not stopping
- Adjust `speedChangeRate` for faster deceleration
- Add a Physics Material with friction to the player's collider

### Player movement feels sluggish
- Increase `moveSpeed` and `sprintSpeed` values
- Increase `speedChangeRate` for faster acceleration

### Player tips over or rotates unexpectedly
- Verify `RigidbodyConstraints.FreezeRotation` is set
- The script sets this automatically in Start()

### Ground detection issues
- Adjust `groundedOffset` and `groundedRadius` to match your collider
- Ensure `groundLayers` includes all ground surfaces

### Time.timeScale = 0 not working
- Verify all `Time.deltaTime` has been replaced with `Time.unscaledDeltaTime`
- Check that FixedUpdate is being called (it uses unscaled time internally)

## API Reference

### Public Properties
All original properties are preserved:
- `moveSpeed` - Normal movement speed (default: 4.0)
- `sprintSpeed` - Sprint movement speed (default: 6.0)
- `jumpHeight` - Jump height in meters (default: 1.2)
- `gravity` - Custom gravity value (default: -15.0)
- `rotationSpeed` - Camera/player rotation speed (default: 1.0)
- `speedChangeRate` - Acceleration/deceleration rate (default: 10.0)
- `groundedOffset` - Ground check offset (default: -0.14)
- `groundedRadius` - Ground check radius (default: 0.5)
- `groundLayers` - Layers considered as ground

### Notes
- The implementation maintains the same public API as the CharacterController version
- Existing settings and inspector values should work without modification
- The script is fully compatible with Unity's New Input System
