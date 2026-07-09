using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; set; }

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
