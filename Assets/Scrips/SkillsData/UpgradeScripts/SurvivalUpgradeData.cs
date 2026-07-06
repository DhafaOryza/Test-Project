using UnityEngine;

[CreateAssetMenu(fileName = "New Survival Upgrade", menuName = "Gacha Upgrades/Survival Upgrade")]
public class SurvivalUpgradeData : UpgradeData
{
    [Header("Stat Changes")]
    public int armorBonus = 0;
    public float magnetChange = 1f;
    public float lifestealBonus = 0f;

    public override void ApplyEffect()
    {
        PlayerStats.Instance.armor += armorBonus;
        PlayerStats.Instance.magnetRadiusMultiplier += magnetChange;
        PlayerStats.Instance.lifestealChance += lifestealBonus;

        Debug.Log($"Efek Survival Diterapkan dari kartu: {upgradeName}");
    }
}