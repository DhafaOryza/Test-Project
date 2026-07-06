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

    public float CurrentTime => currentTime;
    public bool IsGameActive => isGameActive;

    void Start()
    {
        currentTime = timeLimitInMinutes * 60f; 
    }

    void Update()
    {
        if (!isGameActive) return;

        currentTime -= Time.deltaTime;

        UpdateTimerUI();

        if (currentTime <= 0)
        {
            currentTime = 0;
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