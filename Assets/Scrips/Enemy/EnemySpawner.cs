using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Spawn Area (Around Player)")]
    [SerializeField] private Transform player;
    [SerializeField] private float minSpawnRadius = 6f;
    [SerializeField] private float maxSpawnRadius = 10f;

    [Header("Spawn Speed Settings")]
    [SerializeField] private float startingSpawnInterval = 3f;
    [SerializeField] private float minimumSpawnInterval = 0.5f;

    [Header("Difficulty Scaling")]
    [SerializeField] private float difficultyIncreaseInterval = 300f;
    [SerializeField] private float intervalReductionPerTier = 0.3f;

    [Header("References")]
    [SerializeField] private GameTimer gameTimer;

    private float currentSpawnInterval;
    private float spawnTimer;
    private int lastDifficultyTier = 0;

    private void Start()
    {
        currentSpawnInterval = startingSpawnInterval;
        spawnTimer = currentSpawnInterval;
    }

    private void Update()
    {
        if (gameTimer != null && !gameTimer.IsGameActive) return;

        CheckDifficultyIncrease();

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnOneEnemy();
            spawnTimer = currentSpawnInterval;
        }
    }

    private void CheckDifficultyIncrease()
    {
        if (gameTimer == null) return;

        int currentTier = Mathf.FloorToInt(gameTimer.CurrentTime / difficultyIncreaseInterval);

        if (currentTier > lastDifficultyTier)
        {
            lastDifficultyTier = currentTier;

            currentSpawnInterval = Mathf.Max(minimumSpawnInterval, currentSpawnInterval - intervalReductionPerTier);

            Debug.Log("Difficulty increased! Spawn interval is now: " + currentSpawnInterval + "s");
        }
    }

    private void SpawnOneEnemy()
    {
        if (enemyPrefabs.Length == 0 || player == null) return;

        GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Vector2 spawnPosition = GetRandomPositionAroundPlayer();

        Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
    }

    private Vector2 GetRandomPositionAroundPlayer()
    {
        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius);

        float radians = randomAngle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * randomDistance;

        Vector2 playerPosition = player.position;
        return playerPosition + offset;
    }
}