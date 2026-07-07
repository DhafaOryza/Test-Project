using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float timeLimitInMinutes = 10f;

    [Header("Survive Panel")]
    [SerializeField] private DeathPanelUI survivedPanel;
    
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI timerText;

    private float currentTime = 0f;
    private float maxTimeInSeconds;
    private bool isGameActive = true;
    private bool gameEnded = false;

    public float CurrentTime => currentTime;
    public bool IsGameActive => isGameActive;

    void Start()
    {
        maxTimeInSeconds = timeLimitInMinutes * 60f;
        currentTime = maxTimeInSeconds;
    }

    void Update()
    {
        if (!isGameActive || gameEnded) return;

        currentTime -= Time.deltaTime;

        UpdateTimerUI();

        if (currentTime <= 0)
        {
            currentTime = 0;
            UpdateTimerUI();
            GameWon();
        }
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void GameWon()
    {
        isGameActive = false;

        float finalTime = maxTimeInSeconds;
        int totalKills = ScoreManager.Instance.enemiesKilled;
        int finalLevel = LevelManager.Instance.GetCurrentLevel();

        survivedPanel.ShowSurvivePanel(finalTime, totalKills, finalLevel);
    }
}