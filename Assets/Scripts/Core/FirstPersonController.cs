using UnityEngine;
using UnityEngine.InputSystem;

namespace ChronoSniper
{
    /// <summary>
    /// Unified first-person controller that integrates movement and camera systems
    /// This is the main component to add to the player GameObject
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
            // Check if movement should be disabled based on game state
            if (GameManager.Instance != null && !enableMovementInPlanning)
            {
                if (GameManager.Instance.CurrentState == GameState.Planning)
                {
                    // Movement is always enabled now, but you can disable if needed
                    // movement.enabled = false;
                }
            }

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
