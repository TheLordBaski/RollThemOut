using UnityEngine;

public enum WeaponType
{
    Cannon,          // Heavy, slow rate of fire, pushes player back
    MachineGun,      // Medium weight, fast fire rate, slight recoil
    RocketLauncher,  // Very heavy, propels player when firing
    Laser,           // Light, continuous beam
    Shotgun,         // Medium weight, wide spread
    Sword,           // Melee weapon, deals damage on contact/swing
    Axe,             // Heavy melee weapon, high damage, slow
    Spear            // Medium melee weapon, longer range
}

public class WeaponItem : AttachableItem
{
    [Header("Weapon Properties")]
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float recoilForce = 5f;
    [SerializeField] private float range = 10f;
    
    [Header("Auto-Fire")]
    [SerializeField] private bool autoFire = true;
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private LayerMask enemyLayer;
    
    [Header("Melee Settings")]
    [Tooltip("For melee weapons: angle of the swing arc in degrees")]
    [SerializeField] private float meleeSwingAngle = 90f;
    [Tooltip("For melee weapons: range of the melee attack")]
    [SerializeField] private float meleeRange = 2f;
    
    private float nextFireTime = 0f;
    private Transform currentTarget;
    private PlayerController player;
    
    public WeaponType Type => weaponType;
    
    /// <summary>
    /// Check if this weapon is a melee type
    /// </summary>
    public bool IsMeleeWeapon => weaponType == WeaponType.Sword || 
                                  weaponType == WeaponType.Axe || 
                                  weaponType == WeaponType.Spear;
    
    protected override void Awake()
    {
        base.Awake();
        
        // Melee weapons should keep colliders enabled for contact damage
        if (IsMeleeWeapon)
        {
            DisableCollidersOnAttach = false;
        }
    }
    
    public override void OnAttached(PlayerController attachedPlayer)
    {
        base.OnAttached(attachedPlayer);
        player = attachedPlayer;
    }
    
    protected override void ApplyAttachedEffect()
    {
        if (!autoFire) return;
        
        // Find nearest enemy
        FindTarget();
        
        // Fire at target (check cooldown)
        if (currentTarget != null && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }
        
        // Melee weapons also check for continuous contact damage
        if (IsMeleeWeapon)
        {
            CheckMeleeContact();
        }
    }
    
    private void FindTarget()
    {
        // For melee weapons, use melee range for detection
        float effectiveRadius = IsMeleeWeapon ? meleeRange : detectionRadius;
        
        // Use OverlapSphere - if enemyLayer is not set (value 0), find all enemies by tag
        Collider[] enemies;
        if (enemyLayer.value != 0)
        {
            enemies = Physics.OverlapSphere(transform.position, effectiveRadius, enemyLayer);
        }
        else
        {
            // Fallback: find all colliders and filter by tag
            enemies = Physics.OverlapSphere(transform.position, effectiveRadius);
        }
        
        float closestDistance = float.MaxValue;
        Transform closestEnemy = null;
        
        foreach (Collider enemy in enemies)
        {
            // Check if it's an enemy (by layer or tag)
            bool isEnemy = (enemyLayer.value != 0 && ((1 << enemy.gameObject.layer) & enemyLayer.value) != 0) ||
                          enemy.CompareTag("Enemy") ||
                          enemy.GetComponent<Enemy>() != null;
            
            if (!isEnemy) continue;
            
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }
        
        currentTarget = closestEnemy;
    }
    
    private void Fire()
    {
        if (currentTarget == null) return;
        
        Vector3 direction = (currentTarget.position - transform.position).normalized;
        
        // Apply weapon-specific behavior
        switch (weaponType)
        {
            case WeaponType.Cannon:
                FireCannon(direction);
                break;
            case WeaponType.MachineGun:
                FireMachineGun(direction);
                break;
            case WeaponType.RocketLauncher:
                FireRocket(direction);
                break;
            case WeaponType.Laser:
                FireLaser(direction);
                break;
            case WeaponType.Shotgun:
                FireShotgun(direction);
                break;
            case WeaponType.Sword:
                SwingSword(direction);
                break;
            case WeaponType.Axe:
                SwingAxe(direction);
                break;
            case WeaponType.Spear:
                ThrustSpear(direction);
                break;
        }
        
        // Apply recoil to player (reduced for melee weapons)
        if (!IsMeleeWeapon)
        {
            ApplyRecoil(-direction);
        }
    }
    
    private void FireCannon(Vector3 direction)
    {
        // Spawn heavy projectile
        Debug.DrawRay(transform.position, direction * range, Color.red, 0.5f);
        Debug.Log($"Cannon fired! Heavy impact ahead.");
    }
    
    private void FireMachineGun(Vector3 direction)
    {
        // Rapid fire projectile
        Debug.DrawRay(transform.position, direction * range, Color.yellow, 0.1f);
    }
    
    private void FireRocket(Vector3 direction)
    {
        // Rocket with explosion - propels player backward!
        Debug.DrawRay(transform.position, direction * range, Color.orange, 1f);
        Debug.Log($"Rocket launched! Player propelled backward!");
    }
    
    private void FireLaser(Vector3 direction)
    {
        // Continuous beam
        Debug.DrawRay(transform.position, direction * range, Color.cyan, 0.1f);
    }
    
