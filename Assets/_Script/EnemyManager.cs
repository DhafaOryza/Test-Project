using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab; // prefab sama dengan Card Player, punya component CardView
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private DropZone enemyDropZone; // drag GameObject "EnemyArea" di sini
    [SerializeField] private List<CardData> possibleEnemies; // CardData dengan Type = Enemy
    [SerializeField] private PlayerStats playerStats; // target yang diserang saat giliran musuh

    private CardView currentEnemy;

    public System.Action<CardView> OnEnemyDefeated;

    void Start()
    {
        SpawnRandomEnemy();
    }

    public void SpawnRandomEnemy()
    {
        if (possibleEnemies.Count == 0) return;

        CardData data = possibleEnemies[Random.Range(0, possibleEnemies.Count)];
        SpawnEnemy(data);
    }

    public void SpawnEnemy(CardData data)
    {
        Card enemyCard = new Card(data);

        GameObject g = Instantiate(cardPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);
        CardView view = g.GetComponent<CardView>();
        view.Setup(enemyCard);
        view.SetInteractable(false); // enemy tidak bisa di-drag
        view.OnCardDefeated += HandleEnemyDefeated;

        currentEnemy = view;
        enemyDropZone.EnemyCardView = view; // ini kunci: supaya kartu Attack tahu siapa yang diserang
        Debug.Log($"[EnemyManager] Enemy '{view.CardData.Title}' assigned ke zone '{enemyDropZone.name}'");
    }

    private void HandleEnemyDefeated(CardView view)
    {
        Debug.Log($"{view.CardData.Title} dikalahkan!");
        enemyDropZone.EnemyCardView = null;
        currentEnemy = null;

        OnEnemyDefeated?.Invoke(view);

        // TODO: kasih delay/animasi kalau perlu, baru spawn enemy berikutnya
        SpawnRandomEnemy();
    }

    
    public void EnemyAttackPlayer()
    {
        if (currentEnemy == null || playerStats == null) return;

        int dmg = currentEnemy.CardData.Damage;
        playerStats.TakeDamage(dmg);
        Debug.Log($"{currentEnemy.CardData.Title} menyerang Player sebesar {dmg} damage");
    }
}