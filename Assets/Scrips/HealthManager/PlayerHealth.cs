using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 3;

    [Header ("Health bar")]
    [SerializeField] private RawImage[] HealtImages;

    [SerializeField] private Color fullColor = Color.white;
    [SerializeField] private Color emptyColor = Color.gray;

    [Header("Death UI")]
    [SerializeField] private DeathPanelUI deathUI;
    [SerializeField] private GameTimer gameTimer;

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        updateHealthUI();
    }

    public void TakeDamage(float damageAmount)
    {
        float finalDamage = damageAmount - PlayerStats.Instance.armor;

        finalDamage = Mathf.Max(1f, finalDamage);

        currentHealth -= finalDamage;
        Debug.Log("Player HP: " + currentHealth + " (Terkena damage: " + finalDamage + ")");

        updateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // menghitung berapa lama player bertahan
        // asumsikan timelimitMinutes pada game adalah 10 menit atau 600 detik
        float totalSurvivalTime = 600f - gameTimer.CurrentTime;

        int finalKills = ScoreManager.Instance.enemiesKilled;
        int finalLevel = ScoreManager.Instance.currentLevel;
        
        deathUI.ShowDeathPanel(totalSurvivalTime, finalKills, finalLevel);
    }

    public void Heal(float healthAmount)
    {
        currentHealth += healthAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log("Player Healed! HP: " + currentHealth);
        updateHealthUI();
    }

    private void updateHealthUI()
    {
        for (int i = 0; i < HealtImages.Length; i++)
        {
            if (i < currentHealth)
            {
                HealtImages[i].color = fullColor;
            }
            else
            {
                HealtImages[i].color = emptyColor;
            }
        }
    }
}