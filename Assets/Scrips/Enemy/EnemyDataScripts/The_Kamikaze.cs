using System.Collections;
using UnityEngine;

public class The_Kamikaze : MonoBehaviour
{
    [Header("Kamikaze Settings")]
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float explosionDamageToEnemies = 5f;
    [SerializeField] private float timeExplode = 1.5f;
    private bool isCountingDown = false;

    private bool hasExploded = false;

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
        if (playerTarget == null || isCountingDown || status == null) return;

        // menghitung jarak dari player dengan Vector2.Distance
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= explosionRadius)
        {
            // waktu beberapa detik sebelum meledak
            isCountingDown = true;
            StartCoroutine(ExplodeCountdown());
        }
    }

    private IEnumerator ExplodeCountdown()
    {
        yield return new WaitForSeconds(timeExplode);
        status.Die();
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Collider2D[] objectsInBlastRadius = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D obj in objectsInBlastRadius)
        {
            // ledakan bisa Melukai Player
            if (obj.CompareTag("Player"))
            {
                PlayerHealth playerHealth = obj.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(status.data.damage * 3f);
                }
            }

            // dan ledakan juga bisa Melukai Musuh Lain
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