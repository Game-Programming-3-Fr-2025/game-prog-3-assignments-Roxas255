using UnityEngine;
using TMPro;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("Human Prefabs")]
    public Humans unarmedHumanPrefab;
    public Humans armedHumanPrefab;

    [Header("UI")]
    public TMP_Text waveText;

    [Header("Spawn Area (world bounds)")]
    public Vector2 minXY = new Vector2(-8, -4);
    public Vector2 maxXY = new Vector2(8, 4);

    [Header("Safe Spawn")]
    public float minSpawnDistanceFromPlayer = 6f;
    public int spawnTries = 25;

    [Header("Wave Settings")]
    public int baseHumans = 6;
    public int addHumansPerWave = 2;

    [Range(0f, 1f)]
    public float baseArmedChance = 0.2f;
    public float armedChanceIncreasePerWave = 0.03f;

    [Header("Projectile Scaling (armed humans only)")]
    public float baseProjectileSpeed = 12f;
    public float projectileSpeedIncrease = 0.25f;

    [Header("Timing")]
    public float timeBetweenWaves = 1.25f;

    private bool spawning = false;

    void Start()
    {
        SpawnWave(WaveState.CurrentWave);
        UpdateWaveUI();
    }

    void Update()
    {
        if (spawning) return;

        if (CountHealthyHumans() == 0)
        {
            StartCoroutine(NextWaveRoutine());
        }
    }

    IEnumerator NextWaveRoutine()
    {
        spawning = true;

        WaveState.CurrentWave++;
        UpdateWaveUI();

        yield return new WaitForSeconds(timeBetweenWaves);

        SpawnWave(WaveState.CurrentWave);

        spawning = false;
    }

    void SpawnWave(int wave)
    {
        int totalHumans = baseHumans + (wave - 1) * addHumansPerWave;

        float armedChance = Mathf.Clamp01(
            baseArmedChance + (wave - 1) * armedChanceIncreasePerWave
        );

        float projectileSpeed =
            baseProjectileSpeed + (wave - 1) * projectileSpeedIncrease;

        Vector2 playerPos = Vector2.zero;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player) playerPos = player.transform.position;

        for (int i = 0; i < totalHumans; i++)
        {
            bool spawnArmed = Random.value < armedChance;
            Humans prefabToSpawn = spawnArmed ? armedHumanPrefab : unarmedHumanPrefab;

            Vector2 spawnPos = GetSafeSpawnPosition(playerPos);

            Humans h = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

            if (spawnArmed)
            {
                h.projectileSpeed = projectileSpeed;
            }
        }
    }

    Vector2 GetSafeSpawnPosition(Vector2 playerPos)
    {
        for (int i = 0; i < spawnTries; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(minXY.x, maxXY.x),
                Random.Range(minXY.y, maxXY.y)
            );

            if (Vector2.Distance(pos, playerPos) >= minSpawnDistanceFromPlayer)
                return pos;
        }

        // fallback if no safe spot found
        return new Vector2(
            Random.Range(minXY.x, maxXY.x),
            Random.Range(minXY.y, maxXY.y)
        );
    }

    int CountHealthyHumans()
    {
        Humans[] all = FindObjectsByType<Humans>(FindObjectsSortMode.None);
        int count = 0;

        foreach (var h in all)
        {
            if (h && h.Current == Humans.State.Healthy)
                count++;
        }

        return count;
    }

    void UpdateWaveUI()
    {
        if (waveText)
            waveText.text = "Wave " + WaveState.CurrentWave;
    }
}