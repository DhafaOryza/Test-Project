using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Base Data")]
    public EnemyData data;
    [SerializeField] private PoolIdSO pointPoolId;
    [SerializeField] private float spawnProtectionDuration = 0.5f;
    public PoolIdSO myPoolId;

    protected float currentHealth;
    protected bool isDead = false;
    protected bool isInvulnerable = true;
    protected float invulnerabilityTimer;

    protected Transform playerTarget;
    protected Rigidbody2D rb;
    protected float nextAttackTime = 0f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnEnable()
    {
        currentHealth = data.maxHealth;

        invulnerabilityTimer = spawnProtectionDuration;
        isDead = false;
        isInvulnerable = true;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    protected virtual void Update()
    {
        if (isInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;
            if (invulnerabilityTimer <= 0) isInvulnerable = false;
        }
    }

    public virtual void TakeDamage(float damageAmount, Vector3 hitPoint = default)
    {
        if (isDead || isInvulnerable) return;

        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }

    protected virtual void Die()
    {
        if (GameManager.Instance != null && GameManager.Instance.scoreManager != null)
        {
            GameManager.Instance.scoreManager.AddKill();
        }

        for (int i = 0; i < data.dropAmount; i++)
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.poolManager != null)
            {
                GameManager.Instance.poolManager.Spawn(
                    pointPoolId,
                    transform.position,
                    Quaternion.identity);
            }
        }
        if (GameManager.Instance != null && GameManager.Instance.poolManager != null)
        {
            GameManager.Instance.poolManager.Despawn(myPoolId, gameObject);
        }
    }
}