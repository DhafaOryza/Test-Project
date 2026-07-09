using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image levelFillBar;
    [SerializeField] private TMP_Text levelText;

    [Header("Level Settings")]
    [SerializeField] private int pointsPerLevel = 5;

    [Header("Fill Bar Animation")]
    [SerializeField] private float fillSpeed = 2f;
    private int currentLevel = 1;
    private int currentPoints = 0;
    private float targetFillAmount = 0f;
    private float displayedFillAmount = 0f;

    public void Initialize()
    {
        currentLevel = 1;
        currentPoints = 0;
        targetFillAmount = 0f;
        displayedFillAmount = 0f;

        if (levelFillBar != null)
        {
            levelFillBar.fillAmount = 0f;
        }
    }

    private void Start()
    {
        UpdateLevelText();
    }

    private void Update()
    {
        if (displayedFillAmount != targetFillAmount)
        {
            displayedFillAmount = Mathf.MoveTowards(displayedFillAmount,targetFillAmount,fillSpeed * Time.deltaTime);

            levelFillBar.fillAmount = displayedFillAmount;
        }
    }

    public void AddPoints(int amount)
    {
        currentPoints += amount;

        while (currentPoints >= pointsPerLevel)
        {
            currentPoints -= pointsPerLevel;
            currentLevel++;
            UpdateLevelText();
        
            if (GameManager.Instance.upgradeManager != null)
            {
                GameManager.Instance.upgradeManager.TriggerLevelUp();
            }

        }

        targetFillAmount = (float)currentPoints / pointsPerLevel;
    }

    private void UpdateLevelText()
    {
        levelText.text = "Level " + currentLevel;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}