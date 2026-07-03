using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<CardData> cardDataList;
    [SerializeField] private HandManager handManager;
    private List<Card> deck;

    void Start()
    {
       deck = new();
       for (int i = 0; i < 10; i++)
        {
            CardData data = cardDataList[Random.Range(0, cardDataList.Count)];
            Card card = new Card(data);
            deck.Add(card);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DrawCard();
        }
    }

    public void DrawCard()
    {
        if (deck.Count == 0)
        return;

        Card drawnCard = deck[Random.Range(0, deck.Count)];
        deck.Remove(drawnCard);
        handManager.AddCardToHand(drawnCard);
        
    }
}