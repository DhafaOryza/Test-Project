using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TopDown.Combat;
using TopDown.Movement;
using TopDown.UI;

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
    public PlayerPoints playerPoints;
    public AmmoUI ammoUI;

    public Camera mainCamera;
    public Transform player;
    public Transform cameraTarget;
    public bool hasInitialized = false;

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
        if (playerPoints == null) playerPoints = Object.FindAnyObjectByType<PlayerPoints>();
        if (ammoUI == null) ammoUI = Object.FindAnyObjectByType<AmmoUI>();

        if (weaponHolder != null) weaponHolder.InitializeSession();
        if (playerAnimation != null) playerAnimation.Initalize();
        if (poolManager != null) poolManager.Initialize();
        if (upgradeManager != null) upgradeManager.Initialize();
        if (levelManager != null) levelManager.Initialize();
        if (scoreManager != null) scoreManager.Initialize();
        if (playerStats != null) playerStats.Initialize();
        if (movementPlayer != null) movementPlayer.Initialize();
        if (playerHealth != null) playerHealth.Initalize();
        if (cameraFollow != null) cameraFollow.Initialize(cameraTarget);
        if (playerRotation != null) playerRotation.Initialize(player, cameraTarget, mainCamera);
        if (gameTimer != null) gameTimer.Initialize();
        if (playerPoints != null) playerPoints.initialize();
        if (ammoUI != null) ammoUI.Initialize(weaponHolder);
    
    }

    public void RunCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

}
