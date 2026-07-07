using System.Collections;
using System.Collections.Generic;
using _Dev.Script.Runtime.Core.Wave;
using _Dev.Script.Runtime.Enum;
using UnityEngine;

namespace _Dev.Script.Runtime.GameManager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private WaveManager waveManager;

        private GameState _state = GameState.Preparation;
        
        public GameState State => _state;

        private bool _playerReady;

        Coroutine preparationRoutine;

        void Start()
        {
            waveManager.OnWaveFinished += EnterPreparation;

            EnterPreparation();
        }

        void EnterPreparation()
        {
            _state = GameState.Preparation;

            _playerReady = false;

            if(preparationRoutine != null) StopCoroutine(preparationRoutine);

            preparationRoutine = StartCoroutine(PreparationRoutine());
        }

        private IEnumerator PreparationRoutine()
        {
            float timer = waveManager.CurrentWave.PreparationDuration;

            while (timer > 0)
            {
                if (_playerReady)
                    break;

                timer -= Time.deltaTime;

                yield return null;
            }

            EnterBattle();
        }

        private void EnterBattle()
        {
            _state = GameState.Battle;

            waveManager.StartWave();
        }

        public void Ready()
        {
            if(State != GameState.Preparation)
                return;

            _playerReady = true;
        }
    }
}