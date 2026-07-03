using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;

    [Header("Point Settings")]
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private int dropAmount = 1;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount, Vector3 hitPoint)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        for (int i = 0; i < dropAmount; i++)
        {
            Instantiate(pointPrefab, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }
}