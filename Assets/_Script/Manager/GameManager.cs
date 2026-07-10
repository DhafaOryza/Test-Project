using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    [Header ("Manager")]
    public DeckManager deckManager;
    public CardChoiceManager cardChoiceManager;
    public AllyManager allyManager;
    public HandManager handManager;
    public TurnManager turnManager;
    public EnemyManager enemyManager;
    public PlayerStats playerStats;
    public PoolManager poolManager;

    private bool hasInitialized = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        
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

        if (poolManager == null) poolManager = Object.FindFirstObjectByType<PoolManager>();
        if (deckManager == null) deckManager = Object.FindFirstObjectByType<DeckManager>();
        if (handManager == null) handManager = Object.FindFirstObjectByType<HandManager>();
        if (allyManager == null) allyManager = Object.FindFirstObjectByType<AllyManager>();
        if (turnManager == null) turnManager = Object.FindFirstObjectByType<TurnManager>();
        if (cardChoiceManager == null) cardChoiceManager = Object.FindFirstObjectByType<CardChoiceManager>();
        if (playerStats == null) playerStats = Object.FindFirstObjectByType<PlayerStats>();
        if (enemyManager == null) enemyManager = Object.FindFirstObjectByType<EnemyManager>();


        if (poolManager != null) poolManager.Initialize();
        if (enemyManager != null) enemyManager.Initialize();
        if (handManager != null) handManager.Initialize();
        if (allyManager != null) allyManager.Initialize();
        if (turnManager != null) turnManager.Initialize();
        if (cardChoiceManager != null) cardChoiceManager.Initialize();
        if (deckManager != null) deckManager.Initialize();


    }

     public void RunCoroutine(IEnumerator coroutine) => StartCoroutine(coroutine);
}