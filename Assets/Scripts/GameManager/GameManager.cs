using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TopDown.Combat;
using TopDown.Movement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    [Header("Managers")]
    public PoolManager poolManager;
    public UpgradeManager upgradeManager;
    public LevelManager levelManager;
    public WeaponHolder weaponHolder;
    public ScoreManager scoreManager;
    public PlayerStats playerStats;
    public MovementPlayer movementPlayer;
    public PlayerAnimation playerAnimation;
    public PlayerHealth playerHealth;
    public CameraFollow cameraFollow;
    public PlayerRotation playerRotation;
    public GameTimer gameTimer;

    private bool hasInitialized = false;
    private Camera mainCamera;
    private Transform player;
    private Transform cameraTarget;

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

        mainCamera = Camera.main;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        GameObject target = GameObject.Find("CameraTarget");

        if (target != null) cameraTarget = target.transform;

        if (weaponHolder == null) weaponHolder = Object.FindAnyObjectByType<WeaponHolder>(FindObjectsInactive.Include);
        if (playerAnimation == null) playerAnimation = Object.FindAnyObjectByType<PlayerAnimation>();
        if (poolManager == null) poolManager = Object.FindAnyObjectByType<PoolManager>();
        if (upgradeManager == null) upgradeManager = Object.FindAnyObjectByType<UpgradeManager>();
        if (levelManager == null) levelManager = Object.FindAnyObjectByType<LevelManager>();
        if (scoreManager == null) scoreManager = Object.FindAnyObjectByType<ScoreManager>();
        if (playerStats == null) playerStats = Object.FindAnyObjectByType<PlayerStats>();
        if (movementPlayer == null) movementPlayer = Object.FindAnyObjectByType<MovementPlayer>();
        if (playerHealth == null) playerHealth = Object.FindAnyObjectByType<PlayerHealth>();
        if (cameraFollow == null) cameraFollow = Object.FindAnyObjectByType<CameraFollow>();
        if (playerRotation == null) playerRotation = Object.FindAnyObjectByType<PlayerRotation>();
        if (gameTimer == null) gameTimer = Object.FindAnyObjectByType<GameTimer>();

        if (weaponHolder != null) weaponHolder.InitializeSession();
        if (playerAnimation != null) playerAnimation.Initalize();
        // if (poolManager != null) poolManager.Intialize();
        if (upgradeManager != null) upgradeManager.Initialize();
        if (levelManager != null) levelManager.Initialize();
        if (scoreManager != null) scoreManager.Initialize();
        if (playerStats != null) playerStats.Initialize();
        if (movementPlayer != null) movementPlayer.Initialize();
        if (playerHealth != null) playerHealth.Initalize();
        if (cameraFollow != null) cameraFollow.Initialize(player);
        if (playerRotation != null) playerRotation.Initialize(player, cameraTarget, mainCamera);
        if (gameTimer != null) gameTimer.Initialize();
    
    }

    public void RunCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

}
