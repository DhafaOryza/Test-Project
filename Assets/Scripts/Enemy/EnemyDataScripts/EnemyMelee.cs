using UnityEngine;

public class EnemyMelee : EnemyBase
{
    protected virtual void FixedUpdate()
    {
        if (playerTarget != null && !isDead)
        {
            Vector2 direction = (playerTarget.position - transform.position).normalized;
            rb.linearVelocity = direction * data.speed;
        }
        else if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(data.damage);
                nextAttackTime = Time.time + data.attackCooldown;
            }
        }
    }
}