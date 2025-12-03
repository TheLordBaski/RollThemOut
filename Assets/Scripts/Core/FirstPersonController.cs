using UnityEngine;
using UnityEngine.InputSystem;

namespace ChronoSniper
{
    /// <summary>
    /// Unified first-person controller that integrates movement and camera systems
    /// This is the main component to add to the player GameObject
    /// 
    /// SETUP INSTRUCTIONS:
    /// 1. Add this component to your player GameObject
    /// 2. Ensure the GameObject has:
    ///    - Rigidbody (configured automatically)
    ///    - CapsuleCollider (for collision)
    ///    - PlayerInput component (with InputSystem_Actions asset)
    /// 3. Create a child GameObject for the camera and assign it to FirstPersonCamera.cameraTransform
    /// 4. Create an empty child GameObject called "Orientation" for movement direction reference
    /// 5. Assign the orientation transform to FirstPersonMovement.orientation
    /// 6. Configure layer masks for ground detection in GroundDetector component
    /// 7. The existing PlayerController can coexist for shooting mechanics
    /// 
    /// CONTROLS (as configured in InputSystem_Actions):
    /// - WASD/Left Stick: Move
    /// - Mouse/Right Stick: Look
    /// - Space: Jump
    /// - Left Shift: Sprint
    /// - C/Ctrl: Crouch (hold to crouch, sprint+crouch for slide)
    /// - Left Click: Place bounce point (Planning state)
    /// - Right Click: Remove bounce point
    /// - Escape: Toggle cursor lock
    /// </summary>
    [RequireComponent(typeof(FirstPersonMovement))]
    [RequireComponent(typeof(FirstPersonCamera))]
    [RequireComponent(typeof(PlayerInput))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] private FirstPersonMovement movement;
        [SerializeField] private FirstPersonCamera cameraController;
        
        [Header("Player Settings")]
        [SerializeField] private bool enableMovementInPlanning = false;

        private PlayerInput playerInput;

        public FirstPersonMovement Movement => movement;
        public FirstPersonCamera CameraController => cameraController;

        private void Awake()
        {
            // Get components
            if (movement == null) movement = GetComponent<FirstPersonMovement>();
            if (cameraController == null) cameraController = GetComponent<FirstPersonCamera>();
            playerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            // Setup cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleGameStateInput();
        }

        private void HandleGameStateInput()
        {
            // Handle cursor unlock
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                ToggleCursor();
            }
        }

        private void ToggleCursor()
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        /// <summary>
        /// Teleport the player to a position
        /// </summary>
        public void Teleport(Vector3 position)
        {
            transform.position = position;
        }

        /// <summary>
        /// Set the player's rotation
        /// </summary>
        public void SetRotation(float vertical, float horizontal)
        {
            if (cameraController != null)
            {
                cameraController.SetRotation(vertical, horizontal);
            }
        }
    }
}
