using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathPanelUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject deathPanelObject;
    public GameObject survivePanelObject;

    public TextMeshProUGUI timeText_Label;   public TextMeshProUGUI timeText_Score;
    public TextMeshProUGUI killsText_Label;  public TextMeshProUGUI killsText_Score;
    public TextMeshProUGUI levelText_Label;  public TextMeshProUGUI levelText_Score;
    public TextMeshProUGUI totalText_Score;

    [Header("Point Multipliers")]
    public int pointsPerSecond = 1;
    public int pointsPerKill = 2;
    public int pointsPerLevel = 100;

    public void ShowDeathPanel(float timeSurvived, int enemiesKilled, int levelEarned)
    {
        if (deathPanelObject != null) deathPanelObject.SetActive(true);
        if (survivePanelObject != null) survivePanelObject.SetActive(false);

        Time.timeScale = 0f;
        CalculateAndShowScore(timeSurvived, enemiesKilled, levelEarned);
    }

    public void ShowSurvivePanel(float timeSurvived, int enemiesKilled, int levelEarned)
    {
        if (survivePanelObject != null) survivePanelObject.SetActive(true);
        if (deathPanelObject != null) deathPanelObject.SetActive(false);

        Time.timeScale = 0f;
        CalculateAndShowScore(timeSurvived, enemiesKilled, levelEarned);
    }

    public void CalculateAndShowScore(float timeSurvived, int enemiesKilled, int levelEarned)
    {
        // 1. melakukan kalkulasi dengan format (MM:SS)
        int minutes = Mathf.FloorToInt(timeSurvived / 60);
        int seconds = Mathf.FloorToInt(timeSurvived % 60);
        int timePoints = Mathf.FloorToInt(timeSurvived) * pointsPerSecond;

        int killPoints = enemiesKilled * pointsPerKill;
        int levelPoints = levelEarned * pointsPerLevel;
        int totalPoints = timePoints + killPoints + levelPoints;

        // 2. melakukan update teks Label (kiri)
        timeText_Label.text = $"Time Survived ({minutes:00}:{seconds:00})";
        killsText_Label.text = $"Enemies Killed ({enemiesKilled})";
        levelText_Label.text = $"Levels Earned ({levelEarned})";

        // 3. melakukan update text angka (kanan)
        timeText_Score.text = timePoints.ToString();
        killsText_Score.text = killPoints.ToString();
        levelText_Score.text = levelPoints.ToString();
        totalText_Score.text = totalPoints.ToString();
    }

    public void RetyGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}