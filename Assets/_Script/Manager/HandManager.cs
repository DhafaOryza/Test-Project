using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using DG.Tweening;

public class HandManager : MonoBehaviour
{
    [Header("Pooling System")]
    [SerializeField] private PoolIdSO playerCardPoolId;

    [SerializeField] private int maxHandSize;
    [SerializeField] private int maxPlaysPerRound = 3;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnpoint;

    private List<CardView> handCards = new();
    private int playsRemaining;
    public bool IsHandFull => handCards.Count >= maxHandSize;

    public System.Action<int> OnPlaysChanged; // dikirim tiap kesempatan berubah, dengar dari ChanceUI/TurnManager
    public System.Action OnPlaysExhausted;    // dikirim sekali saat kesempatan habis (giliran musuh mulai)
    public System.Action<BaseCard> OnCardSentToDiscardPile;

    private void Awake()
    {
        if (maxHandSize <= 0)
        {
            Debug.LogWarning("[HandManager] Max Hand Size belum di-set (0) di Inspector! Di-set default ke 10 sementara.");
            maxHandSize = 10;
        }
    }

    public void Initialize()
    {
        handCards.Clear();

        ResetRoundPlays();
    }

    public List<BaseCard> GetAllCardsInHand()
    {
        List<BaseCard> handData = new List<BaseCard>();
        foreach (var cardView in handCards)
        {
            if (cardView != null &&  cardView.CardData != null)
            {
                handData.Add(cardView.CardData);
            }
        }
        return handData;
    }


    public void ResetRoundPlays()
    {
        playsRemaining = maxPlaysPerRound;
        OnPlaysChanged?.Invoke(playsRemaining);

        UpdateCardsInteractability();
    }

    private void TriggerCardDraw(int amount)
    {
        if (GameManager.Instance.deckManager != null)
        {
            for (int i = 0 ; i < amount ; i++)
            {
                GameManager.Instance.deckManager.DrawCard();
            }
        }
    }

    public void AddCardToHand(BaseCard card)
    {
        if (card == null || card.Type == CardType.Enemy) return;
        
        if (handCards.Count >= maxHandSize)
        {
            Debug.Log($"[HandManager] Gagal tambah '{card.Title}' — hand sudah penuh ({handCards.Count}/{maxHandSize})");
            return;
        }

        GameObject g = GameManager.Instance.poolManager.Spawn(playerCardPoolId, spawnpoint.position, spawnpoint.rotation);

        foreach (Transform child in g.transform)
        {
            if (child != null)
            {
                child.gameObject.SetActive(true);
            }
        }
        Canvas canvas = g.GetComponentInChildren<Canvas>(true);
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
            canvas.enabled = true;
            canvas.overrideSorting = false;
        }
        CardView view = g.GetComponent<CardView>();
        view.Setup(card);
        view.SetPlayerStats(GameManager.Instance.playerStats);

        view.OnCardUsed -= HandleCardUsed;
        view.OnCardUsed += HandleCardUsed;

        view.OnCardDrawTriggered -= TriggerCardDraw;
        view.OnCardDrawTriggered += TriggerCardDraw;

        view.OnCardDiscarded -= HandleCardDiscarded;
        view.OnCardDiscarded += HandleCardDiscarded;

        if (GameManager.Instance.allyManager != null)
        {
            view.OnCardSummoned -= GameManager.Instance.allyManager.RegisterAlly;
            view.OnCardSummoned += GameManager.Instance.allyManager.RegisterAlly;
        }

        view.SetInteractable(playsRemaining > 0);
        handCards.Add(view);
        UpdateCardPositions();
    }

    private void HandleCardUsed(CardView view)
    {
        if (playsRemaining <= 0) return;
        playsRemaining--;
        bool removed = handCards.Remove(view);

        if (view.CardData.Type != CardType.Summon)
        {  
            OnCardSentToDiscardPile?.Invoke(view.CardData);

            if (GameManager.Instance.deckManager != null)
            {
                GameManager.Instance.deckManager.SpawnDiscardVisual(view.CardData, view.transform.position);
            }

            GameManager.Instance.poolManager.Despawn(playerCardPoolId, view.gameObject);
        }

        UpdateCardPositions();
        UpdateCardsInteractability();

        OnPlaysChanged?.Invoke(playsRemaining);

        if (handCards.Count == 0 && playsRemaining > 0)
        {
            ForceEndTurn();
            return;
        }

        if (playsRemaining <= 0)
            OnPlaysExhausted?.Invoke();
    }

    private void UpdateCardsInteractability()
    {
        bool canPlay = playsRemaining > 0;
        foreach (var card in handCards)
        {
            if (card != null) card.SetInteractable(canPlay);
        }
    }

    public bool HasPlaysRemaining() => playsRemaining > 0;

    private void HandleCardDiscarded(CardView view)
    {
        HandleCardUsed(view);
    }

    
    public void ForceEndTurn()
    {
        if (playsRemaining <= 0)
        {
            //Debug.Log("[HandManager] ForceEndTurn dipanggil tapi playsRemaining sudah 0, diabaikan");
            return;
        }
        playsRemaining = 0;
        //Debug.Log("[HandManager] ForceEndTurn -> OnPlaysExhausted di-invoke");
        OnPlaysChanged?.Invoke(playsRemaining);

        UpdateCardsInteractability();
        OnPlaysExhausted?.Invoke();
    }

    private void UpdateCardPositions()
    {
        handCards.RemoveAll(card => card == null);

        if (handCards.Count == 0) return;

        float cardSpacing = 1f / maxHandSize;
        float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2f;
        Spline spline = splineContainer.Spline;

        for (int i = 0; i < handCards.Count; i++)
        {
            float p = firstCardPosition + i * cardSpacing;

            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 cardForwardDir = spline.EvaluateTangent(p);
            Vector3 cardUpDir = spline.EvaluateUpVector(p);

            Quaternion rotation = Quaternion.LookRotation(cardUpDir, Vector3.Cross(cardUpDir, cardForwardDir).normalized);

            handCards[i].transform.DOMove(splinePosition, 0.25f).SetLink(handCards[i].gameObject);
            handCards[i].transform.DOLocalRotateQuaternion(rotation, 0.25f).SetLink(handCards[i].gameObject);

            // penting: update "rumah" card supaya drag-return akurat sesuai posisi terbaru di hand
            handCards[i].SetHomeTransform(splinePosition, rotation);
        }
    }

    public List<CardView> GetHandCardViews()
    {
        return handCards;
    }
}