using UnityEngine;
using UnityEngine.InputSystem;

namespace ChronoSniper
{
    /// <summary>
    /// Rigidbody-based first-person movement controller with support for:
    /// - Walking, sprinting, crouching, sliding, jumping
    /// - Slope and stair navigation
    /// - Unscaled time support (works when Time.timeScale = 0)
    /// Modular and extensible design
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(GroundDetector))]
    public class FirstPersonMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float crouchSpeed = 3f;
        [SerializeField] private float slideSpeed = 10f;
        [SerializeField] private float airControl = 0.3f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 10f;

        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float gravity = 20f;
        [SerializeField] private int maxAirJumps = 0; // 0 = single jump only
        
        [Header("Crouch Settings")]
        [SerializeField] private float crouchHeight = 1f;
        [SerializeField] private float standHeight = 2f;
        [SerializeField] private float crouchTransitionSpeed = 10f;
        
        [Header("Slide Settings")]
        [SerializeField] private float slideMinSpeed = 6f;
        [SerializeField] private float slideDuration = 1f;
        [SerializeField] private float slideDeceleration = 5f;
        [SerializeField] private bool canSlideOnSlopes = true;
        [SerializeField] private float slopeSlideBoostMultiplier = 0.1f;

        [Header("References")]
        [SerializeField] private Transform orientation;
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private LayerMask standUpCheckLayers = -1;

        private Rigidbody rb;
        private GroundDetector groundDetector;
        private PlayerInput playerInput;

        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction sprintAction;
        private InputAction crouchAction;

        private Vector2 moveInput;
        private MovementState currentState = MovementState.Walking;
        private Vector3 currentVelocity;
        private int airJumpCount = 0;
        
        // Slide state
        private float slideTimer = 0f;
        private Vector3 slideDirection;

        // Crouch state
        private float currentHeight;
        private float targetHeight;

        public MovementState CurrentState => currentState;
        public Vector3 Velocity => currentVelocity;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            groundDetector = GetComponent<GroundDetector>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            playerInput = GetComponent<PlayerInput>();

            // Configure rigidbody
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.useGravity = false; // We'll handle gravity manually for unscaled time support

            // Get input actions
            if (playerInput != null)
            {
                moveAction = playerInput.actions["Move"];
                jumpAction = playerInput.actions["Jump"];
                sprintAction = playerInput.actions["Sprint"];
                crouchAction = playerInput.actions["Crouch"];
            }

            currentHeight = standHeight;
            targetHeight = standHeight;
        }

        private void OnEnable()
        {
            if (jumpAction != null) jumpAction.performed += OnJump;
            if (crouchAction != null) crouchAction.performed += OnCrouchPressed;
            if (crouchAction != null) crouchAction.canceled += OnCrouchReleased;
        }

        private void OnDisable()
        {
            if (jumpAction != null) jumpAction.performed -= OnJump;
            if (crouchAction != null) crouchAction.performed -= OnCrouchPressed;
            if (crouchAction != null) crouchAction.canceled -= OnCrouchReleased;
        }

        private void Update()
        {
            // Use unscaled delta time for input reading
            float deltaTime = Time.unscaledDeltaTime;
            
            ReadInput();
            UpdateState();
            HandleCrouchHeight(deltaTime);
        }

        private void FixedUpdate()
        {
            // Use unscaled delta time for physics
            float deltaTime = Time.fixedUnscaledDeltaTime;
            
            ApplyMovement(deltaTime);
            ApplyGravity(deltaTime);
            LimitVelocity();
        }

        private void ReadInput()
        {
            if (moveAction != null)
            {
                moveInput = moveAction.ReadValue<Vector2>();
            }
        }

        private void UpdateState()
        {
            bool isGrounded = groundDetector.IsGrounded;

            // Handle sliding
            if (currentState == MovementState.Sliding)
            {
                slideTimer -= Time.unscaledDeltaTime;
                if (slideTimer <= 0f || !isGrounded)
                {
                    ExitSlide();
                }
                return;
            }

            // In air state
            if (!isGrounded)
            {
                currentState = MovementState.InAir;
                return;
            }

            // Reset air jump when grounded
            if (isGrounded)
            {
                airJumpCount = 0;
            }

            // Determine ground state
            bool isSprinting = sprintAction != null && sprintAction.IsPressed();
            bool isCrouching = crouchAction != null && crouchAction.IsPressed();

            if (isCrouching)
            {
                currentState = MovementState.Crouching;
            }
            else if (isSprinting)
            {
                currentState = MovementState.Sprinting;
            }
            else
            {
                currentState = MovementState.Walking;
            }
        }

        private void ApplyMovement(float deltaTime)
        {
            // Get move direction in world space
            Vector3 forward = orientation != null ? orientation.forward : transform.forward;
            Vector3 right = orientation != null ? orientation.right : transform.right;
            
            Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

            // Apply slope movement if on slope
            if (groundDetector.OnSlope && !groundDetector.IsSlopeTooSteep())
            {
                moveDirection = groundDetector.GetSlopeMoveDirection(moveDirection);
            }

            float targetSpeed = GetTargetSpeed();
            Vector3 targetVelocity = moveDirection * targetSpeed;

            // Handle sliding
            if (currentState == MovementState.Sliding)
            {
                ApplySlideMovement(deltaTime);
                return;
            }

            // Calculate acceleration
            float accel = groundDetector.IsGrounded ? acceleration : acceleration * airControl;
            
            // Smoothly accelerate to target velocity
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            Vector3 velocityDiff = targetVelocity - horizontalVelocity;
            
            float decel = moveDirection.magnitude > 0.1f ? accel : deceleration;
            Vector3 velocityChange = velocityDiff * decel * deltaTime;
            
            // Apply velocity change
            currentVelocity = rb.velocity + new Vector3(velocityChange.x, 0f, velocityChange.z);
            rb.velocity = currentVelocity;
        }

        private void ApplySlideMovement(float deltaTime)
        {
            // Continue sliding in the initial direction with deceleration
            float currentSlideSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
            float newSpeed = Mathf.Max(currentSlideSpeed - slideDeceleration * deltaTime, crouchSpeed);
            
            Vector3 slideVelocity = slideDirection * newSpeed;
            
            // Add slope boost if sliding on slope
            if (canSlideOnSlopes && groundDetector.OnSlope)
            {
                slideVelocity += Vector3.down * groundDetector.CurrentSlopeAngle * slopeSlideBoostMultiplier;
            }
            
            rb.velocity = new Vector3(slideVelocity.x, rb.velocity.y, slideVelocity.z);
        }

        private void ApplyGravity(float deltaTime)
        {
            // Don't apply gravity if on steep slope (let physics handle it)
            if (groundDetector.IsGrounded && !groundDetector.OnSlope)
            {
                // Apply small downward force to keep grounded
                rb.velocity = new Vector3(rb.velocity.x, -2f, rb.velocity.z);
            }
            else if (!groundDetector.IsGrounded)
            {
                // Apply custom gravity
                rb.velocity += Vector3.down * gravity * deltaTime;
            }
        }

        private void LimitVelocity()
        {
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            float maxSpeed = GetMaxSpeed();

            if (horizontalVelocity.magnitude > maxSpeed)
            {
                Vector3 limitedVelocity = horizontalVelocity.normalized * maxSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }

        private float GetTargetSpeed()
        {
            switch (currentState)
            {
                case MovementState.Sprinting:
                    return sprintSpeed;
                case MovementState.Crouching:
                    return crouchSpeed;
                case MovementState.Sliding:
                    return slideSpeed;
                case MovementState.InAir:
                    // Maintain last ground speed in air
                    return Mathf.Min(new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude, sprintSpeed);
                default:
                    return walkSpeed;
            }
        }

        private float GetMaxSpeed()
        {
            switch (currentState)
            {
                case MovementState.Sliding:
                    return slideSpeed * 1.5f; // Allow some overspeed on slides
                default:
                    return GetTargetSpeed() * 1.1f;
            }
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            bool canJump = groundDetector.IsGrounded || airJumpCount < maxAirJumps;

            if (canJump && currentState != MovementState.Sliding)
            {
                // Cancel vertical velocity before jumping
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                
                // Apply jump force
                rb.velocity += Vector3.up * jumpForce;

                if (!groundDetector.IsGrounded)
                {
                    airJumpCount++;
                }
            }
        }

        private void OnCrouchPressed(InputAction.CallbackContext context)
        {
            targetHeight = crouchHeight;

            // Start slide if moving fast enough
            if (groundDetector.IsGrounded && 
                currentState == MovementState.Sprinting &&
                new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude >= slideMinSpeed)
            {
                StartSlide();
            }
        }

        private void OnCrouchReleased(InputAction.CallbackContext context)
        {
            // Check if there's space to stand up
            if (CanStandUp())
            {
                targetHeight = standHeight;
            }
        }

        private void StartSlide()
        {
            currentState = MovementState.Sliding;
            slideTimer = slideDuration;
            
            // Store current movement direction
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            slideDirection = horizontalVelocity.normalized;
        }

        private void ExitSlide()
        {
            if (currentState == MovementState.Sliding)
            {
                if (crouchAction != null && crouchAction.IsPressed())
                {
                    // Player is still holding crouch
                    currentState = MovementState.Crouching;
                }
                else if (CanStandUp())
                {
                    // Player released crouch and can stand
                    currentState = MovementState.Walking;
                    targetHeight = standHeight;
                }
                else
                {
                    // Cannot stand up, stay crouched
                    currentState = MovementState.Crouching;
                }
            }
        }

        private void HandleCrouchHeight(float deltaTime)
        {
            // Smoothly transition height
            currentHeight = Mathf.Lerp(currentHeight, targetHeight, crouchTransitionSpeed * deltaTime);
            
            if (capsuleCollider != null)
            {
                capsuleCollider.height = currentHeight;
                capsuleCollider.center = new Vector3(0f, currentHeight / 2f, 0f);
            }
        }

        private bool CanStandUp()
        {
            // Raycast upward to check for obstacles
            Vector3 checkPosition = transform.position + Vector3.up * crouchHeight;
            float checkDistance = standHeight - crouchHeight + 0.2f;
            
            return !Physics.Raycast(checkPosition, Vector3.up, checkDistance, standUpCheckLayers, QueryTriggerInteraction.Ignore);
        }

        private void OnDrawGizmosSelected()
        {
            // Draw movement direction
            if (Application.isPlaying)
            {
                Gizmos.color = Color.blue;
                Vector3 forward = orientation != null ? orientation.forward : transform.forward;
                Vector3 right = orientation != null ? orientation.right : transform.right;
                Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
                Gizmos.DrawRay(transform.position, moveDirection * 2f);
            }
        }
    }
}
