using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class EnemyManager : MonoBehaviour
{
    [Header("Prefabs & Data")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<CardData> possibleEnemies;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private CardChoiceManager cardChoiceManager;

    [Header("Drop Zones & Positions (Kotak Warna)")]
    [SerializeField] private DropZone enemyDropZone;
    [SerializeField] private Transform DefeatedEnemyPoint;  // Makam
    [SerializeField] private Transform ActiveEnemyPoint;    // Arena
    [SerializeField] private Transform NextEnemyPoint;      // Antrean

    [Header("Animation Settings")]
    [SerializeField] private float moveDuration = 0.5f;

    [Header("Deck & Level System")]
    [SerializeField] private int maxEnemyDeck = 10;
    [SerializeField] private float hpScalePerLevel = 0.3f;
    [SerializeField] private float damageScalePerLevel = 0.2f;
    
    [Header("Visual Tumpukan")]
    [SerializeField] private Vector3 stackOffset = new Vector3(0.1f, -0.1f, 0.1f);

    [Header("Tooltip UI")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMPro.TMP_Text tooltipText;

    private int currentLevel = 1;
    private int enemiesDefeatedCount = 0;

    // --- LIST DECK BARU ---
    private List<CardData> enemyDeck = new List<CardData>();

    private CardView activeEnemy;
    private CardView nextEnemy;
    private CardView cardInGraveyard;
    private GameObject nextDummyCard;
    private GameObject defeatedDummyCard;

    public System.Action<CardView> OnEnemyDefeated;

    // Membuka akses untuk HoverArea
    public int DefeatedCount => enemiesDefeatedCount;
    public int CardsLeft => enemyDeck.Count;
    public Transform ActiveEnemyTransform => activeEnemy != null ? activeEnemy.transform : null;

    void Start()
    {
        // 1. GENERATE DECK DI AWAL GAME
        for (int i = 0; i < maxEnemyDeck; i++)
        {
            // Pilih monster acak dari Possible Enemies
            CardData randomMonster = possibleEnemies[Random.Range(0, possibleEnemies.Count)];
            enemyDeck.Add(randomMonster); // Masukkan ke dalam tumpukan deck
        }

        if (enemyDeck.Count > 1)
        {
            nextDummyCard = CreateDummyCard(NextEnemyPoint);
        }

        activeEnemy = SpawnCard(ActiveEnemyPoint);
        ActivateEnemy(activeEnemy);

        nextEnemy = SpawnCard(NextEnemyPoint);
    }

    private GameObject CreateDummyCard(Transform basePoint)
    {
        Vector3 dummyPos = basePoint.position + stackOffset;
        GameObject dummy = Instantiate(cardPrefab, dummyPos, basePoint.rotation);
        
        Destroy(dummy.GetComponent<CardView>());
        
        Collider2D col = dummy.GetComponent<Collider2D>();
        if (col != null) Destroy(col);

        Canvas dummyCanvas = GetComponentInChildren<Canvas>();
        if (dummyCanvas != null)
        {
            dummyCanvas.gameObject.SetActive(false);
        }

        Transform imageCardObj = dummy.transform.Find("ImageCard");
        if (imageCardObj != null)
        {
            SpriteRenderer imageSprite = imageCardObj.GetComponent<SpriteRenderer>();
            if (imageSprite != null)
            {
                imageSprite.enabled = false;
            }
        }
        
        SpriteRenderer Rootsprites = dummy.GetComponent<SpriteRenderer>();
        if (Rootsprites != null)
        {
            Rootsprites.color = new Color(0.7f, 0.7f, 0.7f, 1f); 
            Rootsprites.sortingOrder = -5;
        }

        return dummy;
    }

    private CardView SpawnCard(Transform spawnPoint)
    {
        
        if (enemyDeck.Count == 0) return null;

       
        CardData data = enemyDeck[0]; 
        enemyDeck.RemoveAt(0);

        Card enemyCard = new Card(data);

        // --- SISTEM SCALING STATUS ---
        int levelMultiplier = currentLevel - 1; 
        float hpBonus = enemyCard.MaxHealth * (hpScalePerLevel * levelMultiplier);
        float dmgBonus = enemyCard.Damage * (damageScalePerLevel * levelMultiplier);

        enemyCard.MaxHealth += Mathf.RoundToInt(hpBonus);
        enemyCard.CurrentHealth = enemyCard.MaxHealth;
        enemyCard.Damage += Mathf.RoundToInt(dmgBonus);

        GameObject g = Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation);
        CardView view = g.GetComponent<CardView>();
        view.Setup(enemyCard);
        view.SetInteractable(false);

        // Matikan collider saat kartu baru spawn (masih antre)
        Collider2D col = g.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        return view;
    }

    private void ActivateEnemy(CardView view)
    {
        if (view == null) return;
        
        // Hidupkan kembali collider SAAT MASUK KOTAK MERAH agar bisa diserang
        Collider2D col = view.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        activeEnemy = view;
        enemyDropZone.EnemyCardView = view; 
        view.OnCardDefeated += HandleEnemyDefeated;
        
       // Debug.Log($"[EnemyManager] Enemy '{view.CardData.Title}' siap di Kotak Merah! HP: {view.CardData.CurrentHealth}");
    }

    private void HandleEnemyDefeated(CardView defeatedView)
    {
        //Debug.Log($"{defeatedView.CardData.Title} dikalahkan!");
        enemiesDefeatedCount++;
        currentLevel++; 

        enemyDropZone.EnemyCardView = null;
        defeatedView.OnCardDefeated -= HandleEnemyDefeated;
        activeEnemy = null;

        //Matikan collider kartu yang mati agar tidak menutupi kotak hitam
        Collider2D col = defeatedView.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (enemiesDefeatedCount > 1 && defeatedDummyCard == null)
        {
            defeatedDummyCard = CreateDummyCard(DefeatedEnemyPoint);
        }

        CardView oldCardInGraveyard = cardInGraveyard;
        cardInGraveyard = defeatedView;

        defeatedView.transform.DOMove(DefeatedEnemyPoint.position, moveDuration)
            .SetEase(Ease.InOutQuad)
            .SetLink(defeatedView.gameObject)
            .OnComplete(() => 
            {
                if (oldCardInGraveyard != null) Destroy(oldCardInGraveyard.gameObject);
            });

        if (nextEnemy == null && enemyDeck.Count <= 0)
        {
            Debug.Log("🎉 SELAMAT! KAMU MENANG!");
            return;
        }

        if (cardChoiceManager != null)
        {
            cardChoiceManager.BeginRewardSelection();
        }
        else
        {
            Debug.LogWarning("[EnemyManager] CardChoiceManager belum diisi di Inspector!");
        }

        if (nextEnemy != null)
        {
            CardView incomingEnemy = nextEnemy;
            nextEnemy = null;

            incomingEnemy.transform.DOMove(ActiveEnemyPoint.position, moveDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => ActivateEnemy(incomingEnemy));
        }

        DOVirtual.DelayedCall(moveDuration * 0.5f, () => 
        {
            nextEnemy = SpawnCard(NextEnemyPoint);
            
            if (enemyDeck.Count <= 0 && nextDummyCard != null)
            {
                Destroy(nextDummyCard);
            }
        });
        
        OnEnemyDefeated?.Invoke(defeatedView);
    }

    public void EnemyAttackTarget(Transform targetTransform, System.Action onComplete = null)
    {
        if (activeEnemy == null || targetTransform == null)
        {
            onComplete?.Invoke();
            return;
        }
        
        Vector3 originalPos = activeEnemy.transform.position;
        Vector3 targetPos = new Vector3(targetTransform.position.x , targetTransform.position.y, originalPos.z);

       
        SpriteRenderer[] sprites = activeEnemy.GetComponentsInChildren<SpriteRenderer>();
        Canvas canvas = activeEnemy.GetComponentInChildren<Canvas>();

        foreach(var sr in sprites) sr.sortingOrder = 100;
        if (canvas != null) 
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = 101;
        }
        

        activeEnemy.transform.DOMove(targetPos, 0.25f).SetEase(Ease.InBack).SetLink(activeEnemy.gameObject).OnComplete(() =>
        {
            int dmg = activeEnemy.CardData.Damage;
            
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

            activeEnemy.transform.DOMove(originalPos, 0.3f)
                .SetEase(Ease.OutQuad)
                .SetLink(activeEnemy.gameObject)
                .OnComplete(() => 
                {
                    // Kembalikan ke layer awal (0)
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