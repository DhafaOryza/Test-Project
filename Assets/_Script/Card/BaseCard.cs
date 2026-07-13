using UnityEngine;

public abstract class BaseCard
{
    protected readonly CardData cardData;

    public BaseCard(CardData cardData)
    {
        this.cardData = cardData;
        Cost = cardData.Cost;
        Type = cardData.Type;
    }

    public Sprite Sprite => cardData.Sprite;
    public string Title => cardData.Title;
    public string Description => cardData.Description;
    public int Cost {get; private set;}
    public CardType Type {get; private set;}

    public abstract bool ResolveEffect(DropZone dropZone);
}