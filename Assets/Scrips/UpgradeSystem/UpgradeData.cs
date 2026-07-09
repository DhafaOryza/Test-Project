using UnityEngine;

public abstract class UpgradeData : ScriptableObject
{
    public string upgradeName;
    [TextArea(2, 4)] public string description;
    public Sprite icon;

    public abstract void ApplyEffect();
}