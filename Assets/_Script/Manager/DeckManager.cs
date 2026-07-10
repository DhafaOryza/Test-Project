using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.VisualScripting;

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
        if(discardedCard == null || discardedCard.Type == CardType.Enemy)
        {
            Debug.LogWarning("[DeckManager] Mencegah kartu Enemy masuk ke Discard Pile!");
            return;
        }
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
                {
                    GameManager.Instance.poolManager.Despawn(dummyCardPoolId, cardVisual.gameObject);
                }
            }
            discardDropZone.DiscardedVisuals.Clear();
        }
        // ilusi 3 kartu beterbangan dari Discard ke Deck
        int dummyCount = Mathf.Min(discardedCount, 5); 
        for (int i = 0; i < dummyCount; i++)
        {
            GameObject dummy = GameManager.Instance.poolManager.Spawn(dummyCardPoolId, discardPointTransform.position, discardPointTransform.rotation);

            Canvas dummyCanvas = dummy.GetComponentInChildren<Canvas>(true);
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

    public void SpawnDiscardVisual(Card cardData, Vector3 startPos)
    {
        GameObject dummy = GameManager.Instance.poolManager.Spawn(dummyCardPoolId, startPos, Quaternion.identity);

        foreach (Transform child in dummy.transform)
        {
            if (child != null)
            {
                child.gameObject.SetActive(true);
            }
        }
        Canvas canvas = dummy.GetComponentInChildren<Canvas>(true);
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
            canvas.enabled = true;
            canvas.overrideSorting = false;
        }

        CardView view = dummy.GetComponent<CardView>();
        if (view != null)
        {
            view.Setup(cardData);
            view.SetInteractable(false);
        }

        Collider2D col = dummy.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        if (discardDropZone != null)
        {
            discardDropZone.DiscardedVisuals.Add(view);

            if (discardDropZone.DiscardedVisuals.Count > 3)
            {
                CardView oldest = discardDropZone.DiscardedVisuals[0];
                discardDropZone.DiscardedVisuals.RemoveAt(0);

                if (oldest != null)
                {
                    GameManager.Instance.poolManager.Despawn(dummyCardPoolId, oldest.gameObject);
                }
            }

            int stackIndex = discardDropZone.DiscardedVisuals.Count - 1;
            Vector3 offset = new Vector3(0.05f * stackIndex, 0.05f * stackIndex, -0.1f * stackIndex);
            Vector3 targetPos = discardDropZone.transform.position + offset;

            SpriteRenderer[] sprites = dummy.GetComponentsInChildren<SpriteRenderer>();
            foreach(var sr in sprites) sr.sortingOrder = stackIndex;

            if (canvas != null)
            {
                bool isTopCard = stackIndex == discardDropZone.DiscardedVisuals.Count - 1;
                canvas.gameObject.SetActive(isTopCard);
                canvas.enabled = isTopCard;    

                if (isTopCard)
                {
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = stackIndex + 1;
                }
                
            }

            dummy.transform.DOMove(targetPos, 0.3f).SetEase(Ease.InOutQuad);
            dummy.transform.DOLocalRotateQuaternion(discardDropZone.transform.rotation, 0.3f);

            if (discardDropZone.DiscardedVisuals.Count > 1)
            {
                CardView previousTopCard = discardDropZone.DiscardedVisuals[discardDropZone.DiscardedVisuals.Count - 2];
                Canvas prevCanvas = previousTopCard.GetComponentInChildren<Canvas>(true);
                if (prevCanvas != null)
                {
                    prevCanvas.gameObject.SetActive(false);
                    prevCanvas.enabled = false;
                }
            }
        }
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
        List<Card> allPlayerCards = new List<Card>();
        allPlayerCards.AddRange(drawPile);
        allPlayerCards.AddRange(discardPile);

        foreach (var handCard in activeHandCard)
        {
            if (handCard != null && handCard.CardData != null)
            {
                handCard.gameObject.SetActive(false);
                allPlayerCards.Add(handCard.CardData);
            }
        }

        if (allPlayerCards.Count == 0) return;

        Dictionary<string, (Card data, int count)> groupedCards = new Dictionary<string, (Card, int)>();

        foreach (Card c in allPlayerCards)
        {
            if (groupedCards.ContainsKey(c.Title))
            {
                var existing = groupedCards[c.Title];
                groupedCards[c.Title] = (existing.data , existing.count + 1);

            }
            else
            {
                groupedCards.Add(c.Title, (c , 1));
            }
        }

        int uniqueCardCount = groupedCards.Count;
        float spacing = Mathf.Min(1.5f , maxSpreadWidth / Mathf.Max(1, uniqueCardCount));
        float totalWidth = (uniqueCardCount - 1) * spacing;
        float StartX = showcaseCenterPoint.position.x - (totalWidth / 2f);

        int globalIndex = 0;

        
        foreach (var kvp in groupedCards)
        {
            Card cardData = kvp.Value.data;
            int stackCount = kvp.Value.count;

            // Titik awal terbang dari tengah layar agar menyebar elegan
            Vector3 startPos = showcaseCenterPoint.position; 
            
            AnimateGroupedCardToLineup(cardData, stackCount, startPos, StartX, spacing, globalIndex);
            globalIndex++;
        }
    }

    private void AnimateGroupedCardToLineup(Card cardToDisplay, int stackCount, Vector3 startPos, float startX, float spacing, int index)
    {
        GameObject dummy = GameManager.Instance.poolManager.Spawn(dummyCardPoolId, startPos, Quaternion.identity);
        
        foreach (Transform child in dummy.transform)
        {
            if (child != null)
            {
                child.gameObject.SetActive(true);
            }
        }
        Canvas canvas = dummy.GetComponentInChildren<Canvas>(true);
        if (canvas != null)
        {
            canvas.enabled = true;
            canvas.gameObject.SetActive(true);
            canvas.overrideSorting = false;
            canvas.sortingOrder = 0;
        }

        CardView view = dummy.GetComponent<CardView>();
        if (view != null)
        {
            view.Setup(cardToDisplay);
            view.SetInteractable(false);
            
            view.SetStackCount(stackCount);
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