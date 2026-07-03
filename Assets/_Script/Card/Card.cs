using UnityEngine;

public class Card
{
    private readonly CardData cardData;

    public Card(CardData cardData)
    {
        this.cardData = cardData;
        Cost = cardData.Cost;
        Damage = cardData.Damage;
        CurrentHealth = cardData.Health;
    }

    public Sprite Sprite => cardData.Sprite;
    public string Title => cardData.Title;
    public string Description => cardData.Description;
    public CardType Type => cardData.Type;

    public string Effect => cardData.Effect;
    public BuffEffectType EffectType => cardData.EffectType;
    public int EffectAmount => cardData.EffectAmount;

    public int Cost { get; private set; }
    public int Damage { get; private set; }
    public int MaxHealth => cardData.Health;
    public int CurrentHealth { get; set; } // runtime, berubah kalau kena damage (buat Enemy/Summon)

    public bool IsAlive => CurrentHealth > 0;

    public void TakeDamage(int amount)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
    }
}