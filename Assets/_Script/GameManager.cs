using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [Header("Deck System")]
    [Tooltip("Isi list ini dengan kartu yang dipilih player di awal game")]
    [SerializeField] private List<CardData> playerStartingDeck; 
    [SerializeField] private HandManager handManager;
    [SerializeField] private CardChoiceManager cardChoiceManager;
    [SerializeField] private TurnManager turnManager;
    
    [Header("UI System")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Reshuffle Visuals (Animasi DOTween)")]
    [SerializeField] private GameObject dummyCardPrefab; // Bisa diisi dengan CardPrefab biasa
    [SerializeField] private Transform discardPointTransform; // Posisi tempat sampah
    [SerializeField] private Transform deckSpawnPointTransform; // Posisi deck awal
    [SerializeField] private DropZone discardDropZone; // DropZone discard untuk menghapus pajangan

    private List<Card> drawPile = new();
    private List<Card> discardPile = new();

    public int DrawPileCount => drawPile.Count;
    public int DiscardPileCount => discardPile.Count;

    void Start()
    {
        handManager.OnCardSentToDiscardPile += HandleCardDiscarded;
        playerStats.OnPlayerDied += ShowGameOverPanel;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (cardChoiceManager != null)
        {
            cardChoiceManager.OnDeckReady += StartGameWithChosenDeck;
            cardChoiceManager.OnRewardCardChosen += AddRewardCardToDeck;
            cardChoiceManager.BeginCardSelection();
        }
        else
        {
            BuildInitialDeck(); 
        }

        
    }

    private void StartGameWithChosenDeck(List<CardData> chosenDeck)
    {
        playerStartingDeck = chosenDeck;
        BuildInitialDeck();
        turnManager.BeginFirstTurn();
    }

    private void AddRewardCardToDeck(CardData rewardData)
    {
        Card newCard = new Card(rewardData);
        AddCardToDiscard(newCard);
        
        //Debug.Log($"[GameManager] Kartu baru '{rewardData.Title}' telah ditambahkan ke deck pemain!");
        /*
        hapus comment ketika mau reset Turn/
        
        if (handManager != null)
        {
            handManager.ResetRoundPlays();
        }*/
    }

    private void BuildInitialDeck()
    {
        drawPile.Clear();
        discardPile.Clear();

        foreach (CardData data in playerStartingDeck)
        {
            drawPile.Add(new Card(data));
        }

        ShuffleList(drawPile);
        Debug.Log($"[GameManager] Game dimulai! Isi deck: {drawPile.Count} kartu");
    }

    public void DrawCard()
    {
        if (handManager.IsHandFull)
        {
            Debug.Log("[GameManager] Tangan sudah penuh, batal tarik kartu dari Deck.");
            return;
        }

        if (drawPile.Count == 0)
        {
            if (discardPile.Count == 0)
            {
                Debug.Log("[GameManager] Peringatan! Draw Pile dan Discard Pile kosong!");
                return;
            }

            //Debug.Log($"[GameManager] Mengocok {discardPile.Count} kartu dari Discard ke Deck...");

            int discardedAmount = discardPile.Count;

            
            // Logika pindah kartu
            drawPile.AddRange(discardPile);
            discardPile.Clear();
            ShuffleList(drawPile);

            float animDuration = PlayReshuffleAnimation(discardedAmount);
            DOVirtual.DelayedCall(animDuration, () => ExecuteDraw());
        }
        else
        {
            ExecuteDraw();
        }
    }

    public void AddCardToDiscard(Card discardedCard)
    {
        discardPile.Add(discardedCard);
    }

    private void ExecuteDraw()
    {
        if (handManager.IsHandFull)
        {
            return;
        }

        Card drawnCard = drawPile[0];
        drawPile.RemoveAt(0);
        handManager.AddCardToHand(drawnCard);
    }

    private float PlayReshuffleAnimation(int discardedCount)
    {
        float totalDuration = 0f;

        if (discardDropZone != null)
        {
            foreach (var cardVisual in discardDropZone.DiscardedVisuals)
            {
                if (cardVisual != null)
                Destroy(cardVisual.gameObject);
            }
            discardDropZone.DiscardedVisuals.Clear();
        }

        // ilusi 3 kartu beterbangan dari Discard ke Deck
        int dummyCount = Mathf.Min(discardedCount, 5); 
        for (int i = 0; i < dummyCount; i++)
        {
            GameObject dummy = Instantiate(dummyCardPrefab, discardPointTransform.position, discardPointTransform.rotation);

            Canvas dummyCanvas = dummy.GetComponentInChildren<Canvas>();
            if (dummyCanvas != null)
            {
                dummyCanvas.enabled = false;
            }
            
            // Hapus komponen interaksi agar murni menjadi pajangan mati
            Destroy(dummy.GetComponent<CardView>());
            Collider2D col = dummy.GetComponent<Collider2D>();
            if (col != null) Destroy(col);

            
            float delay = i * 0.15f; 
            float MoveTime = 0.4f;

            // Terbangkan ke arah Deck dengan jeda waktu berurutan
            dummy.transform.DOMove(deckSpawnPointTransform.position, 0.4f)
                .SetDelay(delay)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => Destroy(dummy)); 
                
            
            dummy.transform.DORotate(new Vector3(0, 0, 180), 0.4f, RotateMode.FastBeyond360).SetDelay(delay);
            totalDuration = Mathf.Max(totalDuration, delay + MoveTime);
        }

        return totalDuration + 0.1f;
    }

    private void HandleCardDiscarded(Card discardedCard)
    {
        discardPile.Add(discardedCard);
    }

    private void ShuffleList(List<Card> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Card temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void ShowGameOverPanel()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowDeckViewer()
    {
        Debug.Log("Menampilkan list kartu deck...");
    }
}