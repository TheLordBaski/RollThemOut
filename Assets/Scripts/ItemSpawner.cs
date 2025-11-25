using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> weaponPrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> armorPrefabs = new List<GameObject>();
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxItemsInWorld = 20;
    
    [Header("Spawn Area")]
    [SerializeField] private Vector3 spawnCenter = Vector3.zero;
    [SerializeField] private float spawnHeight = 2f;
    
    private float nextSpawnTime = 0f;
    private List<GameObject> spawnedItems = new List<GameObject>();
    
    void Start()
    {
        // Spawn initial items
        for (int i = 0; i < 5; i++)
        {
            SpawnRandomItem();
        }
    }
    
    void Update()
    {
        // Clean up destroyed items
        spawnedItems.RemoveAll(item => item == null);
        
        // Spawn new items periodically
        if (Time.time >= nextSpawnTime && spawnedItems.Count < maxItemsInWorld)
        {
            SpawnRandomItem();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }
    
    private void SpawnRandomItem()
    {
        // Choose random item type
        bool spawnWeapon = Random.value > 0.5f;
        GameObject prefabToSpawn = null;
        
        if (spawnWeapon && weaponPrefabs.Count > 0)
        {
            prefabToSpawn = weaponPrefabs[Random.Range(0, weaponPrefabs.Count)];
        }
        else if (!spawnWeapon && armorPrefabs.Count > 0)
        {
            prefabToSpawn = armorPrefabs[Random.Range(0, armorPrefabs.Count)];
        }
        
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("No prefabs available to spawn!");
            return;
        }
        
        // Calculate random spawn position
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = spawnCenter + new Vector3(randomCircle.x, spawnHeight, randomCircle.y);
        
        // Spawn item
        GameObject spawnedItem = Instantiate(prefabToSpawn, spawnPosition, Random.rotation);
        spawnedItems.Add(spawnedItem);
        
        Debug.Log($"Spawned {spawnedItem.name} at {spawnPosition}");
    }
    
    public void SpawnSpecificItem(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return;
        
        GameObject spawnedItem = Instantiate(prefab, position, Random.rotation);
        spawnedItems.Add(spawnedItem);
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw spawn radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spawnCenter, spawnRadius);
        
        // Draw spawn height plane
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawWireCube(spawnCenter + Vector3.up * spawnHeight, new Vector3(spawnRadius * 2, 0.1f, spawnRadius * 2));
    }
}

