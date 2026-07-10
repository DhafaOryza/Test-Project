using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Base Data")]
    public EnemyData data;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private float spawnProtectionDuration = 0.5f;

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

    public void Start()
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
        for (int i = 0; i < data.dropAmount; i++)
        {
            Instantiate(pointPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}