    private void FireShotgun(Vector3 direction)
    {
        // Multiple pellets
        for (int i = -2; i <= 2; i++)
        {
            Vector3 spread = Quaternion.Euler(0, i * 10f, 0) * direction;
            Debug.DrawRay(transform.position, spread * range * 0.7f, Color.red, 0.3f);
        }
    }
    
    #region Melee Weapon Methods
    
    /// <summary>
    /// Swing the sword in an arc, damaging all enemies in range
    /// </summary>
    private void SwingSword(Vector3 direction)
    {
        // Perform melee attack in an arc
        PerformMeleeAttack(direction, damage, meleeRange, meleeSwingAngle);
        
        // Visual feedback - draw sword swing arc
        DrawMeleeArc(direction, meleeRange, meleeSwingAngle, Color.white);
        Debug.Log($"Sword swing! Dealing {damage} damage.");
    }
    
    /// <summary>
    /// Swing the axe - slower but higher damage
    /// </summary>
    private void SwingAxe(Vector3 direction)
    {
        // Axe deals more damage but in a narrower arc
        float axeDamage = damage * 1.5f;
        float axeArc = meleeSwingAngle * 0.7f;
        
        PerformMeleeAttack(direction, axeDamage, meleeRange, axeArc);
        
        // Visual feedback
        DrawMeleeArc(direction, meleeRange, axeArc, Color.gray);
        Debug.Log($"Axe swing! Dealing {axeDamage} damage.");
    }
    
    /// <summary>
    /// Thrust the spear - longer range but narrow attack
    /// </summary>
    private void ThrustSpear(Vector3 direction)
    {
        // Spear has longer range but narrow attack cone
        float spearRange = meleeRange * 1.5f;
        float spearArc = 30f;
        
        PerformMeleeAttack(direction, damage, spearRange, spearArc);
        
        // Visual feedback
        Debug.DrawRay(transform.position, direction * spearRange, Color.yellow, 0.3f);
        Debug.Log($"Spear thrust! Dealing {damage} damage.");
    }
    
    /// <summary>
    /// Perform a melee attack in an arc, damaging all enemies hit
    /// </summary>
    private void PerformMeleeAttack(Vector3 direction, float attackDamage, float attackRange, float attackAngle)
    {
        // Find all potential targets in range
        Collider[] potentialTargets = Physics.OverlapSphere(transform.position, attackRange);
        
        foreach (Collider col in potentialTargets)
        {
            // Check if it's an enemy
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy == null) continue;
            
            // Check if enemy is within the attack arc
            Vector3 toEnemy = (col.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(direction, toEnemy);
            
            if (angle <= attackAngle / 2f)
            {
                // Enemy is within attack arc - deal damage
                enemy.TakeDamage(attackDamage);
            }
        }
    }
    
    /// <summary>
    /// Check for melee contact damage (for rolling into enemies)
    /// </summary>
    private void CheckMeleeContact()
    {
        if (!IsAttached || player == null) return;
        
        // Get player velocity to determine if we're moving fast enough for contact damage
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb == null) return;
        
        float playerSpeed = playerRb.linearVelocity.magnitude;
        if (playerSpeed < 1f) return; // Need to be moving to deal contact damage
        
        // Contact damage is handled by OnCollisionEnter below
    }
    
    /// <summary>
    /// Draw the melee attack arc for visual feedback
    /// </summary>
    private void DrawMeleeArc(Vector3 direction, float arcRange, float arcAngle, Color color)
    {
        float halfAngle = arcAngle / 2f;
        int segments = 5;
        
        for (int i = 0; i <= segments; i++)
        {
            float angle = -halfAngle + (arcAngle * i / segments);
            Vector3 arcDirection = Quaternion.Euler(0, angle, 0) * direction;
            Debug.DrawRay(transform.position, arcDirection * arcRange, color, 0.3f);
        }
    }
    
    /// <summary>
    /// Handle collision damage for melee weapons
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsAttached || !IsMeleeWeapon) return;
        
        // Check if we hit an enemy
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Deal contact damage based on impact velocity
            float impactVelocity = collision.relativeVelocity.magnitude;
            float contactDamage = damage * Mathf.Clamp(impactVelocity / 5f, 0.5f, 2f);
            
            enemy.TakeDamage(contactDamage);
            Debug.Log($"{weaponType} contact damage: {contactDamage}");
        }
    }
    
    #endregion
    
    private void ApplyRecoil(Vector3 direction)
    {
        if (player == null) return;
        
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            // Heavier weapons have more recoil
            float effectiveRecoil = recoilForce * Mass;
            playerRb.AddForceAtPosition(direction * effectiveRecoil, transform.position, ForceMode.Impulse);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw detection radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        // Draw range (or melee range for melee weapons)
        if (IsMeleeWeapon)
        {
            // Draw melee range
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, meleeRange);
            
            // Draw attack arc representation
            Vector3 forward = transform.forward;
            float halfAngle = meleeSwingAngle / 2f;
            Vector3 leftEdge = Quaternion.Euler(0, -halfAngle, 0) * forward;
            Vector3 rightEdge = Quaternion.Euler(0, halfAngle, 0) * forward;
            
            Gizmos.color = Color.white;
            Gizmos.DrawRay(transform.position, leftEdge * meleeRange);
            Gizmos.DrawRay(transform.position, rightEdge * meleeRange);
        }
        else
        {
            // Draw projectile range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, range);
        }
        
        // Draw line to current target
        if (currentTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }
    }
}

