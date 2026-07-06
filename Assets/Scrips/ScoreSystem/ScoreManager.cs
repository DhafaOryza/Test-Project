using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Player Stats")]
    public int enemiesKilled = 0;
    public int currentLevel = 1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AddKill()
    {
        enemiesKilled++;
    }

    public void AddLevel()
    {
        currentLevel++;
    }
}