using UnityEngine;

public class The_Kamikaze : MonoBehaviour
{
    [Header("Kamikaze Settings")]
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float explosionDamageToEnemies = 5f;
    [SerializeField] private bool isExploding = false;

    private EnemyStatus status;
    private Transform playerTarget;

    private void Awake()
    {
        status = GetComponent<EnemyStatus>();

        if (status != null)
        {
            // Mendaftarkan fungsi Explode saat musuh ini mati (karena ditembak atau nabrak player)
            status.onDeath.AddListener(Explode);
        }
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    private void Update()
    {
        if (playerTarget == null || isExploding || status == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= explosionRadius)
        {
            // Cukup panggil Die(). Otomatis akan memicu Explode() berkat fungsi di Awake()
            status.Die(); 
        }
    }

    private void Explode()
    {
        if (isExploding) return; 
        isExploding = true; 

        Collider2D[] objectsInBlastRadius = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D obj in objectsInBlastRadius)
        {
            // Melukai Player
            if (obj.CompareTag("Player"))
            {
                PlayerHealth playerHealth = obj.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(status.data.damage * 3f);
                }
            }

            // Melukai Musuh Lain
            EnemyStatus otherEnemy = obj.GetComponent<EnemyStatus>();

            if (otherEnemy != null && otherEnemy != this.status)
            {
                otherEnemy.TakeDamage(explosionDamageToEnemies, transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}