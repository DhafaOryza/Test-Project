using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float timeLimitInMinutes = 10f;
    
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI timerText;

    private float currentTime = 0f;
    private float maxTimeInSeconds;
    private bool isGameActive = true;

    void Start()
    {
        maxTimeInSeconds = timeLimitInMinutes * 60f; 
    }

    void Update()
    {
        if (!isGameActive) return;

        currentTime += Time.deltaTime;

        UpdateTimerUI();

        if (currentTime >= maxTimeInSeconds)
        {
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
    }
}