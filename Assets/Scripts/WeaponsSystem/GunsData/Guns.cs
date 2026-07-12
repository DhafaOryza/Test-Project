using UnityEngine;
using System.Collections;

namespace TopDown.Combat
{
    public abstract class Guns : MonoBehaviour
    {
        [Header("Weapon Data")]
        [SerializeField] protected WeaponData data;
        [SerializeField] protected Transform gunPoint;
        [SerializeField] protected LayerMask hitLayers;

        protected int currentAmmo;
        protected bool canShoot = true;
        public bool isReloading;

        public WeaponData Data => data;
        public int CurrentAmmo => currentAmmo;
        public int MagazineSize => data.magazineSize;

        public void Initialize()
        {
            Debug.Log("Guns Initialize");
            
            if (data != null)
            {
                currentAmmo = data.magazineSize;
            }

            canShoot = true;
            isReloading = false;
        }

        public void TryShoot()
        {
            if (Time.timeScale == 0 || !canShoot || isReloading) return;

            if (currentAmmo <= 0)
            {
                Reload();
                return;
            }

            Shoot();
            currentAmmo--;

            if (currentAmmo <= 0)
            {
                Reload();    
            }

            StartCoroutine(FireCooldown());
        }

        protected abstract void Shoot();

        public void Reload()
        {
            if (isReloading || currentAmmo == data.magazineSize)
                return;

            StartCoroutine(ReloadRoutine());
        }

        private IEnumerator ReloadRoutine()
        {
            isReloading = true;

            float actualReloadTime = data.reloadTime * GameManager.Instance.playerStats.reloadMultiplier;

            yield return new WaitForSeconds(actualReloadTime);
            currentAmmo = data.magazineSize;
            isReloading = false;
        }

        private IEnumerator FireCooldown()
        {
            canShoot = false;

            float actualFireRate = data.fireRate * GameManager.Instance.playerStats.fireRateMultiplier;

            yield return new WaitForSeconds(actualFireRate);
            canShoot = true;
        }

        protected void SpawnBulletTrail(Vector3 hitpoint)
        {
            if (gunPoint == null) return;

            if (data.bulletTrailId == null) return;

            if (GameManager.Instance == null || GameManager.Instance.poolManager == null) return;

            GameObject trailObj = GameManager.Instance.poolManager.Spawn(data.bulletTrailId, gunPoint.position, Quaternion.identity);

            if (trailObj == null)
            {
                Debug.LogWarning("Trail Pool kosong!");
                return;
            }

            SpriteRenderer sr = trailObj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = "Effects";
                sr.sortingOrder = 10;
            }

            BulletTrail trail = trailObj.GetComponent<BulletTrail>();
            if (trail != null)
            {
                trail.SetTargetPosition(hitpoint);
            }
        }
    }
}