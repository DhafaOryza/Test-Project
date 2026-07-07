using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    [Header("Prefabs & Data")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<CardData> possibleEnemies;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private CardChoiceManager cardChoiceManager;

    [Header("Level / Wave System")]
    [Tooltip("Atur jumlah musuh per level. Contoh: 1, 2, 3")]
    [SerializeField] private List<int> enemiesPerLevel = new List<int>() { 1, 2, 3 };
    [SerializeField] private float hpScalePerLevel = 0.3f;
    [SerializeField] private float damageScalePerLevel = 0.2f;

    [Header("Positions (Titik Koordinat)")]
    [Tooltip("Titik slot di arena (Kiri, Tengah, Kanan)")]
    [SerializeField] private List<Transform> activeEnemyPoints;
    
    [Tooltip(" Koordinat tempat tumpukan Deck/Draw Pile musuh berada")]
    [SerializeField] private Transform enemyDeckPoint;
    
    [SerializeField] private Transform DefeatedEnemyPoint;

    [Header("Animation Settings")]
    [SerializeField] private float moveDuration = 0.6f;
    [SerializeField] private Vector3 stackOffset = new Vector3(0.1f, -0.1f, 0.1f);

    [Header("Tooltip UI")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMP_Text tooltipText;

    private int currentLevelIndex = 0;
    private int enemiesDefeatedCount = 0;
    private int enemiesLeftInCurrentWave = 0;
    private int totalEnemiesInPool = 0;

    public List<CardView> ActiveEnemies { get; private set; } = new List<CardView>();
    private List<CardView> graveyardCards = new List<CardView>();
    private List<GameObject> deckDummyCards = new List<GameObject>();
    private Queue<CardData> enemyDeckQueue = new Queue<CardData>(); 

    public System.Action<CardView> OnEnemyDefeated;

    // Akses data untuk HoverArea / Tooltip
    public int DefeatedCount => enemiesDefeatedCount;
    public int CardsLeft => totalEnemiesInPool + enemiesLeftInCurrentWave; 
    public int CurrentLevel => currentLevelIndex + 1; 

    void Start()
    {
        // Hitung total seluruh musuh di game untuk menggambar ketebalan visual deck awal
        CalculateTotalEnemies();
        UpdateDeckVisuals();
        StartWave(currentLevelIndex);
    }

    private void CalculateTotalEnemies()
    {
        totalEnemiesInPool = 0;
        enemyDeckQueue.Clear();

        for (int i = currentLevelIndex; i < enemiesPerLevel.Count; i++)
        {
            int count = enemiesPerLevel[i];
            totalEnemiesInPool += count;

            for (int j = 0; j < count ; j++)
            {
                CardData RandomMonster = possibleEnemies[Random.Range(0, possibleEnemies.Count)];
                enemyDeckQueue.Enqueue(RandomMonster);
            }
        }
    }

    // Mengupdate ketebalan visual tumpukan kartu deck musuh
    private void UpdateDeckVisuals()
    {
        foreach (var dummy in deckDummyCards) if (dummy != null) Destroy(dummy);
        deckDummyCards.Clear();

        int dummyCount = Mathf.Min(totalEnemiesInPool, 4);
        for (int i = 0; i < dummyCount; i++)
        {
            Vector3 pos = enemyDeckPoint.position + (stackOffset * i);
            GameObject dummy = Instantiate(cardPrefab, pos, enemyDeckPoint.rotation);
        
            Destroy(dummy.GetComponent<CardView>());
            Collider2D col = dummy.GetComponent<Collider2D>();
            if (col != null) Destroy(col);
            
            CardView view = dummy.GetComponent<CardView>();
            if (i == 0 && enemyDeckQueue.Count > 0)
            {
                view.Setup(new Card(enemyDeckQueue.Peek()));
            }

            foreach (Transform child in dummy.transform)
            {
                child.gameObject.SetActive(i == 0);
            }

            SpriteRenderer[] sprites = dummy.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in sprites)
            {
                sr.sortingOrder = -i;
            }

            deckDummyCards.Add(dummy);
        }
    }

    private void StartWave(int levelIndex)
    {
        if (levelIndex >= enemiesPerLevel.Count)
        {
            //Debug.Log("🎉 SELAMAT! KAMU MENANG SEMUA LEVEL!");
            return;
        }

        int enemyCountToSpawn = enemiesPerLevel[levelIndex];
        
        if (enemyCountToSpawn > activeEnemyPoints.Count) 
            enemyCountToSpawn = activeEnemyPoints.Count;

        enemiesLeftInCurrentWave = enemyCountToSpawn;
        
        // Kurangi pool utama karena pasukan wave ini akan keluar dari tumpukan deck
        totalEnemiesInPool -= enemyCountToSpawn;
        UpdateDeckVisuals();

        ActiveEnemies.Clear();

        Debug.Log($"[EnemyManager] Level {levelIndex + 1} Dimulai! Memunculkan {enemyCountToSpawn} musuh.");

        for (int i = 0; i < enemyCountToSpawn; i++)
        {
            SpawnEnemyToSlot(i, levelIndex);
        }

        UpdateDeckVisuals();
    }

    private void SpawnEnemyToSlot(int slotIndex, int levelIndex)
    {
        CardData nextMonster = enemyDeckQueue.Dequeue();
        Card enemyCard = new Card(nextMonster);

        // Status Scaling
        float hpBonus = enemyCard.MaxHealth * (hpScalePerLevel * levelIndex);
        float dmgBonus = enemyCard.Damage * (damageScalePerLevel * levelIndex);
        enemyCard.MaxHealth += Mathf.RoundToInt(hpBonus);
        enemyCard.CurrentHealth = enemyCard.MaxHealth;
        enemyCard.Damage += Mathf.RoundToInt(dmgBonus);

        GameObject g = Instantiate(cardPrefab, enemyDeckPoint.position, enemyDeckPoint.rotation);
        
        CardView view = g.GetComponent<CardView>();
        view.Setup(enemyCard);
        view.SetInteractable(false);

        Collider2D col = g.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Transform targetPoint = activeEnemyPoints[slotIndex];
        float cascadeDelay = slotIndex * 0.25f; 

        view.transform.DOMove(targetPoint.position, moveDuration)
            .SetDelay(cascadeDelay)
            .SetEase(Ease.OutBack) // Efek memantul saat mendarat di arena
            .OnComplete(() => ActivateEnemy(view));
    }

    private void ActivateEnemy(CardView view)
    {
        Collider2D col = view.GetComponent<Collider2D>();
        if (col != null) col.enabled = true; 

        ActiveEnemies.Add(view);
        view.OnCardDefeated += HandleEnemyDefeated;
    }

    private void HandleEnemyDefeated(CardView defeatedView)
    {
        enemiesDefeatedCount++;
        enemiesLeftInCurrentWave--;

        defeatedView.OnCardDefeated -= HandleEnemyDefeated;
        ActiveEnemies.Remove(defeatedView);

        Collider2D col = defeatedView.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        graveyardCards.Add(defeatedView);
        int stackIndex = graveyardCards.Count - 1;
        Vector3 targetPos = DefeatedEnemyPoint.position + (stackOffset * stackIndex);

        // Atur sistem sorting agar kartu yang baru mati berada paling depan secara visual
        Canvas canvas = defeatedView.GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = stackIndex + 2;
        }

        defeatedView.transform.DOMove(targetPos, moveDuration)
            .SetEase(Ease.InOutQuad)
            .SetLink(defeatedView.gameObject)
            .OnComplete(() => 
            {
                foreach (var graveCard in graveyardCards)
                {
                    if (graveCard != defeatedView && graveCard != null)
                    {
                        Canvas canvas = graveCard.GetComponentInChildren<Canvas>();
                        if (canvas != null)
                        {
                            canvas.gameObject.SetActive(false);
                        }
                    }
                }
                
                // Batasi penumpukan di scene (opsional, untuk mencegah lag)
                if (graveyardCards.Count > 4)
                {
                    CardView oldest = graveyardCards[0];
                    graveyardCards.RemoveAt(0);
                    if (oldest != null) Destroy(oldest.gameObject);
                }
            });

        OnEnemyDefeated?.Invoke(defeatedView);

        if (enemiesLeftInCurrentWave <= 0)
        {
            Debug.Log($"Level {currentLevelIndex + 1} Clear!");
            if (cardChoiceManager != null) cardChoiceManager.BeginRewardSelection();

            currentLevelIndex++;

            DOVirtual.DelayedCall(1.5f, () => 
            {
                StartWave(currentLevelIndex);
            });
        }
    }

    public void EnemyAttackTarget(CardView attacker, Transform targetTransform, System.Action onComplete = null)
    {
        if (attacker == null || targetTransform == null)
        {
            onComplete?.Invoke();
            return;
        }
        
        Vector3 originalPos = attacker.transform.position;
        Vector3 targetPos = new Vector3(targetTransform.position.x , targetTransform.position.y, originalPos.z);
       
        SpriteRenderer[] sprites = attacker.GetComponentsInChildren<SpriteRenderer>();
        Canvas canvas = attacker.GetComponentInChildren<Canvas>();

        foreach(var sr in sprites) sr.sortingOrder = 100;
        if (canvas != null) 
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = 101;
        }
        
        attacker.transform.DOMove(targetPos, 0.25f).SetEase(Ease.InBack).SetLink(attacker.gameObject).OnComplete(() =>
        {
            int dmg = attacker.CardData.Damage;
            
            PlayerStats targetPlayer = targetTransform.GetComponent<PlayerStats>();
            if (targetPlayer != null)
            {
                targetPlayer.TakeDamage(dmg);
            }
            else
            {
                CardView targetAlly = targetTransform.GetComponent<CardView>();
                if (targetAlly != null)
                {
                    targetAlly.ReceiveDamage(dmg);
                }
            }

            attacker.transform.DOMove(originalPos, 0.3f)
                .SetEase(Ease.OutQuad)
                .SetLink(attacker.gameObject)
                .OnComplete(() => 
                {
                    foreach(var sr in sprites) sr.sortingOrder = 0;
                    if (canvas != null) canvas.sortingOrder = 0;
                    
                    onComplete?.Invoke();
                });
        });
    }

    public void ShowTooltip(string message)
    {
        if (tooltipPanel == null || tooltipText == null) return;
        tooltipText.text = message;
        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        if (tooltipPanel == null) return;
        tooltipPanel.SetActive(false);
    }

    void Update()
    {
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            Vector3 mousePos = Input.mousePosition;
            tooltipPanel.transform.position = mousePos + new Vector3(120f, -40f, 0f); 
        }
    }
}