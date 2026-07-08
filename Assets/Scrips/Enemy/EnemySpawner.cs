using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Enemy Progression")]
    // Index 0 (Grunt) = 0 detik
    // Index 1 (Kamikaze) = 150 detik (2.5 menit)
    // Index 2 (Splitter) = 300 detik (5 menit)
    // Index 3 (Shooter) = 450 detik (7.5 menit)
    [SerializeField] private float[] unlockTimes = {0f, 150f, 300f, 450f};

    [Header("Spawn Area (Around Player)")]
    [SerializeField] private Transform player;
    [SerializeField] private float minSpawnRadius = 6f;
    [SerializeField] private float maxSpawnRadius = 10f;

    [Header("Spawn Speed Settings")]
    [SerializeField] private float startingSpawnInterval = 3f;
    [SerializeField] private float minimumSpawnInterval = 0.5f;

    [Header("Difficulty Scaling")]
    [SerializeField] private float difficultyIncreaseInterval = 60f;
    [SerializeField] private float intervalReductionPerTier = 0.15f;

    [Header("References")]
    [SerializeField] private GameTimer gameTimer;

    private float currentSpawnInterval;
    private float spawnTimer;
    private int lastDifficultyTier = 0;
    private int availableEnemyCount = 1;

    // variabel khusus untuk spawner yang selalu menghitung maju dari nol
    private float timeAlive = 0f;

    private void Start()
    {
        currentSpawnInterval = startingSpawnInterval;
        spawnTimer = currentSpawnInterval;
    }

    private void Update()
    {
        if (gameTimer != null && !gameTimer.IsGameActive) return;

        timeAlive += Time.deltaTime;

        CheckDifficultyIncrease();
        CheckEnemyUnlocks();

        // menghitung dari belakang dari 10:00 ke 0
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnOneEnemy();
            spawnTimer = currentSpawnInterval;
        }
    }

    private void CheckEnemyUnlocks()
    {
        if (gameTimer == null || enemyPrefabs.Length == 0) return;
        
        // mengecek apakah waktu untuk enemy sudah unlock atau belum
        if (availableEnemyCount < enemyPrefabs.Length && availableEnemyCount < unlockTimes.Length)
        {
            if (timeAlive >= unlockTimes[availableEnemyCount])
            {
                availableEnemyCount++;
                Debug.Log("Enemy Bertambah : " + availableEnemyCount);
            }
        }
    }

    private void CheckDifficultyIncrease()
    {
        if (gameTimer == null) return;

        int currentTier = Mathf.FloorToInt(timeAlive / difficultyIncreaseInterval);

        if (currentTier > lastDifficultyTier)
        {
            lastDifficultyTier = currentTier;

            // mengunakan Mathf.Max agar interval tidak lebih kecil dari 0.5
            currentSpawnInterval = Mathf.Max(minimumSpawnInterval, currentSpawnInterval - intervalReductionPerTier);

            Debug.Log("Difficulty increased! Spawn interval is now: " + currentSpawnInterval + "s");
        }
    }

    private void SpawnOneEnemy()
    {
        // cek enemy yang sudah di Unlock untuk di spawn.
        if (enemyPrefabs.Length == 0 || player == null) return;

        GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, availableEnemyCount)];
        Vector2 spawnPosition = GetRandomPositionAroundPlayer();

        Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
    }

    private Vector2 GetRandomPositionAroundPlayer()
    {
        // menentukan jarak radius untuk spwan
        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius);

        // mengubah polar (sudut + jarak) menjadi koordinat kartesius (x, y)
        float radians = randomAngle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * randomDistance;

        Vector2 playerPosition = player.position;
        return playerPosition + offset;
    }
}