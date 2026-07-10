using UnityEngine;

namespace TopDown.Combat
{
    public class AssaultRifle : Guns
    {
        protected override void Shoot()
        {
            Vector2 origin = gunPoint.position;
            Vector2 direction = gunPoint.up;

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, data.range, hitLayers);

            Vector3 endPoint;

            if (hit.collider != null)
            {
                endPoint = hit.point;

                EnemyBase enemy = hit.collider.GetComponentInParent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.TakeDamage(data.damage, endPoint);
                }
            }
            else
            {
                endPoint = origin + direction * data.range;
            }

            SpawnBulletTrail(endPoint);
        }
    }
}