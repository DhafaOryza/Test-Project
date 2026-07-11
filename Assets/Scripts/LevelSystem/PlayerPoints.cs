using UnityEngine;

public class PlayerPoints : MonoBehaviour
{
    public int CurrentPoints { get; private set; }

    public void initialize()
    {
        CurrentPoints = 0;
    }

    public void AddPoint(int amount)
    {
        CurrentPoints += amount;

        if (GameManager.Instance.levelManager != null)
        {
            GameManager.Instance.levelManager.AddPoints(amount);
        }
        Debug.Log("Point : " + CurrentPoints);
    }
}