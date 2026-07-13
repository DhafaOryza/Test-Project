public static class CardFactory
{
    public static BaseCard CreateCard(CardData data)
    {
        if (data == null) return null;

        switch (data.Type)
        {
            case CardType.Attack:
                return new AttackCard(data);

            case CardType.Buff:
                return new BuffCard(data);

            case CardType.Debuff:
                return new DebuffCard(data);
            
            case CardType.Summon:
            case CardType.Enemy:
                return new SummonCard(data);

            default:
                throw new System.Exception($"Tipe kartu {data.Type} belum terdaftar di CardFactory!");
        }
    }
}