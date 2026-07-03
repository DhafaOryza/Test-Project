using UnityEngine;
using TopDown.Combat;

public class PlayerWeaponHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Guns currentWeapon;
    [SerializeField] private Transform weaponHolder;

    void Update()
    {
        if (currentWeapon == null) return;

        RotateWeaponToMouse();

        if (currentWeapon.Data.isAutomatic)
        {
            if (Input.GetButton("Fire1"))
                currentWeapon.TryShoot();
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
                currentWeapon.TryShoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentWeapon.Reload();
        }
    }

    private void RotateWeaponToMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = (mousePos - weaponHolder.position).normalized;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        weaponHolder.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}