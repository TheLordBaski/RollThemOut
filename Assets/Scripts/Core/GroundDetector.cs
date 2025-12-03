using UnityEngine;

namespace ChronoSniper
{
    /// <summary>
    /// Handles ground detection, slope checking, and stair detection for the first-person controller
    /// </summary>
    public class GroundDetector : MonoBehaviour
    {
        [Header("Ground Detection")]
        [SerializeField] private float groundCheckDistance = 0.3f;
        [SerializeField] private float groundCheckRadius = 0.28f;
        [SerializeField] private LayerMask groundLayer = -1;
        [SerializeField] private Transform groundCheckPoint;

        [Header("Slope Settings")]
        [SerializeField] private float maxSlopeAngle = 45f;

        private RaycastHit slopeHit;
        private bool isGrounded;
        private bool onSlope;
        private float currentSlopeAngle;

        public bool IsGrounded => isGrounded;
        public bool OnSlope => onSlope;
        public float CurrentSlopeAngle => currentSlopeAngle;
        public Vector3 SlopeNormal => slopeHit.normal;

        private void FixedUpdate()
        {
            CheckGround();
        }

        private void CheckGround()
        {
            Vector3 checkPosition = groundCheckPoint != null ? groundCheckPoint.position : transform.position;
            
            // Sphere cast for ground detection
            isGrounded = Physics.CheckSphere(checkPosition, groundCheckRadius, groundLayer);

            // Check for slopes
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, groundCheckDistance + 0.5f, groundLayer))
            {
                currentSlopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                onSlope = currentSlopeAngle > 0.1f && currentSlopeAngle <= maxSlopeAngle;
            }
            else
            {
                currentSlopeAngle = 0f;
                onSlope = false;
            }
        }

        /// <summary>
        /// Projects movement direction onto slope
        /// </summary>
        public Vector3 GetSlopeMoveDirection(Vector3 moveDirection)
        {
            if (!onSlope) return moveDirection;
            
            return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
        }

        /// <summary>
        /// Checks if the current slope is too steep to climb
        /// </summary>
        public bool IsSlopeTooSteep()
        {
            return currentSlopeAngle > maxSlopeAngle;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 checkPosition = groundCheckPoint != null ? groundCheckPoint.position : transform.position;
            
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
            
            if (onSlope)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position, slopeHit.normal * 2f);
            }
        }
    }
}
