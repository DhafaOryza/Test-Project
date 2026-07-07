using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;

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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateLevelText();
    }

    private void Update()
    {
        if (displayedFillAmount != targetFillAmount)
        {
            displayedFillAmount = Mathf.MoveTowards(
                displayedFillAmount,
                targetFillAmount,
                fillSpeed * Time.deltaTime
            );

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
        
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.TriggerLevelUp();
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