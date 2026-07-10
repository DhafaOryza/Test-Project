using System.Collections;
using UnityEngine;

public class The_Kamikaze : EnemyMelee
{
    [Header("Kamikaze Settings")]
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float explosionDamageToEnemies = 5f;
    [SerializeField] private float timeExplode = 1.5f;

    private bool isCountingDown = false;
    private bool hasExploded = false;

    protected override void Update()
    {
        base.Update();

        if (playerTarget == null || isCountingDown || isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);
        if (distanceToPlayer <= explosionRadius)
        {
            isCountingDown = true;
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(ExplodeCountdown());
        }
    }

    private IEnumerator ExplodeCountdown()
    {
        yield return new WaitForSeconds(timeExplode);
        Die(); 
    }

    protected override void Die()
    {
        if (!hasExploded)
        {
            hasExploded = true;
            Explode();
        }
        
        base.Die();
    }

    private void Explode()
    {
        Collider2D[] objectsInBlastRadius = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D obj in objectsInBlastRadius)
        {
            if (obj.CompareTag("Player"))
            {
                PlayerHealth ph = obj.GetComponent<PlayerHealth>();
                if (ph != null) ph.TakeDamage(data.damage * 3f);
            }

            EnemyBase otherEnemy = obj.GetComponent<EnemyBase>();
            if (otherEnemy != null && otherEnemy != this)
            {
                otherEnemy.TakeDamage(explosionDamageToEnemies);
            }
        }
    }
}