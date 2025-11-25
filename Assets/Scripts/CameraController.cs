using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    
    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -10f);
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private bool lookAtTarget = true;
    
    [Header("Dynamic Zoom")]
    [SerializeField] private bool dynamicZoom = true;
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float zoomSpeed = 2f;
    
    private Vector3 currentOffset;
    private Vector3 velocity = Vector3.zero;
    
    void Start()
    {
        if (target == null)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                target = player.transform;
            }
        }
        
        currentOffset = offset;
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // Update offset based on player mass if dynamic zoom is enabled
        if (dynamicZoom)
        {
            UpdateDynamicZoom();
        }
        
        // Calculate desired position
        Vector3 desiredPosition = target.position + currentOffset;
        
        // Smoothly move camera to desired position
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / smoothSpeed);
        transform.position = smoothedPosition;
        
        // Look at target
        if (lookAtTarget)
        {
            transform.LookAt(target.position);
        }
    }
    
    private void UpdateDynamicZoom()
    {
        // Zoom out as player gets heavier/bigger
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        if (targetRb != null)
        {
            // Calculate zoom based on mass (assuming base mass is 1)
            float massRatio = targetRb.mass / 1f;
            float targetDistance = Mathf.Lerp(minDistance, maxDistance, Mathf.Clamp01(massRatio / 10f));
            
            // Smoothly adjust offset
            Vector3 targetOffset = offset.normalized * targetDistance;
            currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * zoomSpeed);
        }
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
        currentOffset = newOffset;
    }
}

