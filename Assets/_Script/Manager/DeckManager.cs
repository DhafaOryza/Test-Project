using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class DeckManager : MonoBehaviour
{
    [Header("Deck System")]
    [Tooltip("Isi list ini dengan kartu yang dipilih player di awal game")]
    [SerializeField] private List<CardData> playerStartingDeck; 

    [Header("Pooling System")]
    [SerializeField] private PoolIdSO dummyCardPoolId;
    
    [Header("UI System")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject CancelButton;

    [Header("Reshuffle Visuals (Animasi DOTween)")]
    [SerializeField] private GameObject dummyCardPrefab;
    [SerializeField] private Transform discardPointTransform; // Posisi tempat sampah
    [SerializeField] private Transform deckSpawnPointTransform; // Posisi deck awal
    [SerializeField] private DropZone discardDropZone; // DropZone discard untuk menghapus pajangan

    [Header("Deck Showcase Settings (Game Over Custom)")]
    [SerializeField] private Transform showcaseCenterPoint;
    [SerializeField] private float maxSpreadWidth = 12f;

    private List<Card> drawPile = new();
    private List<Card> discardPile = new();
    private List<GameObject> showcasedDummyCards = new List<GameObject>();

    public int DrawPileCount => drawPile.Count;
    public int DiscardPileCount => discardPile.Count;

    public void Initialize()
    {
        GameManager.Instance.handManager.OnCardSentToDiscardPile += HandleCardDiscarded;
        GameManager.Instance.playerStats.OnPlayerDied += ShowGameOverPanel;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (CancelButton != null) CancelButton.SetActive(false);

        if ( GameManager.Instance.cardChoiceManager != null)
        {
            GameManager.Instance.cardChoiceManager.OnDeckReady += StartGameWithChosenDeck;
            GameManager.Instance.cardChoiceManager.OnRewardCardChosen += AddRewardCardToDeck;
            GameManager.Instance.cardChoiceManager.BeginCardSelection();
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
        GameManager.Instance.turnManager.BeginFirstTurn();
    }

    private void AddRewardCardToDeck(CardData rewardData)
    {
        Card newCard = new Card(rewardData);
        AddCardToDiscard(newCard);
        
        //Debug.Log($"[GameManager] Kartu baru '{rewardData.Title}' telah ditambahkan ke deck pemain!");
        if (GameManager.Instance.handManager != null)
        {
            GameManager.Instance.handManager.ResetRoundPlays();
        }

        for (int i = 0; i < 2; i++)
        {
            DrawCard();
        }
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
        if (GameManager.Instance.handManager.IsHandFull)
        {
            //Debug.Log("[GameManager] Tangan sudah penuh, batal tarik kartu dari Deck.");
            return;
        }

        if (drawPile.Count == 0)
        {
            if (discardPile.Count == 0)
            {
                //Debug.Log("[GameManager] Peringatan! Draw Pile dan Discard Pile kosong!");
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
        if (GameManager.Instance.handManager.IsHandFull)
        {
            return;
        }

        Card drawnCard = drawPile[0];
        drawPile.RemoveAt(0);
        GameManager.Instance.handManager.AddCardToHand(drawnCard);
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
            GameObject dummy = GameManager.Instance.poolManager.Spawn(dummyCardPoolId, discardPointTransform.position, discardPointTransform.rotation);

            Canvas dummyCanvas = dummy.GetComponentInChildren<Canvas>();
            if (dummyCanvas != null)
            {
                dummyCanvas.enabled = false;
            }

            CardView view = dummy.GetComponent<CardView>();
            if (view != null)
            {
                view.SetInteractable(false);
            }
            
        
            Collider2D col = dummy.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            
            float delay = i * 0.15f; 
            float MoveTime = 0.4f;

            // Terbangkan ke arah Deck dengan jeda waktu berurutan
            dummy.transform.DOMove(deckSpawnPointTransform.position, 0.4f)
                .SetDelay(delay)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => GameManager.Instance.poolManager.Despawn(dummyCardPoolId, dummy)); 
                
            
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

    public void ShowDeckShowcase()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (CancelButton != null) CancelButton.SetActive(true);

        List<CardView> activeHandCard = GameManager.Instance.handManager != null ? GameManager.Instance.handManager.GetHandCardViews() : new List<CardView>();

        int TotalCardsCount = drawPile.Count + discardPile.Count + activeHandCard.Count;
        if (TotalCardsCount == 0)
        {
            return;
        }

        float spacing = Mathf.Min(1.5f , maxSpreadWidth / Mathf.Max(1, TotalCardsCount));
        float totalWidth = (TotalCardsCount - 1) * spacing;
        float StartX = showcaseCenterPoint.position.x - (totalWidth / 2f);
        int globalIndex = 0;

        // 1. Seret visual kartu dari PLAYER DECK (Draw Pile)
        foreach (var card in drawPile)
        {
            Vector3 startPos = deckSpawnPointTransform != null ? deckSpawnPointTransform.position : showcaseCenterPoint.position;
            AnimateCardToLineup(card, startPos, StartX, spacing, globalIndex);
            globalIndex++;
        }

        // 2. Seret visual kartu dari DISCARD PILE
        foreach (var card in discardPile)
        {
            Vector3 startPos = discardPointTransform != null ? discardPointTransform.position : showcaseCenterPoint.position;
            AnimateCardToLineup(card, startPos, StartX, spacing, globalIndex);
            globalIndex++;
        }

        // 3. Seret visual kartu langsung dari TANGAN (Hand) pemain
        foreach (var handCard in activeHandCard)
        {
            if (handCard != null)
            {
                handCard.gameObject.SetActive(false); 

                Vector3 startPos = handCard.transform.position; // Titik asal dari mana dia dipegang
                AnimateCardToLineup(handCard.CardData, startPos, StartX, spacing, globalIndex);
                globalIndex++;
            }
        }
    }

    private void AnimateCardToLineup(Card cardToDisplay, Vector3 startPos, float startX, float spacing, int index)
    {
        GameObject dummy = GameManager.Instance.poolManager.Spawn(dummyCardPoolId, startPos, Quaternion.identity);
        CardView view = dummy.GetComponent<CardView>();
        if (view != null)
        {
            view.Setup(cardToDisplay);
            view.SetInteractable(false);
        }

        Vector3 targetPos = new Vector3(
            startX + (index * spacing),
            showcaseCenterPoint.position.y,
            showcaseCenterPoint.position.z - (index * 0.01f)
        );

        float delay = index * 0.05f;
        dummy.transform.DOMove(targetPos, 0.6f).SetEase(Ease.OutCubic).SetDelay(delay);
        dummy.transform.DORotate(Vector3.zero, 0.6f).SetDelay(delay);

        showcasedDummyCards.Add(dummy);
    }

    public void HideDeckShowcase()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (CancelButton != null) CancelButton.SetActive(false);

        foreach (var dummy in showcasedDummyCards)
        {
            if (dummy != null)
            {
                GameManager.Instance.poolManager.Despawn(dummyCardPoolId, dummy);
            }
        }
        showcasedDummyCards.Clear();

        if (GameManager.Instance.handManager != null)
        {
            foreach(var card in GameManager.Instance.handManager.GetHandCardViews())
            {
                if (card != null)
                {
                    card.gameObject.SetActive(true);
                }
            }
        }
    }
}