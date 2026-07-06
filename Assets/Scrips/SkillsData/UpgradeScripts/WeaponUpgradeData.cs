using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Upgrade", menuName = "Gacha Upgrades/Weapon Upgrade")]
public class WeaponUpgradeData : UpgradeData
{
    [Header("Stat Changes")]
    public float fireRateChange = 1f;
    public float reloadTimeChange = 1f;
    public int extraPierce = 0;

    public override void ApplyEffect()
    {
        PlayerStats.Instance.fireRateMultiplier += fireRateChange;
        PlayerStats.Instance.reloadMultiplier += reloadTimeChange;
        PlayerStats.Instance.bonusPierce += extraPierce;

        Debug.Log($"Efek Senjata Diterapkan dari kartu: {upgradeName}");
    }
}