using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class AllyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DropZone discardDropZone; // Tempat visual numpuk saat mati

    [Header("Pooling System")]
    [SerializeField] private PoolIdSO playerCardPoolId;

    public List<CardView> activeAllies = new List<CardView>();

    // Membuka akses list ini agar nanti bisa dibaca oleh TurnManager
    public List<CardView> ActiveAllies => activeAllies;

    // Fungsi ini dipanggil otomatis saat kartu Summon diletakkan di meja

    public void Initialize()
    {
        activeAllies.Clear();
    }
    public void RegisterAlly(CardView allyCard)
    {
        if (!activeAllies.Contains(allyCard))
        {
            activeAllies.Add(allyCard);
            allyCard.OnCardDefeated += HandleAllyDefeated; 
            //Debug.Log($"[AllyManager] {allyCard.CardData.Title} resmi bergabung ke arena!");
        }
    }

    private void HandleAllyDefeated(CardView deadAlly)
    {
        deadAlly.OnCardDefeated -= HandleAllyDefeated;
        activeAllies.Remove(deadAlly);

        if (deadAlly.CardData == null || deadAlly.CardData.Type == CardType.Enemy) return;

        if (GameManager.Instance.deckManager != null)
        {
            GameManager.Instance.deckManager.AddCardToDiscard(deadAlly.CardData);
            GameManager.Instance.deckManager.SpawnDiscardVisual(deadAlly.CardData, deadAlly.transform.position);
        }

        if (GameManager.Instance.poolManager != null)
        {
            GameManager.Instance.poolManager.Despawn(playerCardPoolId, deadAlly.gameObject);
        }
    }

    public void AllyAttackTarget(CardView allyCard, Transform targetTransform, System.Action onComplete = null)
    {
        if (allyCard == null || targetTransform == null)
        {
            onComplete?.Invoke();
            return;
        }

        Vector3 originalPos = allyCard.transform.position;
        Vector3 targetPos = new Vector3(targetTransform.position.x, targetTransform.position.y, originalPos.z);

        SpriteRenderer[] sprites = allyCard.GetComponentsInChildren<SpriteRenderer>();
        Canvas canvas = allyCard.GetComponentInChildren<Canvas>();

        foreach(var sr in sprites) sr.sortingOrder = 100;
        if (canvas != null) 
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = 101;
        }

        allyCard.transform.DOMove(targetPos, 0.25f).SetEase(Ease.InBack).SetLink(allyCard.gameObject).OnComplete(() =>
        {
            // Berikan damage tepat saat Ally menabrak Musuh
            CardView targetEnemy = targetTransform.GetComponent<CardView>();
            if (targetEnemy != null)
            {
                int dmg = 0;
                if (allyCard.CardData is SummonCard summonData)
                {
                    dmg = summonData.Damage;
                }
                targetEnemy.ReceiveDamage(dmg);
            }

            // Animasi mundur pulang ke titik di mana dia di-summon
            allyCard.transform.DOMove(originalPos, 0.3f)
                .SetEase(Ease.OutQuad)
                .SetLink(allyCard.gameObject)
                .OnComplete(() => 
                {
                    foreach(var sr in sprites) sr.sortingOrder = 1;
                    if (canvas != null) canvas.sortingOrder = 2;
                    
                    onComplete?.Invoke();
                });
        });
    }
}