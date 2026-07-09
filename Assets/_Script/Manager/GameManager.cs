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

        if (deckManager == null) deckManager = Object.FindFirstObjectByType<DeckManager>();

        if (deckManager != null) deckManager.Initialize();


    }

     public void RunCoroutine(IEnumerator coroutine) => StartCoroutine(coroutine);
}