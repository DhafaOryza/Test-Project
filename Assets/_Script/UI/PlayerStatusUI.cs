using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthFillImage;

    private void OnEnable()
    {
        playerStats.OnHealthChanged += UpdateHealthUI;
    }

    private void OnDisable()
    {
        playerStats.OnHealthChanged -= UpdateHealthUI;
    }

    private void Start()
    {
        UpdateHealthUI(playerStats.CurrentHealth, playerStats.MaxHealth); // set awal
    }

    private void UpdateHealthUI(int current, int max)
    {
        if (healthText != null)
            healthText.text = $"{current}/{max}";

        if (healthFillImage != null)
            healthFillImage.fillAmount = max > 0 ? (float)current / max : 0f;
    }
}