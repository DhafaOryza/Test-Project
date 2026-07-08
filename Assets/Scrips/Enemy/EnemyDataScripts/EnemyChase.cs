using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    private EnemyStatus status;
    private Transform playerTarget;
    private float nextAttackTime = 0f; 


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        status = GetComponent<EnemyStatus>();
        
        // enemy mengejar object yang memiliki tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }    
    }

    void FixedUpdate()
    {
        if (playerTarget != null)
        {
            // 
            Vector2 direction = (playerTarget.position - transform.position).normalized;
            rb.linearVelocity = direction * status.data.speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= nextAttackTime)
            {
                // jika terkena seranagn player health akan berkurang
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                
                if (playerHealth != null)
                {
                    // mengambil damage dan menunggu cooldown penyerangan
                    playerHealth.TakeDamage(status.data.damage);
                    nextAttackTime = Time.time + status.data.attackCooldown;
                }
            }
        }
    }
}