using UnityEngine;

public class DebuffCard : BaseCard
{
    public int Damage {get; private set;}
    public string Effect => cardData.Effect; 
    public int EffectAmount => cardData.EffectAmount;

    public DebuffCard(CardData cardData) : base(cardData)
    {
        Damage = cardData.Damage;
    }
    public override bool ResolveEffect(DropZone zone)
    {
        if (zone.EnemyCardView != null && zone.EnemyCardView.CardData is SummonCard targetEnemy)
        {
            if (targetEnemy.IsAlive)
            {
                if (Effect == "AttackDown")
                {
                    zone.EnemyCardView.ReceiveAttackDebuff(EffectAmount);
                    zone.EnemyCardView.ReceiveDamage(Damage);
                    return true;   
                }
            }
        }
        return false;
    }
}