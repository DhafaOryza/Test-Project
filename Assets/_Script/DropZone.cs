using UnityEngine;

public enum DropZoneType {
    EnemyArea,
    PlayEffectArea,
    DiscardArea
    }

public class DropZone : MonoBehaviour
{
    [field : SerializeField] public DropZoneType ZoneType {get; private set;}
    [field : SerializeField] public CardView EnemyCardView { get;  set;}
}