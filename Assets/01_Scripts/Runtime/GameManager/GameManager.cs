using System;
using _01_Scripts.Runtime.Core.ActionSystem;
using _01_Scripts.Runtime.Core.Character.Ally;
using _01_Scripts.Runtime.Core.Coin;
using _01_Scripts.Runtime.Core.Health;
using _01_Scripts.Runtime.PoolingSystem;
using LumineREx.Utils.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace _01_Scripts.Runtime.GameManager
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("Game References")]
        [SerializeField]
        private ActionSystem _actionSystem;
        [SerializeField]
        private PoolManager _poolManager;
        [SerializeField]
        private HealthManager _healthManager;
        [SerializeField]
        private CoinManager _coinManager;
        [SerializeField]
        private AlliesManager _alliesManager;
        
        
        //Property
        public ActionSystem ActionSystem => _actionSystem;
        public PoolManager PoolManager => _poolManager;
        public HealthManager HealthManager => _healthManager;
        public CoinManager CoinManager => _coinManager;
        public AlliesManager AlliesManager => _alliesManager;
        
        
        private bool _hasInitialized = false;

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;


        private void Start()
        {
            if (!_hasInitialized) return;
            UpdateStageData();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            _hasInitialized = false;
            UpdateStageData();
        }

        private void UpdateStageData()
        {
            _hasInitialized = true;
            
            if (_poolManager == null) _poolManager = Object.FindFirstObjectByType<PoolManager>();
            
            if (_poolManager != null) _poolManager.Initialize();
            
            // if (poolManager == null) poolManager = Object.FindFirstObjectByType<PoolManager>();
            // if (levelSpawnerManager == null) levelSpawnerManager = Object.FindAnyObjectByType<LevelSpawnerManager>();
            
            // if (poolManager != null) poolManager.Initialize();
            // if (levelSpawnerManager != null) levelSpawnerManager.Initialize();
        }
    }
}