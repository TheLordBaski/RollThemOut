using UnityEngine;

public enum WeaponType
{
    Cannon,          // Heavy, slow rate of fire, pushes player back
    MachineGun,      // Medium weight, fast fire rate, slight recoil
    RocketLauncher,  // Very heavy, propels player when firing
    Laser,           // Light, continuous beam
    Shotgun          // Medium weight, wide spread
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
    
    private float nextFireTime = 0f;
    private Transform currentTarget;
    private PlayerController player;
    
    public WeaponType Type => weaponType;
    
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
        
        // Fire at target
        if (currentTarget != null && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }
    
    private void FindTarget()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        
        float closestDistance = float.MaxValue;
        Transform closestEnemy = null;
        
        foreach (Collider enemy in enemies)
        {
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
        }
        
        // Apply recoil to player
        ApplyRecoil(-direction);
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
        
        // Draw range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
        
        // Draw line to current target
        if (currentTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }
    }
}

