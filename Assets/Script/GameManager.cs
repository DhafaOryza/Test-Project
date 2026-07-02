using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<CardData> cardDataList;
    [SerializeField] private CardView cardView;
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

    public void DrawCard()
    {
        Card drawnCard = deck[Random.Range(0, deck.Count)];
        deck.Remove(drawnCard);
        CardView view = Instantiate(cardView);
        view.Setup(drawnCard);
    }
}