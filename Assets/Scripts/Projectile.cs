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
    [Tooltip("Makes hit detection more forgiving by increasing effective collision radius")]
    [SerializeField] private float hitDetectionRadius = 0.5f;
    
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
    
    void FixedUpdate()
    {
        // Add forgiving hit detection by checking for nearby enemies
        if (hitDetectionRadius > 0)
        {
            CheckForgivingHitDetection();
        }
    }
    
    /// <summary>
    /// Check for enemies near the projectile for forgiving hit detection
    /// </summary>
    private void CheckForgivingHitDetection()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, hitDetectionRadius);
        
        foreach (Collider col in nearbyColliders)
        {
            // Skip player and other projectiles
            if (col.CompareTag("Player") || col.GetComponent<Projectile>() != null)
                continue;
            
            // Check if it's an enemy
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Hit enemy!
                HitEnemy(enemy, transform.position);
                return; // Destroy after hitting one enemy
            }
        }
    }
    
    private void HitEnemy(Enemy enemy, Vector3 hitPoint)
    {
        enemy.TakeDamage(damage);
        
        // Explosion effect
        if (explodeOnImpact)
        {
            Explode(hitPoint);
        }
        
        // Spawn hit effect
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);
        }
        
        // Destroy projectile
        Destroy(gameObject);
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
            HitEnemy(enemy, collision.contacts[0].point);
            return;
        }
        
        // Hit something else (wall, etc) - spawn effects and destroy
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
        }
        
        if (explodeOnImpact)
        {
            Explode(collision.contacts[0].point);
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

