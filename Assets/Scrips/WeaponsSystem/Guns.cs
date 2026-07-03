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

        protected virtual void Awake()
        {
            if (data != null)
            {
                currentAmmo = data.magazineSize;
            }
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
            yield return new WaitForSeconds(data.reloadTime);
            currentAmmo = data.magazineSize;
            isReloading = false;
        }

        private IEnumerator FireCooldown()
        {
            canShoot = false;
            yield return new WaitForSeconds(data.fireRate);
            canShoot = true;
        }

        protected void SpawnBulletTrail(Vector3 hitpoint)
        {
            if (data.bulletTrailPrefab == null) return;

            GameObject trailObj = Instantiate(data.bulletTrailPrefab, gunPoint.position, Quaternion.identity);
            
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