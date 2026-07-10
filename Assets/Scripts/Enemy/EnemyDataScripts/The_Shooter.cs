using UnityEngine;

public class The_Shooter : EnemyBase
{
    [Header("Shooting Settings")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float stopDistance = 5f;
    
    protected override void Update()
    {
        base.Update(); // Jalankan timer kebal

        if (playerTarget == null || isDead) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer > stopDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTarget.position, data.speed * Time.deltaTime);
        }
        else
        {
            if (Time.time >= nextAttackTime)
            {
                Shoot();
                nextAttackTime = Time.time + data.attackCooldown;
            }
        }
    }

    private void Shoot()
    {
        Vector2 direction = (playerTarget.position - shootPoint.position).normalized;
        GameObject bullet = Instantiate(enemyBulletPrefab, shootPoint.position, Quaternion.identity);
        
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null) bulletRb.linearVelocity = direction * 8f;

        EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
        if (bulletScript != null) bulletScript.Setdamage(data.damage);
    }
}