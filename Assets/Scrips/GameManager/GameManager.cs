using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    [Header("Managers")]
    public PoolManager poolManager;
    public UpgradeManager upgradeManager;
    public LevelManager levelManager;

    private bool hasInitialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (hasInitialized) return;
        UpdateStageData();
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hasInitialized = false;
        UpdateStageData();
    }

    private void UpdateStageData()
    {
        if (hasInitialized) return;
        hasInitialized = true;

        if (poolManager == null) poolManager = Object.FindAnyObjectByType<PoolManager>();
        if (upgradeManager == null) upgradeManager = Object.FindAnyObjectByType<UpgradeManager>();
        if (levelManager == null) levelManager = Object.FindAnyObjectByType<LevelManager>();

        // if (poolManager != null) poolManager.Intialize();
        if (upgradeManager != null) upgradeManager.Initialize();
        if (levelManager != null) levelManager.Initialize();
    }

    public void RunCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

}
