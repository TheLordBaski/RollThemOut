using UnityEngine;

public abstract class AttachableItem : MonoBehaviour
{
    [Header("Item Properties")]
    [SerializeField] private string itemName;
    [SerializeField] private float mass = 0.5f;
    [SerializeField] private Vector3 attachmentOffset;
    
    [Header("Physics")]
    [SerializeField] private Rigidbody rb;
    
    [Header("Collider Settings")]
    [Tooltip("Whether to disable colliders when item is attached to player")]
    [SerializeField] private bool disableCollidersOnAttach = true;
    
    private bool isAttached = false;
    private PlayerController attachedToPlayer;
    private Collider[] itemColliders;
    
    public float Mass => mass;
    public bool IsAttached => isAttached;
    public string ItemName => itemName;
    public bool DisableCollidersOnAttach 
    { 
        get => disableCollidersOnAttach; 
        set => disableCollidersOnAttach = value; 
    }
    
    protected virtual void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        rb.mass = mass;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        // Cache colliders for toggling
        itemColliders = GetComponentsInChildren<Collider>();
    }
    
    public virtual void OnAttached(PlayerController player)
    {
        isAttached = true;
        attachedToPlayer = player;
        
        // Disable gravity for attached item
        rb.useGravity = false;
        
        // Optionally disable colliders based on setting
        if (disableCollidersOnAttach)
        {
            SetCollidersEnabled(false);
        }
        
        Debug.Log($"{itemName} attached to player!");
    }
    
    public virtual void OnDetached()
    {
        isAttached = false;
        attachedToPlayer = null;
        
        // Re-enable physics
        rb.useGravity = true;
        rb.isKinematic = false;
        
        // Re-enable colliders
        SetCollidersEnabled(true);
        
        Debug.Log($"{itemName} detached from player!");
    }
    
    /// <summary>
    /// Enable or disable all colliders on this item
    /// </summary>
    public void SetCollidersEnabled(bool enabled)
    {
        if (itemColliders == null) return;
        
        foreach (Collider col in itemColliders)
        {
            if (col != null)
            {
                col.enabled = enabled;
            }
        }
    }
    
    protected virtual void Update()
    {
        if (isAttached)
        {
            // Apply item-specific effects while attached
            ApplyAttachedEffect();
        }
    }
    
    // Override this in derived classes for item-specific behavior
    protected abstract void ApplyAttachedEffect();
}

