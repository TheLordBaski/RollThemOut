using UnityEngine;

public enum ArmorType
{
    LightArmor,      // Low mass, small defense boost
    HeavyArmor,      // High mass, large defense boost, slows player
    Spikes,          // Medium mass, damages enemies on contact
    Shield,          // Medium mass, blocks projectiles from one side
    RocketBooster    // Light mass, provides forward thrust
}

public class ArmorItem : AttachableItem
{
    [Header("Armor Properties")]
    [SerializeField] private ArmorType armorType;
    [SerializeField] private float defenseBonus = 5f;
    [SerializeField] private float speedModifier = 1f; // Multiplier for player speed
    
    [Header("Special Effects")]
    [SerializeField] private float spikesDamage = 10f;
    [SerializeField] private float boosterForce = 15f;
    
    private PlayerController player;
    private Rigidbody playerRb;
    
    public ArmorType Type => armorType;
    public float DefenseBonus => defenseBonus;
    public float SpeedModifier => speedModifier;
    
    public override void OnAttached(PlayerController attachedPlayer)
    {
        base.OnAttached(attachedPlayer);
        player = attachedPlayer;
        playerRb = attachedPlayer.GetComponent<Rigidbody>();
    }
    
    protected override void ApplyAttachedEffect()
    {
        // Apply armor-specific effects based on type
        switch (armorType)
        {
            case ArmorType.RocketBooster:
                ApplyBoosterThrust();
                break;
            case ArmorType.Spikes:
                // Spikes deal damage on contact (handled in OnCollisionEnter)
                break;
            case ArmorType.Shield:
                // Shield blocks projectiles (requires projectile system)
                break;
        }
    }
    
    private void ApplyBoosterThrust()
    {
        if (playerRb == null) return;
        
        // Calculate thrust direction based on armor's facing direction
        Vector3 thrustDirection = transform.forward;
        
        // Apply continuous thrust force
        playerRb.AddForceAtPosition(thrustDirection * boosterForce * Time.deltaTime, 
                                    transform.position, ForceMode.Force);
        
        // Visual feedback
        Debug.DrawRay(transform.position, -thrustDirection * 2f, Color.cyan);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsAttached) return;
        
        // Spikes damage enemies on contact
        if (armorType == ArmorType.Spikes)
        {
            // Check if collision is with enemy
            if (collision.gameObject.CompareTag("Enemy"))
            {
                // Apply damage to enemy
                Debug.Log($"Spikes dealt {spikesDamage} damage to {collision.gameObject.name}!");
                
                // Could implement health system here
                // EnemyHealth health = collision.gameObject.GetComponent<EnemyHealth>();
                // if (health != null) health.TakeDamage(spikesDamage);
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw armor coverage area
        Gizmos.color = new Color(0, 0, 1, 0.3f);
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Draw booster direction
        if (armorType == ArmorType.RocketBooster)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, transform.forward * 2f);
        }
        
        // Draw spike coverage
        if (armorType == ArmorType.Spikes)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.6f);
        }
    }
}

