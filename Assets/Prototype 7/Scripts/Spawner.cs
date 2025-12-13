using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public BoxCollider2D boundary;      // Boundary 
    public Enemy enemyPrefab;

    [Header("Spawning")]
    public int poolSize = 100;
    public float initialSpawnInterval = 1.2f;
    public float minSpawnInterval = 0.25f;
    public float timeToMinInterval = 180f; // seconds to ramp to min
    public float safeRadiusFromPlayer = 4f; // don’t spawn right on top

    [Header("Enemy Scaling")]
    public float baseEnemySpeed = 3.5f;
    public float speedIncreaseEvery = 30f; // seconds per tier
    public float speedIncreaseAmount = 0.35f;

    float spawnTimer;
    float elapsed;
    List<Enemy> pool;
    int poolIndex;

    void Start()
    {
        if (player == null || boundary == null || enemyPrefab == null)
        {
            Debug.LogError("Spawner missing refs.");
            enabled = false;
            return;
        }

        // Build pool
        pool = new List<Enemy>(poolSize);
        for (int i = 0; i < poolSize; i++)
        {
            var e = Instantiate(enemyPrefab, transform);
            e.gameObject.SetActive(false);
            pool.Add(e);
        }
        spawnTimer = initialSpawnInterval;
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        // Current interval 
        float t = Mathf.Clamp01(elapsed / timeToMinInterval);
        float currentInterval = Mathf.Lerp(initialSpawnInterval, minSpawnInterval, t);

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            if (TryGetSpawnPoint(out Vector2 pos))
                SpawnEnemy(pos);

            spawnTimer = currentInterval;
        }
    }

    bool TryGetSpawnPoint(out Vector2 pos)
    {
        
        for (int i = 0; i < 12; i++)
        {
            Vector2 candidate = RandomPointInBoundary();
            if (Vector2.Distance(candidate, player.position) >= safeRadiusFromPlayer)
            {
                pos = candidate;
                return true;
            }
        }
        pos = (Vector2)player.position + Random.insideUnitCircle.normalized * safeRadiusFromPlayer;
        return true;
    }

    Vector2 RandomPointInBoundary()
    {
        
        Vector2 half = boundary.size * 0.5f;
        Vector2 local = new Vector2(
            Random.Range(-half.x, half.x),
            Random.Range(-half.y, half.y)
        );
        return boundary.transform.TransformPoint(local);
    }

    void SpawnEnemy(Vector2 position)
    {
        var enemy = GetFromPool();
        enemy.transform.position = position;

        // Scale speed by time
        int tiers = Mathf.FloorToInt(elapsed / speedIncreaseEvery);
        enemy.speed = baseEnemySpeed + tiers * speedIncreaseAmount;

        enemy.Init(player, boundary);
    }

    Enemy GetFromPool()
    {
        
        for (int i = 0; i < pool.Count; i++)
        {
            poolIndex = (poolIndex + 1) % pool.Count;
            if (!pool[poolIndex].gameObject.activeInHierarchy)
                return pool[poolIndex];
        }
        
        var e = Instantiate(enemyPrefab, transform);
        e.gameObject.SetActive(false);
        pool.Add(e);
        return e;
    }
}
