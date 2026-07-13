using UnityEngine;

public class BuffCard : BaseCard
{
    public string Effect => cardData.Effect;
    public BuffEffectType EffectType => cardData.EffectType;
    public int EffectAmount => cardData.EffectAmount;

    public BuffCard(CardData cardData) : base(cardData)
    {
    }

    public override bool ResolveEffect(DropZone zone)
    {
        if (EffectType == BuffEffectType.HealthBoost)
        {
            if (GameManager.Instance.playerStats != null && !GameManager.Instance.playerStats.isHealthFull())
            {
                GameManager.Instance.playerStats.Heal(EffectAmount);
                return true;
            }
            
        }

        else if (EffectType == BuffEffectType.Shield)
        {
            GameManager.Instance.playerStats?.AddShield(EffectAmount);
            return true;
        }
        
        else if (EffectType == BuffEffectType.DrawExtraCard)
        {
            if (GameManager.Instance.deckManager)
            {
                for (int i = 0 ; i < EffectAmount ; i++)
                {
                     GameManager.Instance.deckManager.DrawCard();
                }
                return true;
            } 
        }
        return false;
    }
}