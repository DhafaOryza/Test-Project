using UnityEngine;

namespace TopDown.Combat
{
    public class WeaponHolder : MonoBehaviour
    {
        [Header("Weapon List")]
        [SerializeField] private Guns[] weapons; 

        [Header("Animation Settings")]
        [SerializeField] private PlayerAnimation playerAnim;
        [SerializeField] private RuntimeAnimatorController[] weaponAnimators;

        private int currentWeaponIndex = 0; 
        private bool isInputLocked = false; 

        public Guns Currentweapon => weapons[currentWeaponIndex];
        public int CurrentWeaponIndex => currentWeaponIndex;
        
        public void InitializeSession()
        {
            Debug.Log("WeaponHolder Initialize");
            
            int selectedWeapon = 0;

            if (GameSession.Instance != null)
            {
                selectedWeapon = GameSession.Instance.selectedWeaponIndex;
            }
            
            EquipWeapon(selectedWeapon);
        }

        private void Update()
        {
            if (isInputLocked) 
            {
                return;
            }

            RotateWeaponToMouse();
            CheckShootingInput();

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (weapons[currentWeaponIndex] != null)
                {
                    weapons[currentWeaponIndex].Reload();
                }
            }
        }
        private void RotateWeaponToMouse()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 lookDir = (mousePos - transform.position).normalized;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
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

        private void EquipWeapon(int newWeaponIndex)
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

            // untuk mengganti animasi player untuk setiap senjata
            if (playerAnim != null && weaponAnimators.Length > newWeaponIndex)
            {
                playerAnim.ChangeWeaponAnimator(weaponAnimators[newWeaponIndex]);
            }
        }
    }
}