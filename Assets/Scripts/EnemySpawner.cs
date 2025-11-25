using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] private float spawnRadius = 20f;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxEnemies = 10;
    
    [Header("Difficulty")]
    [SerializeField] private float difficultyIncreaseRate = 0.1f;
    [SerializeField] private float minSpawnInterval = 1f;
    
    [Header("Spawn Area")]
    [SerializeField] private Transform player;
    [SerializeField] private float minSpawnDistance = 10f;
    
    private float nextSpawnTime = 0f;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private float gameStartTime;
    
    void Start()
    {
        gameStartTime = Time.time;
        
        if (player == null)
        {
            PlayerController playerController = FindFirstObjectByType<PlayerController>();
            if (playerController != null)
            {
                player = playerController.transform;
            }
        }
        
        // Spawn initial enemies
        for (int i = 0; i < 3; i++)
        {
            SpawnEnemy();
        }
    }
    
    void Update()
    {
        // Clean up destroyed enemies
        spawnedEnemies.RemoveAll(enemy => enemy == null);
        
        // Adjust difficulty over time
        float timeSinceStart = Time.time - gameStartTime;
        float currentSpawnInterval = Mathf.Max(
            minSpawnInterval,
            spawnInterval - (timeSinceStart * difficultyIncreaseRate)
        );
        
        // Spawn new enemies
        if (Time.time >= nextSpawnTime && spawnedEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + currentSpawnInterval;
        }
    }
    
    private void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("No enemy prefabs assigned!");
            return;
        }
        
        if (player == null)
        {
            Debug.LogWarning("No player reference found!");
            return;
        }
        
        // Choose random enemy type
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        
        // Calculate spawn position (around player but not too close)
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        // Spawn enemy
        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        spawnedEnemies.Add(spawnedEnemy);
        
        Debug.Log($"Spawned enemy at {spawnPosition}. Total enemies: {spawnedEnemies.Count}");
    }
    
    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 spawnPos = Vector3.zero;
        int attempts = 0;
        
        do
        {
            // Random position in circle around player
            Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minSpawnDistance, spawnRadius);
            spawnPos = player.position + new Vector3(randomCircle.x, 1f, randomCircle.y);
            attempts++;
            
        } while (Vector3.Distance(spawnPos, player.position) < minSpawnDistance && attempts < 10);
        
        return spawnPos;
    }
    
    public void ClearAllEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();
    }
    
    private void OnDrawGizmosSelected()
    {
        if (player == null) return;
        
        // Draw spawn radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, spawnRadius);
        
        // Draw minimum spawn distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, minSpawnDistance);
    }
}

