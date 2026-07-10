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
        GameManager.Instance.playerStats.armor += armorBonus;
        GameManager.Instance.playerStats.magnetRadiusMultiplier += magnetChange;
        GameManager.Instance.playerStats.lifestealChance += lifestealBonus;

        Debug.Log($"Efek Survival Diterapkan dari kartu: {upgradeName}");
    }
}