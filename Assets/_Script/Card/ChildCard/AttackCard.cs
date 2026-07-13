using UnityEngine;

public class AttackCard : BaseCard
{
    public int Damage {get ; set;}

    public AttackCard(CardData cardData) : base(cardData)
    {
        Damage = cardData.Damage;
    }
    

    public override bool ResolveEffect(DropZone zone)
    {
        if (zone.EnemyCardView.CardData is SummonCard targetEnemy)
        {
            if (targetEnemy.IsAlive)
            {
                zone.EnemyCardView.ReceiveDamage(this.Damage);
                return true;
            }
        }
        return false;
    }
}