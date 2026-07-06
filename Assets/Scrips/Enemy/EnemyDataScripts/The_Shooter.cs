using UnityEngine;

public class The_Shooter : MonoBehaviour
{

    [Header("Shooting Settings")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float stopDistance = 5f;

    private EnemyStatus status;
    private Transform playerTarget;
    private float nextAttackTime;
    
    void Start()
    {
        status = GetComponent<EnemyStatus>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTarget = playerObj.transform;  
    }

    void Update()
    {
        if (playerTarget == null || status == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer > stopDistance)
        {
            // jika player masih terlalu jauh player akan di kejar.
            transform.position = Vector2.MoveTowards(transform.position, playerTarget.position, status.data.speed * Time.deltaTime);
        }
        else
        {
            // jika berada di area tembak, maka akan menembak.
            if (Time.time >= nextAttackTime)
            {
                Shoot();
                nextAttackTime = Time.time + status.data.attackCooldown;
            }
        }
    }

    private void Shoot()
    {
        // gunakan normalized agar konsisten
        Vector2 direction = (playerTarget.position - transform.position).normalized;

        // untuk meluncukan pleuru di posisi ShootPoint
        GameObject bullet = Instantiate(enemyBulletPrefab, shootPoint.position, Quaternion.identity);

        // mendorong peluru
        bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * 8f;

        // menghubungkan dengan fungsi damage
        EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
        if (bulletScript != null)
        {
            bulletScript.Setdamage(status.data.damage);
        }
    }
}
