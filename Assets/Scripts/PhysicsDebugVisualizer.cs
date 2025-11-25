using UnityEngine;

/// <summary>
/// Helper script to visualize physics properties in the scene
/// Attach to any GameObject to see useful debug information
/// </summary>
public class PhysicsDebugVisualizer : MonoBehaviour
{
    [Header("Visualization Settings")]
    [SerializeField] private bool showVelocity = true;
    [SerializeField] private bool showAngularVelocity = false;
    [SerializeField] private bool showCenterOfMass = true;
    [SerializeField] private bool showForces = false;
    
    [Header("Display Settings")]
    [SerializeField] private Color velocityColor = Color.green;
    [SerializeField] private Color angularVelocityColor = Color.blue;
    [SerializeField] private Color centerOfMassColor = Color.red;
    [SerializeField] private float velocityScale = 1f;
    
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void OnDrawGizmos()
    {
        if (rb == null) return;
        
        // Show velocity
        if (showVelocity && rb.linearVelocity.magnitude > 0.1f)
        {
            Gizmos.color = velocityColor;
            Gizmos.DrawRay(transform.position, rb.linearVelocity * velocityScale);
            
            // Draw arrow head
            Vector3 arrowEnd = transform.position + rb.linearVelocity * velocityScale;
            Vector3 right = Quaternion.LookRotation(rb.linearVelocity) * Quaternion.Euler(0, 180 + 20, 0) * Vector3.forward;
            Vector3 left = Quaternion.LookRotation(rb.linearVelocity) * Quaternion.Euler(0, 180 - 20, 0) * Vector3.forward;
            Gizmos.DrawRay(arrowEnd, right * 0.3f);
            Gizmos.DrawRay(arrowEnd, left * 0.3f);
        }
        
        // Show angular velocity
        if (showAngularVelocity && rb.angularVelocity.magnitude > 0.1f)
        {
            Gizmos.color = angularVelocityColor;
            Gizmos.DrawRay(transform.position, rb.angularVelocity);
        }
        
        // Show center of mass
        if (showCenterOfMass)
        {
            Gizmos.color = centerOfMassColor;
            Vector3 worldCenterOfMass = transform.position + rb.centerOfMass;
            Gizmos.DrawSphere(worldCenterOfMass, 0.15f);
            Gizmos.DrawLine(transform.position, worldCenterOfMass);
        }
    }
    
    void OnGUI()
    {
        if (!Application.isPlaying || rb == null) return;
        
        // Display physics info in top-left corner
        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        
        int yOffset = 10;
        int lineHeight = 20;
        
        GUI.Label(new Rect(10, yOffset, 300, 20), $"Mass: {rb.mass:F2} kg", style);
        yOffset += lineHeight;
        
        GUI.Label(new Rect(10, yOffset, 300, 20), $"Velocity: {rb.linearVelocity.magnitude:F2} m/s", style);
        yOffset += lineHeight;
        
        GUI.Label(new Rect(10, yOffset, 300, 20), $"Angular Vel: {rb.angularVelocity.magnitude:F2} rad/s", style);
        yOffset += lineHeight;
        
        // Count attached items if this is the player
        PlayerController player = GetComponent<PlayerController>();
        if (player != null)
        {
            int itemCount = transform.childCount;
            GUI.Label(new Rect(10, yOffset, 300, 20), $"Attached Items: {itemCount}", style);
        }
    }
}

