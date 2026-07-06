using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardChoiceManager : MonoBehaviour
{
    [SerializeField] private List<CardData> possibleCardPool;
    [SerializeField] private int optionsPerRound = 3;
    [SerializeField] private int targetDeckSize = 5;
    [SerializeField] private ChoiceCardSlot[] choiceSlots;
    [SerializeField] private GameObject choicePanel;

    [Header("Rarity Weights (semakin besar = semakin sering muncul)")]
    [SerializeField] private float commonWeight = 10f;
    [SerializeField] private float uncommonWeight = 4f;
    [SerializeField] private float rareWeight = 1f;

    [Header("Anti Pengulangan")]
    [Tooltip("Berapa kartu terakhir yang tidak boleh langsung muncul lagi")]
    [SerializeField] private int noRepeatWindow = 2;

    private List<CardData> chosenDeck = new();
    private Queue<CardData> recentlyShown = new(); 
    public System.Action<List<CardData>> OnDeckReady;

    public System.Action<CardData> OnRewardCardChosen;
    private bool isDraftingInitialDeck = true;

    public void BeginCardSelection()
    {
        isDraftingInitialDeck = true;
        chosenDeck.Clear();
        recentlyShown.Clear();
        choicePanel.SetActive(true);
        ShowNewOptions();
    }

    public void BeginRewardSelection()
    {
        isDraftingInitialDeck = false;
        choicePanel.SetActive(true);
        ShowNewOptions();
    }

    private void ShowNewOptions()
    {
        List<CardData> randomPicks = GetWeightRandomCards(optionsPerRound);
        for (int i = 0; i < choiceSlots.Length; i++)
        {
            CardData data = i < randomPicks.Count ? randomPicks[i] : null;
            choiceSlots[i].Setup(data, OnCardChosen);
        }
    }

    private float GetWeight(CardData card)
    {
        return card.Rarity switch
        {
            CardRarity.common => commonWeight,
            CardRarity.uncomon => uncommonWeight,
            CardRarity.rare => rareWeight,
            _ => 1f
        };
    }

    private List<CardData> GetWeightRandomCards(int amount)
    {
       List<CardData> pool = possibleCardPool
       .Where(card => !recentlyShown.Contains(card))
       .ToList();

       if (pool.Count < amount)
        {
            pool = new List<CardData>(possibleCardPool);
        }

        List<CardData> result = new();
        List<CardData> workingPool = new List<CardData>(pool);

        for (int i = 0; i < amount && workingPool.Count > 0; i++)
        {
            CardData picked = WeightedPickOne(workingPool);
            result.Add(picked);
            workingPool.Remove(picked);
        }

        foreach (var c in result)
        {
            recentlyShown.Enqueue(c);
            if (recentlyShown.Count > noRepeatWindow * optionsPerRound)
                recentlyShown.Dequeue();
        }

        return result;
    }

    private CardData WeightedPickOne(List<CardData> pool)
    {
        float totalWeight = pool.Sum(GetWeight);
        float roll = Random.value * totalWeight;

        float cumulative = 0f;
        foreach (var card in pool)
        {
            cumulative += GetWeight(card);
            if (roll <= cumulative)
            {
                return card;
            }
        }

        return pool[pool.Count - 1];
    }

    private void OnCardChosen(CardData chosen)
    {

        if (isDraftingInitialDeck)
        {
            chosenDeck.Add(chosen);
            //Debug.Log($"[CardChoiceManager] '{chosen.Title}' dipilih. Progress deck: {chosenDeck.Count}/{targetDeckSize}");

            if (chosenDeck.Count >= targetDeckSize)
            {
                choicePanel.SetActive(false);
                OnDeckReady?.Invoke(chosenDeck);
            }
            else
            {
                ShowNewOptions();
            }
        }
        else
        {
            choicePanel.SetActive(false);
            OnRewardCardChosen?.Invoke(chosen);
        }
        
    }
}