using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    [Header ("Statistik Player")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private TMP_Text shieldText;
    [SerializeField] private GameObject ShieldContainer;



    private void OnEnable()
    {
        playerStats.OnHealthChanged += UpdateHealthUI;

        playerStats.OnShieldChanged += UpdateShieldUI;
    }

    private void OnDisable()
    {
        playerStats.OnHealthChanged -= UpdateHealthUI;

         playerStats.OnShieldChanged -= UpdateShieldUI;
    }

    private void Start()
    {
        UpdateHealthUI(playerStats.CurrentHealth, playerStats.MaxHealth);
        UpdateShieldUI(playerStats.CurrentShield, playerStats.MaxShield);
    }

    private void UpdateHealthUI(int current, int max)
    {
        if (healthText != null)
            healthText.text = $"{current}/{max}";

        if (healthFillImage != null)
            healthFillImage.fillAmount = max > 0 ? (float)current / max : 0f;
    }

    private void UpdateShieldUI(int currentShield, int maxShield)
    {
        GameObject VisualTarget = ShieldContainer != null ? ShieldContainer : (shieldText != null ? shieldText.gameObject : null);

        if (VisualTarget == null) return;

          //Debug.Log($"[PlayerStatusUI] Menerima update Shield: {currentShield}. Mengubah visual menjadi: {currentShield > 0}");

        if (currentShield <= 0)
        {
            VisualTarget.SetActive(false);
        }
        else
        {
            VisualTarget.SetActive(true);

            if (shieldText != null)
            {
                shieldText.text = $"{currentShield}/{maxShield}";
            }
        }

    }
}