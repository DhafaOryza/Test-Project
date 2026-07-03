using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthFillImage; // opsional, kalau Image di "StatusPlayer" pakai Fill Amount sebagai bar HP

    private void OnEnable()
    {
        playerStats.OnHealthChanged += UpdateHealthUI;
        UpdateHealthUI(playerStats.CurrentHealth, playerStats.MaxHealth); // set awal
    }

    private void OnDisable()
    {
        playerStats.OnHealthChanged -= UpdateHealthUI;
    }

    private void UpdateHealthUI(int current, int max)
    {
        if (healthText != null)
            healthText.text = $"{current}/{max}";

        if (healthFillImage != null)
            healthFillImage.fillAmount = (float)current / max;
    }
}