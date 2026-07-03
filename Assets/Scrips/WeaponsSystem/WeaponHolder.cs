using UnityEngine;

namespace TopDown.Combat
{
    public class WeaponHolder : MonoBehaviour
    {
        [Header("Weapon List")]
        [SerializeField] private Guns[] weapons; 

        [Header("Switch Weapon Effects")]
        [SerializeField] private float cameraShakeDuration = 0.08f;
        [SerializeField] private float cameraShakeIntensity = 0.1f;

        private int currentWeaponIndex = 0; 
        private bool isInputLocked = false; 


        public Guns Currentweapon => weapons[currentWeaponIndex];
        public int CurrentWeaponIndex => currentWeaponIndex;
        
        private void Start()
        {
            EquipWeapon(0, false);
        }

        private void Update()
        {
            if (isInputLocked) 
            {
                return;
            }

            CheckWeaponSwitchInput();
            CheckShootingInput();

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (weapons[currentWeaponIndex] != null)
                {
                    weapons[currentWeaponIndex].Reload();
                }
            }
        }

        private void CheckWeaponSwitchInput()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) 
            {
                EquipWeapon(0, true);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                EquipWeapon(1, true);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                EquipWeapon(2, true);
            }
        }

        private void CheckShootingInput()
        {
            Guns currentWeapon = weapons[currentWeaponIndex];
            if (currentWeapon == null) return;
            if (currentWeapon.isReloading) return;
            bool isAutomatic = currentWeapon.Data.isAutomatic;

            if (isAutomatic)
            {
                if (Input.GetMouseButton(0))
                {
                    currentWeapon.TryShoot();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    currentWeapon.TryShoot();
                }
            }
        }

        private void EquipWeapon(int newWeaponIndex, bool playShakeEffect)
        {
            if (newWeaponIndex < 0 || newWeaponIndex >= weapons.Length) 
            {
                return;
            }
            
            if (weapons[newWeaponIndex] == null) 
            {
                return;
            }

            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] != null)
                {
                    if (i == newWeaponIndex)
                    {
                        weapons[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        weapons[i].gameObject.SetActive(false);
                    }
                }
            }

            currentWeaponIndex = newWeaponIndex;

            if (playShakeEffect && CameraShake.Instance != null)
            {
                CameraShake.Instance.TriggerShake(cameraShakeDuration, cameraShakeIntensity);
            }
        }
    }
}