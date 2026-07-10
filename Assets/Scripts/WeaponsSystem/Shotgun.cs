using UnityEngine;
// That namespace is for Editor tools only and should not be in gameplay scripts.

namespace TopDown.Combat
{
    public class Shotgun : Guns
    {
        protected override void Shoot()
        {
            for (int i = 0; i < data.pelletCount; i++)
            {
                float angleOffset = Random.Range(-data.spreadAngle / 2f, data.spreadAngle / 2f);
                Vector3 shootDirection = Quaternion.Euler(0, 0, angleOffset) * gunPoint.up;
                Vector2 origin = gunPoint.position;

                RaycastHit2D hit = Physics2D.Raycast(origin, shootDirection, data.range, hitLayers);

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
                    endPoint = (Vector3)origin + shootDirection * data.range;
                }

                SpawnBulletTrail(endPoint);
            }
        }
    }
}