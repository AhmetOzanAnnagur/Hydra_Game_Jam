using UnityEngine;

public class EnemyAutoSpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab; // Drag your Enemy prefab here

    [Header("Spawning")]
    public int initialEnemies = 3;
    public float spawnRadius = 5f;

    void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemyAutoSpawner: No enemy prefab assigned! Please drag an Enemy prefab to the enemyPrefab field.");
            return;
        }

        Debug.Log("Auto-creating initial enemies...");

        // CREATE INITIAL ENEMIES
        for (int i = 0; i < initialEnemies; i++)
        {
            CreateEnemy();
        }
    }

    void CreateEnemy()
    {
        // POSITION AROUND SPAWNER
        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

        // INSTANTIATE ENEMY PREFAB
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemy.name = $"Enemy_{System.Guid.NewGuid().ToString().Substring(0, 8)}";

        Debug.Log($"Created enemy at {spawnPos}");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}