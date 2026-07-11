using UnityEngine;
using TMPro;
using TopDown.Combat;

namespace TopDown.UI
{
    public class AmmoUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text ammoText;
        
        // Dibuat private dan dihapus SerializeField-nya karena akan diisi oleh GameManager
        private WeaponHolder weaponHolder; 

        [Header("Ammo Settings")]
        [SerializeField] private int lowAmmoThreshold = 3;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color lowColor = Color.red;

        private Guns lastWeapon; 

        // Dipanggil oleh GameManager
        public void Initialize(WeaponHolder holder)
        {
            weaponHolder = holder;
            ammoText.color = normalColor;
            
            Debug.Log("AmmoUI berhasil diinisialisasi oleh GameManager.");
        }

        private void Update()
        {
            // Jika GameManager belum mengirimkan WeaponHolder, diam saja
            if (weaponHolder == null) return;

            Guns currentWeapon = weaponHolder.Currentweapon; 

            if (currentWeapon == null) 
            {
                ammoText.text = "-";
                return;
            }

            // Mereset warna jika pemain baru saja mengganti senjata
            if (currentWeapon != lastWeapon)
            {
                ammoText.color = normalColor;
                lastWeapon = currentWeapon;
            }

            ammoText.text = $"{currentWeapon.CurrentAmmo}";

            // Efek warna UI berdasarkan sisa peluru
            if (currentWeapon.CurrentAmmo == 0)
            {
                ammoText.color = lowColor;
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