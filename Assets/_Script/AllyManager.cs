using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class AllyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager; // Untuk setor data kartu mati
    [SerializeField] private DropZone discardDropZone; // Tempat visual numpuk saat mati

    private List<CardView> activeAllies = new List<CardView>();

    // Membuka akses list ini agar nanti bisa dibaca oleh TurnManager
    public List<CardView> ActiveAllies => activeAllies;

    // Fungsi ini dipanggil otomatis saat kartu Summon diletakkan di meja
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
        //Debug.Log($"[AllyManager] {deadAlly.CardData.Title} gugur dalam pertempuran!");
        
        // 1. Lepas pendaftaran dan hapus dari daftar pasukan
        deadAlly.OnCardDefeated -= HandleAllyDefeated;
        activeAllies.Remove(deadAlly);

        // 2. Setor datanya ke GameManager agar bisa di-reshuffle nantinya
        if (gameManager != null)
        {
            gameManager.AddCardToDiscard(deadAlly.CardData);
        }

        // 3. Animasi visual membuang Ally yang mati ke tumpukan Discard
        if (discardDropZone != null)
        {
            deadAlly.SetInteractable(false);
            Collider2D col = deadAlly.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            discardDropZone.DiscardedVisuals.Add(deadAlly);

            // Matikan canvas agar numpuknya rapi (seperti trik kita sebelumnya)
            Canvas canvas = deadAlly.GetComponentInChildren<Canvas>();
            if (canvas != null) canvas.gameObject.SetActive(false);

            if (discardDropZone.DiscardedVisuals.Count > 3)
            {
                CardView oldest = discardDropZone.DiscardedVisuals[0];
                if (oldest != null) Destroy(oldest.gameObject);
                discardDropZone.DiscardedVisuals.RemoveAt(0);
            }

            int stackIndex = discardDropZone.DiscardedVisuals.Count - 1;
            Vector3 offset = new Vector3(0.05f * stackIndex, 0.05f * stackIndex, -0.1f * stackIndex);
            Vector3 targetPos = discardDropZone.transform.position + offset;

            SpriteRenderer[] sprites = deadAlly.GetComponentsInChildren<SpriteRenderer>();
            foreach(var sr in sprites) sr.sortingOrder = stackIndex;

            deadAlly.transform.DOMove(targetPos, 0.3f).SetEase(Ease.InOutQuad).SetLink(deadAlly.gameObject);
            deadAlly.transform.DOLocalRotateQuaternion(discardDropZone.transform.rotation, 0.3f).SetLink(deadAlly.gameObject);
        }
        else
        {
            Destroy(deadAlly.gameObject);
        }
    }

    // Fungsi Serangan Ally (Sistemnya 100% sama dengan serangan EnemyManager)
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
                int dmg = allyCard.CardData.Damage;
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