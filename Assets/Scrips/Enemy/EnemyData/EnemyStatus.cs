using UnityEngine;
using UnityEngine.Events;

public class EnemyStatus : MonoBehaviour
{
    [Header("Enemy Data")]
    public EnemyData data;

    [Header("Point Settings")]
    [SerializeField] private GameObject pointPrefab;

    [Header("Spawn Protection")]
    [SerializeField] private float spawnProtectionDuration = 0.5f;
    public UnityEvent onDeath;
    
    private float currentHealth;
    private bool isDead = false;
    private bool isInvulnerable = true;
    private float invulnerabilityTimer;

    private void Start()
    {
        currentHealth = data.maxHealth;
        
        // Mulai timer kebal begitu musuh muncul di layar
        invulnerabilityTimer = spawnProtectionDuration;
    }

    private void Update()
    {
        //Kurangi waktu timer jika musuh sedang dalam masa kebal
        if (isInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;
            
            // Jika waktu kebal sudah habis, matikan status kebal
            if (invulnerabilityTimer <= 0)
            {
                isInvulnerable = false;
            }
        }
    }

    public void TakeDamage(float damageAmount, Vector3 hitPoint)
    {
        //Jika enemy sudah mati atau sedang dalam masa Spawn Protection, abaikan tembakan!
        if (isDead || isInvulnerable) return; 

        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            isDead = true; 
            Die();
        }
    }

    public void Die() 
    {
        ScoreManager.Instance.AddKill();
        
        for (int i = 0; i < data.dropAmount; i++)
        {
            Instantiate(pointPrefab, transform.position, Quaternion.identity);
        }

        onDeath.Invoke();
        
        Destroy(gameObject);
    }
}