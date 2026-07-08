using System.Collections.Generic;
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
    public List<CardView> DiscardedVisuals {get; private set;} = new List<CardView>();

    void Awake()
    {
        if (ZoneType == DropZoneType.EnemyArea)
        {
            EnemyCardView.GetComponent<CardView>();
        }
    }
}