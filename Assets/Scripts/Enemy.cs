using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float health = 50f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float moveSpeed = 2f;
    
    [Header("AI")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 1.5f;
    
    private Transform player;
    private Rigidbody rb;
    private bool isDead = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        // Find player
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            player = playerController.transform;
        }
    }
    
    void Update()
    {
        if (isDead || player == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer(distanceToPlayer);
        }
    }
    
    private void ChasePlayer(float distance)
    {
        Vector3 direction = (player.position - transform.position).normalized;
        
        if (distance > attackRange)
        {
            // Move towards player
            Vector3 moveDirection = direction;
            moveDirection.y = 0; // Keep on ground
            rb.linearVelocity = moveDirection * moveSpeed;
            
            // Face player
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
        else
        {
            // Stop and attack
            rb.linearVelocity = Vector3.zero;
            Attack();
        }
    }
    
    private void Attack()
    {
        // Simple attack - could expand with animations, cooldowns, etc.
        Debug.DrawLine(transform.position, player.position, Color.red, 0.1f);
    }
    
    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        
        Debug.Log($"{gameObject.name} took {damageAmount} damage. Health: {health}");
        
        if (health <= 0 && !isDead)
        {
            Die();
        }
    }
    
    private void Die()
    {
        isDead = true;
        
        // Award score
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(100);
        }
        
        Debug.Log($"{gameObject.name} died!");
        
        // Could add death effects, loot drops, etc.
        Destroy(gameObject, 0.5f);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // Damage player on contact
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Enemy dealt {damage} damage to player!");
            // Could implement player health system here
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

