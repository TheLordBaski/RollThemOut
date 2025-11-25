using UnityEngine;

public abstract class AttachableItem : MonoBehaviour
{
    [Header("Item Properties")]
    [SerializeField] private string itemName;
    [SerializeField] private float mass = 0.5f;
    [SerializeField] private Vector3 attachmentOffset;
    
    [Header("Physics")]
    [SerializeField] private Rigidbody rb;
    
    private bool isAttached = false;
    private PlayerController attachedToPlayer;
    
    public float Mass => mass;
    public bool IsAttached => isAttached;
    public string ItemName => itemName;
    
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
    }
    
    public virtual void OnAttached(PlayerController player)
    {
        isAttached = true;
        attachedToPlayer = player;
        
        // Disable gravity and collision for attached item
        rb.useGravity = false;
        // rb.isKinematic = true;
        
        Debug.Log($"{itemName} attached to player!");
    }
    
    public virtual void OnDetached()
    {
        isAttached = false;
        attachedToPlayer = null;
        
        // Re-enable physics
        rb.useGravity = true;
        rb.isKinematic = false;
        
        Debug.Log($"{itemName} detached from player!");
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

