using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Player Stats")]
    public int enemiesKilled = 0;
    public int currentLevel = 1;

    public void Initialize()
    {
        enemiesKilled = 0;
        currentLevel = 1;
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