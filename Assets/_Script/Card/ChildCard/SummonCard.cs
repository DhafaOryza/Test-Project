using UnityEngine;

public class SummonCard : BaseCard
{
    public int Damage {get; set;}
    public int currentHealth {get; set;}
    public int Maxhealth {get; set;}

    public bool IsAlive => currentHealth > 0;

    public SummonCard(CardData cardData) : base(cardData)
    {
        Damage = cardData.Damage;
        currentHealth = cardData.Health;
        Maxhealth = cardData.Health;
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
    }

    public void ResetHealth()
    {
        currentHealth = Maxhealth;
    }
    public override bool ResolveEffect(DropZone zone)
    {
        return true;
    }
} 