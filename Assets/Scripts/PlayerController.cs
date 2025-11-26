using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveForce = 15f; // Increased from 10f for more responsive movement
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float slowdownFactor = 0.95f; // Reduced from 0.98f for quicker stopping
    
    [Header("Physics")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float baseMass = 1f;
    
    [Header("Magnetic Attraction")]
    [SerializeField] private float magneticRange = 3f;
    [SerializeField] private float magneticPullForce = 5f;
    [SerializeField] private LayerMask itemLayer;
    
    [Header("Attachment")]
    [SerializeField] private List<AttachableItem> attachedItems = new List<AttachableItem>();
    [SerializeField] private float maxAttachmentOffset = 0.5f;
    
    private Vector3 moveInput;
    private Vector3 centerOfMass;
    
    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        
        // Set up physics properties
        rb.mass = baseMass;
        rb.useGravity = true;
        rb.linearDamping = 0.3f; // Reduced from 0.5f for more responsive movement
        rb.angularDamping = 0.5f;
        
        UpdateCenterOfMass();
    }

    void Update()
    {
        HandleInput();
        DetectNearbyItems();
    }
    
    void FixedUpdate()
    {
        ApplyMovement();
        LimitSpeed();
    }
    
    private void HandleInput()
    {
        Vector2 rawInput = InputSystem.actions.FindAction("Move").ReadValue<Vector2>().normalized;
        moveInput.x = rawInput.x;
        moveInput.z = rawInput.y;
        moveInput.y = 0f;
    }
    
    private void ApplyMovement()
    {
        if (moveInput.magnitude > 0.1f)
        {
            // Apply force based on input
            // Use ForceMode.Acceleration for more responsive movement regardless of mass
            Vector3 force = moveInput * moveForce;
            rb.AddForce(force, ForceMode.Acceleration);
        }
        else
        {
            // Apply slowdown when no input
            rb.linearVelocity *= slowdownFactor;
        }
        
        // Apply torque for rolling effect
        if (moveInput.magnitude > 0.1f)
        {
            Vector3 torqueDirection = Vector3.Cross(Vector3.up, moveInput);
            rb.AddTorque(torqueDirection * moveForce * 0.5f, ForceMode.Force);
        }
    }
    
    private void LimitSpeed()
    {
        // Calculate effective max speed based on mass
        float effectiveMaxSpeed = maxSpeed * (baseMass / rb.mass);
        
        if (rb.linearVelocity.magnitude > effectiveMaxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * effectiveMaxSpeed;
        }
    }
    
    private void DetectNearbyItems()
    {
        // Find all items within magnetic range
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, magneticRange, itemLayer);
        
        foreach (Collider col in nearbyColliders)
        {
            AttachableItem item = col.GetComponent<AttachableItem>();
            if (item != null && !item.IsAttached)
            {
                // Pull item towards player
                Vector3 direction = (transform.position - item.transform.position).normalized;
                float distance = Vector3.Distance(transform.position, item.transform.position);
                
                Rigidbody itemRb = item.GetComponent<Rigidbody>();
                if (itemRb != null)
                {
                    float pullStrength = magneticPullForce * (1f - distance / magneticRange);
                    itemRb.AddForce(direction * pullStrength, ForceMode.Force);
                }
            }
        }
    }
    
    public void AttachItem(AttachableItem item)
    {
        if (attachedItems.Contains(item)) return;
        
        attachedItems.Add(item);
        
        // Attach item to player
        item.transform.SetParent(transform);
        
        // Disable item's rigidbody and use joint instead
        Rigidbody itemRb = item.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            // Add fixed joint to attach item
            FixedJoint joint = item.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = rb;
            
            // Add item's mass to player
            rb.mass += item.Mass;
        }
        
        // Mark item as attached
        item.OnAttached(this);
        
        // Update center of mass
        UpdateCenterOfMass();
    }
    
    public void DetachItem(AttachableItem item)
    {
        if (!attachedItems.Contains(item)) return;
        
        attachedItems.Remove(item);
        
        // Remove mass
        rb.mass -= item.Mass;
        
        // Destroy joint
        FixedJoint joint = item.GetComponent<FixedJoint>();
        if (joint != null)
        {
            Destroy(joint);
        }
        
        // Unparent item
        item.transform.SetParent(null);
        
        // Mark item as detached
        item.OnDetached();
        
        // Update center of mass
        UpdateCenterOfMass();
    }
    
    private void UpdateCenterOfMass()
    {
        if (attachedItems.Count == 0)
        {
            rb.centerOfMass = Vector3.zero;
            return;
        }
        
        // Calculate weighted center of mass based on attached items
        Vector3 totalWeightedPosition = transform.position * baseMass;
        float totalMass = baseMass;
        
        foreach (AttachableItem item in attachedItems)
        {
            totalWeightedPosition += item.transform.position * item.Mass;
            totalMass += item.Mass;
        }
        
        centerOfMass = (totalWeightedPosition / totalMass) - transform.position;
        rb.centerOfMass = centerOfMass;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // Try to attach item on collision
        AttachableItem item = collision.gameObject.GetComponent<AttachableItem>();
        if (item != null && !item.IsAttached)
        {
            AttachItem(item);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw magnetic range
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, magneticRange);
        
        // Draw center of mass
        if (Application.isPlaying && rb != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + rb.centerOfMass, 0.2f);
        }
    }
}
