using UnityEngine;
[CreateAssetMenu(fileName = "New Card", menuName = "Card System/Card Data")]
public class CardData : ScriptableObject
{
[field : SerializeField] public Sprite Sprite { get; private set; }
[field : SerializeField] public int Cost { get; private set; }
[field : SerializeField] public string Effect { get; private set; }
}