using UnityEngine;

public class PlayerPoints : MonoBehaviour
{
    public int CurrentPoints { get; private set; }

    public void AddPoint(int amount)
    {
        CurrentPoints += amount;

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.AddPoints(amount);
        }
        Debug.Log("Point : " + CurrentPoints);
    }
}