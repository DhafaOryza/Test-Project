using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using DG.Tweening;

public class HandManager : MonoBehaviour
{
    [SerializeField] private int maxHandSize;
    [SerializeField] private int maxPlaysPerRound = 3;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnpoint;

    private List<CardView> handCards = new();
    private int playsRemaining;

    public System.Action <int> OnplaysChanged;
    public System.Action OnPlayExhausted;

    private void OnEnable() => ResetRoundPlays();

    public void ResetRoundPlays()
    {
        playsRemaining = maxPlaysPerRound;
        OnplaysChanged?.Invoke(playsRemaining);
    }

    public void AddCardToHand(Card card)
    {
        if (handCards.Count >= maxHandSize) return;

        GameObject g = Instantiate(cardPrefab, spawnpoint.position, spawnpoint.rotation);
        CardView view = g.GetComponent<CardView>();
        view.Setup(card);
        view.OnCardUsed += HandleCardUsed;

        handCards.Add(view);
        UpdateCardPositions();
    }

    private void HandleCardUsed(CardView view)
    {
        if (playsRemaining <= 0) return; // safety, seharusnya dicegah sebelum drop juga
        playsRemaining--;
        handCards.Remove(view);
        UpdateCardPositions();

        OnplaysChanged?.Invoke(playsRemaining);
        if (playsRemaining <= 0)
        {
            OnPlayExhausted?.Invoke();
        }
    }

    public bool HasPlaysRemaining() => playsRemaining > 0;

    private void UpdateCardPositions()
    {
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

            handCards[i].transform.DOMove(splinePosition, 0.25f);
            handCards[i].transform.DOLocalRotateQuaternion(rotation, 0.25f);

            handCards[i].SetHomeTransform(splinePosition, rotation);
        }
    }
}