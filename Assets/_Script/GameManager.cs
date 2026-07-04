using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<CardData> cardDataList;
    [SerializeField] private HandManager handManager;
    [SerializeField] private int deckSize = 30; // total kartu di deck, sebelumnya cuma 10 makanya cepat habis
    private List<Card> deck;

    void Start()
    {
        BuildDeck();
    }

    private void BuildDeck()
    {
        deck = new();
        for (int i = 0; i < deckSize; i++)
        {
            CardData data = cardDataList[Random.Range(0, cardDataList.Count)];
            Card card = new Card(data);
            deck.Add(card);
        }
        //Debug.Log($"[GameManager] Deck dibuat ulang, isi: {deck.Count} kartu");
    }

    public void DrawCard()
    {
        if (deck.Count == 0)
        {
            //Debug.Log("[GameManager] Deck kosong, reshuffle otomatis");
            BuildDeck(); // reshuffle otomatis biar gak pernah kehabisan
        }

        Card drawnCard = deck[Random.Range(0, deck.Count)];
        deck.Remove(drawnCard);
        //Debug.Log($"[GameManager] Draw '{drawnCard.Title}', sisa deck: {deck.Count}");
        handManager.AddCardToHand(drawnCard);
    }
}