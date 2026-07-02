using UnityEngine;
using System.Collections;
using UnityEngine.Splines;
using System.Collections.Generic;
using DG.Tweening;

public class HandManager : MonoBehaviour
{
    [SerializeField] private int maxHandSize;
    [SerializeField] private GameObject CardPrefabs;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnpoint;
    private List<GameObject> handCards = new();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DrawCard();
        }
    }
    private void DrawCard()
    {
        if (handCards.Count >= maxHandSize) return;

        GameObject g = Instantiate(CardPrefabs, spawnpoint.position, spawnpoint.rotation);
        handCards.Add(g);
        UpdateCardPositions();
    }
    private void UpdateCardPositions()
    {
        if (handCards.Count == 0) return;
        float cardSpacing = 1f / maxHandSize;
        float firstCardPosition = 0.5f - (handCards.Count -1) * cardSpacing / 2f;
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
        }
    }
}
