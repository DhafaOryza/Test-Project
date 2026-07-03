using UnityEngine;
using TMPro;
using TopDown.Combat;

namespace TopDown.UI
{
    public class AmmoUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WeaponHolder weaponHolder;
        [SerializeField] private TMP_Text ammoText;

        [Header("Ammo Settings")]
        [SerializeField] private int lowAmmoThreshold = 3;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color lowColor = Color.red;

        private Guns lastWeapon; 

        private void Start()
        {
            FindWeaponHolder();
        }

        private void FindWeaponHolder()
        {
            weaponHolder = Object.FindAnyObjectByType<WeaponHolder>();

            if (weaponHolder == null)
            {
                Debug.LogWarning("AmmoUI: WeaponHolder is not found in the scene yet!");
            }
        }

        private void Update()
        {
            if (weaponHolder == null)
            {
                FindWeaponHolder();
                return;
            }

            Guns currentWeapon = weaponHolder.Currentweapon; 

            if (currentWeapon == null) 
            {
                return;
            }

            if (currentWeapon != lastWeapon)
            {
                ammoText.color = normalColor;
                lastWeapon = currentWeapon;
            }

            ammoText.text = $"{currentWeapon.CurrentAmmo}";

            if (currentWeapon.CurrentAmmo == 0)
            {
                ammoText.color = Color.red;
            }
            else if (currentWeapon.CurrentAmmo <= lowAmmoThreshold)
            {
                ammoText.color = Color.Lerp(
                    normalColor,
                    lowColor,
                    Mathf.PingPong(Time.time * 5f, 1f)
                );
            }
            else
            {
                ammoText.color = normalColor;
            }
        }
    }
}