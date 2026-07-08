using UnityEngine;
using TMPro;

public class WeaponSelectionUI : MonoBehaviour
{
    [Header("Weapon Data Resources")]
    [Tooltip("Masukkan ScriptableObject senjata ke sini sesuai urutan 0, 1, 2")]
    public WeaponData[] weaponDatas;

    [Header("UI Text References")]
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI projectilesText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI reloadTimeText;

    private void Start()
    {
        ShowWeaponStats(0);
    }

    public void ShowWeaponStats(int index)
    {
        if (weaponDatas == null || index < 0 || index >= weaponDatas.Length) 
            return;

        WeaponData data = weaponDatas[index];

        if (weaponNameText != null) 
            weaponNameText.text = $"{data.name}";
        
        if (damageText != null) 
            damageText.text = $"Damage: {data.damage.ToString()}"; 
            
        if (fireRateText != null) 
            fireRateText.text = $"FireRate: {data.fireRate.ToString()}";
            
        if (projectilesText != null) 
            projectilesText.text = $"Projectiles: {data.pelletCount.ToString("00")}";
            
        if (ammoText != null) 
            ammoText.text = $"Ammo: {data.magazineSize.ToString("00")}";
            
        if (reloadTimeText != null) 
            reloadTimeText.text = $"ReloadTime: {data.reloadTime.ToString("F1")}";
    }

    public void OnWeaponSelected(int index)
    {
        WeaponSelectionManager.Instance.SelectWeapon(index);
        ShowWeaponStats(index);
    }

    public void OnPlayButtonClicked()
    {
        WeaponSelectionManager.Instance.StartGame();
    }

    public void OnQuitButtonClicked()
    {
        WeaponSelectionManager.Instance.QuitGame();
    }
}