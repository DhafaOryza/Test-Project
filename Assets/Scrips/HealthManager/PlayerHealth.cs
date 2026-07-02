using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;

    [Header ("Health bar")]
    [SerializeField] private RawImage[] HealtImages;

    [SerializeField] private Color fullColor = Color.white;
    [SerializeField] private Color emptyColor = Color.gray;

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        updateHealthUI();
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Player HP: " + currentHealth);

        updateHealthUI();

        if (currentHealth <= 0)
        {
            Time.timeScale = 0;
        }
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