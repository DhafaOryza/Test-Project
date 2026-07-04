using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class EnemyManager : MonoBehaviour
{
    [Header("Prefabs & Data")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<CardData> possibleEnemies;
    [SerializeField] private PlayerStats playerStats;

    [Header("Drop Zones & Positions (Kotak Warna)")]
    [SerializeField] private DropZone enemyDropZone;
    [SerializeField] private Transform DefeatedEnemyPoint;  // Posisi Kotak Hitam (Defeated)
    [SerializeField] private Transform ActiveEnemyPoint;    // Posisi Kotak Merah (Active)
    [SerializeField] private Transform NextEnemyPoint;   // Posisi Kotak Abu-abu (Next Enemy)

    [Header("Animation Settings")]
    [SerializeField] private float moveDuration = 0.5f;
    
    

    private CardView activeEnemy;
    private CardView nextEnemy;
    private CardView cardInGraveyard;

    public System.Action<CardView> OnEnemyDefeated;

    void Start()
    {
        activeEnemy = SpawnCard(ActiveEnemyPoint);
        ActivateEnemy(activeEnemy);

        nextEnemy = SpawnCard(NextEnemyPoint);
    }

    private CardView SpawnCard(Transform spawnPoint)
    {
        if (possibleEnemies.Count == 0) return null;

        CardData data = possibleEnemies[Random.Range(0, possibleEnemies.Count)];
        Card enemyCard = new Card(data);

        GameObject g = Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation);
        CardView view = g.GetComponent<CardView>();
        view.Setup(enemyCard);
        view.SetInteractable(false); // enemy tidak bisa di-drag

        return view;
    }

    private void ActivateEnemy(CardView view)
    {
        if (view == null) return;
        
        activeEnemy = view;
        enemyDropZone.EnemyCardView = view; 
        view.OnCardDefeated += HandleEnemyDefeated;
        
        Debug.Log($"[EnemyManager] Enemy '{view.CardData.Title}' siap di Kotak Merah!");
    }

    private void HandleEnemyDefeated(CardView defeatedView)
    {
        Debug.Log($"{defeatedView.CardData.Title} dikalahkan!");

        enemyDropZone.EnemyCardView = null;
        defeatedView.OnCardDefeated -= HandleEnemyDefeated;
        activeEnemy = null;

        if (cardInGraveyard != null)
        {
            Destroy(cardInGraveyard.gameObject);
        }

        cardInGraveyard = defeatedView;

        defeatedView.transform.DOMove(DefeatedEnemyPoint.position, moveDuration).SetEase(Ease.InOutQuad);
        defeatedView.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), moveDuration).SetEase(Ease.InOutQuad);

        if (nextEnemy != null)
        {
            CardView incomingEnemy = nextEnemy;
            nextEnemy = null;

            incomingEnemy.transform.DOMove(ActiveEnemyPoint.position, moveDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => 
                {
                    
                    ActivateEnemy(incomingEnemy);
                });
        }

        DOVirtual.DelayedCall(moveDuration * 0.5f, () => {nextEnemy = SpawnCard(NextEnemyPoint);});
        OnEnemyDefeated?.Invoke(defeatedView);

    }


    
    public void EnemyAttackPlayer()
    {
        if (activeEnemy == null || playerStats == null) return;

        int dmg = activeEnemy.CardData.Damage;
        playerStats.TakeDamage(dmg);
        Debug.Log($"{activeEnemy.CardData.Title} menyerang Player sebesar {dmg} damage");
    }
}