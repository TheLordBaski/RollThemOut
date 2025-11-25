using UnityEngine;

/// <summary>
/// Simple projectile script for weapon firing
/// Spawned by weapons and travels in a direction
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("Projectile Properties")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private bool explodeOnImpact = false;
    [SerializeField] private float explosionRadius = 2f;
    
    [Header("Visual")]
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject explosionEffectPrefab;
    
    private Rigidbody rb;
    private Vector3 direction;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }
    
    void Start()
    {
        // Auto-destroy after lifetime
        Destroy(gameObject, lifetime);
    }
    
    public void Initialize(Vector3 shootDirection, float projectileSpeed, float projectileDamage)
    {
        direction = shootDirection.normalized;
        speed = projectileSpeed;
        damage = projectileDamage;
        
        // Set velocity
        rb.linearVelocity = direction * speed;
        
        // Face direction of travel
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Don't collide with player or other projectiles
        if (collision.gameObject.CompareTag("Player") || 
            collision.gameObject.GetComponent<Projectile>() != null)
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            return;
        }
        
        // Check if hit enemy
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        
        // Explosion effect
        if (explodeOnImpact)
        {
            Explode(collision.contacts[0].point);
        }
        
        // Spawn hit effect
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
        }
        
        // Destroy projectile
        Destroy(gameObject);
    }
    
    private void Explode(Vector3 explosionPoint)
    {
        // Spawn explosion effect
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, explosionPoint, Quaternion.identity);
        }
        
        // Damage all enemies in radius
        Collider[] hitColliders = Physics.OverlapSphere(explosionPoint, explosionRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Calculate damage falloff based on distance
                float distance = Vector3.Distance(explosionPoint, hitCollider.transform.position);
                float damageFalloff = 1f - (distance / explosionRadius);
                float explosionDamage = damage * damageFalloff;
                
                enemy.TakeDamage(explosionDamage);
                
                // Apply explosion force
                Rigidbody enemyRb = hitCollider.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    Vector3 explosionDirection = (hitCollider.transform.position - explosionPoint).normalized;
                    enemyRb.AddForce(explosionDirection * 10f, ForceMode.Impulse);
                }
            }
        }
        
        Debug.Log($"Explosion at {explosionPoint} with radius {explosionRadius}!");
    }
    
    private void OnDrawGizmosSelected()
    {
        if (explodeOnImpact)
        {
            Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}

