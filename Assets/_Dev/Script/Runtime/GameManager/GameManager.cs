using System;
using System.Collections;
using System.Collections.Generic;
using _Dev.Script.Runtime.Core.ActionSystem;
using _Dev.Script.Runtime.Core.GameAction;
using _Dev.Script.Runtime.Core.Wave;
using _Dev.Script.Runtime.Enum;
using NUnit.Framework.Internal;
using UnityEngine;

namespace _Dev.Script.Runtime.GameManager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private WaveManager waveManager;
        [SerializeField] 
        private PreparationUI preparationUI;

        private GameState _state = GameState.Preparation;
        
        public GameState State => _state;

        private bool _playerReady;

        Coroutine preparationRoutine;

        private void OnEnable()
        {
            ActionSystem.AttachPerformer<PreparationPhaseGA>(PreparationPhasePerformer);
            ActionSystem.AttachPerformer<ResolutionPhaseGA>(ResolutionPhasePerformer);
        }

        private void OnDisable()
        {
            ActionSystem.DetachPerformer<PreparationPhaseGA>();
            ActionSystem.DetachPerformer<ResolutionPhaseGA>();
        }

        private IEnumerator PreparationPhasePerformer(PreparationPhaseGA preparationPhaseGA)
        {
            EnterPreparation();
            yield return null;
        }

        private IEnumerator ResolutionPhasePerformer(ResolutionPhaseGA preparationPhaseGA)
        {
            Debug.Log("Kelar wavenya");
            yield return null;
        }
        
        
        void EnterPreparation()
        {
            _state = GameState.Preparation;

            _playerReady = false;

            if(preparationRoutine != null) StopCoroutine(preparationRoutine);
            
            preparationRoutine = StartCoroutine(PreparationRoutine());
            preparationUI.Show();
        }

        private IEnumerator PreparationRoutine()
        {
            float timer = waveManager.CurrentWave.PreparationDuration;
            float maxTime = timer;

            while (timer > 0)
            {
                preparationUI.SetTimer(timer, maxTime);
                
                if (_playerReady)
                    break;

                timer -= Time.deltaTime;

                yield return null;
            }
            preparationUI.SetTimer(0, maxTime);

            EnterBattle();
        }

        private void EnterBattle()
        {
            preparationUI.Hide();
            _state = GameState.Battle;

            ActionSystem.Instance.Perform(new BattlePhaseGA());
        }

        public void Ready()
        {
            if(State != GameState.Preparation)
                return;

            _playerReady = true;
        }
    }
}