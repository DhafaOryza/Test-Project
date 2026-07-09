using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PoolManager poolManager;
    public LevelSpawnerManager levelSpawnerManager;
    public CharacterController characterController;

    private bool hasInitialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void Start()
    {
        if (hasInitialized) return;
        UpdateStageData();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hasInitialized = false;
        UpdateStageData();
    }

    private void UpdateStageData()
    {
        if (hasInitialized) return;
        hasInitialized = true;

        if (poolManager == null) poolManager = Object.FindFirstObjectByType<PoolManager>();
        if (levelSpawnerManager == null) levelSpawnerManager = Object.FindAnyObjectByType<LevelSpawnerManager>();
        if (characterController == null) characterController = Object.FindAnyObjectByType<CharacterController>();


        if (poolManager != null) poolManager.Initialize();
        if (levelSpawnerManager != null) levelSpawnerManager.Initialize();
        if (characterController != null) characterController.Initialize();
    }

    public void RunCoroutine(IEnumerator coroutine) => StartCoroutine(coroutine);
}