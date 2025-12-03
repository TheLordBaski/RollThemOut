using UnityEngine;
using UnityEngine.InputSystem;

namespace ChronoSniper
{
    /// <summary>
    /// Handles first-person camera look with mouse/gamepad input
    /// Works independently of Time.timeScale using unscaled delta time
    /// </summary>
    public class FirstPersonCamera : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform orientation;
        
        [Header("Look Sensitivity")]
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float gamepadSensitivity = 100f;
        
        [Header("Rotation Limits")]
        [SerializeField] private float maxVerticalAngle = 90f;
        [SerializeField] private float minVerticalAngle = -90f;

        [Header("Smoothing")]
        [SerializeField] private bool smoothRotation = true;
        [SerializeField] private float rotationSmoothTime = 0.05f;

        private float xRotation = 0f;
        private float yRotation = 0f;
        private Vector2 lookInput;
        private Vector2 currentLookVelocity;
        private Vector2 smoothedLookInput;

        private PlayerInput playerInput;
        private InputAction lookAction;

        private void Awake()
        {
            // Get PlayerInput component
            playerInput = GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                lookAction = playerInput.actions["Look"];
            }

            // Lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnEnable()
        {
            if (lookAction != null)
            {
                lookAction.Enable();
            }
        }

        private void OnDisable()
        {
            if (lookAction != null)
            {
                lookAction.Disable();
            }
        }

        private void Update()
        {
            // Use unscaled delta time to work independently of Time.timeScale
            float deltaTime = Time.unscaledDeltaTime;
            
            HandleLookInput(deltaTime);
        }

        private void HandleLookInput(float deltaTime)
        {
            if (lookAction == null) return;

            // Get raw input
            Vector2 rawInput = lookAction.ReadValue<Vector2>();
            
            // Determine if using mouse or gamepad based on input magnitude
            bool isGamepad = Gamepad.current != null && Gamepad.current.rightStick.ReadValue().magnitude > 0.1f;
            float sensitivity = isGamepad ? gamepadSensitivity : mouseSensitivity;

            // Apply sensitivity
            Vector2 scaledInput = rawInput * sensitivity;
            
            // Apply smoothing if enabled
            if (smoothRotation)
            {
                smoothedLookInput = Vector2.SmoothDamp(
                    smoothedLookInput, 
                    scaledInput, 
                    ref currentLookVelocity, 
                    rotationSmoothTime,
                    Mathf.Infinity,
                    deltaTime
                );
                lookInput = smoothedLookInput;
            }
            else
            {
                lookInput = scaledInput;
            }

            // Apply rotation using unscaled delta time
            float lookX = lookInput.x * deltaTime;
            float lookY = lookInput.y * deltaTime;

            // Vertical rotation (pitch) - clamped
            xRotation -= lookY;
            xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

            // Horizontal rotation (yaw)
            yRotation += lookX;

            // Apply rotations
            if (cameraTransform != null)
            {
                cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }

            if (orientation != null)
            {
                orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            }
        }

        /// <summary>
        /// Sets the camera rotation directly
        /// </summary>
        public void SetRotation(float vertical, float horizontal)
        {
            xRotation = Mathf.Clamp(vertical, minVerticalAngle, maxVerticalAngle);
            yRotation = horizontal;

            if (cameraTransform != null)
            {
                cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }

            if (orientation != null)
            {
                orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
            }
        }

        public float GetXRotation() => xRotation;
        public float GetYRotation() => yRotation;
    }
}
