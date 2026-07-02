using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float attackCooldown = 2f;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    private Transform playerTarget;
    private float nextAttackTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
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
            Vector2 direction = (playerTarget.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
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
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(1);
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }
    }
}