using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    [Header("Transition Settings")]
    [SerializeField] private float transitionTime = 2f;

    [Header("Player Data")]
    public int selectedWeaponIndex = 0; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadLevelCoroutine(sceneName));
    }

    public void RestartLevel()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadLevelCoroutine(currentScene));
    }

    private IEnumerator LoadLevelCoroutine(string sceneName)
    {
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }
}