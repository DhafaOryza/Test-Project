using System;
using System.Collections;
using System.Collections.Generic;
using _Dev.Script.Runtime.Core.Character;
using _Dev.Script.Runtime.Core.GameAction;
using _Dev.Script.Runtime.Core.Spawner;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Dev.Script.Runtime.Core.Wave
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField]
        private WaveDefSO waveDef;

        [SerializeField]
        private List<Transform> spawnPoints;

        private int _currentWave;

        private int _enemiesAlive;

        public WaveData CurrentWave => waveDef.WaveData[_currentWave];

        public event Action OnWaveFinished;

        private void OnEnable()
        {
            ActionSystem.ActionSystem.AttachPerformer<BattlePhaseGA>(BattlePhasePerformer);
        }

        private void OnDisable()
        {
            ActionSystem.ActionSystem.DetachPerformer<BattlePhaseGA>();
        }


        private IEnumerator BattlePhasePerformer(BattlePhaseGA battlePhaseGA)
        {
            yield return SpawnRoutine();
        }

        private IEnumerator SpawnRoutine()
        {
            foreach (var enemy in CurrentWave.Enemies)
            {
                for (int i = 0; i < enemy.Amount; i++)
                {
                    Spawn(enemy);
                    

                    _enemiesAlive++;

                    yield return new WaitForSeconds(CurrentWave.SpawnInterval);
                }
            }
        }

        private void Spawn(SpawnEntry enemy)
        {
            var point = spawnPoints[Random.Range(0, spawnPoints.Count)];

            var enemyController = EnemySpawner.Instance.SpawnCharacterController(CreateCharacter(enemy.Character), point);
            enemyController.OnDeathEvent += RegisterEnemyDeath;
        }

        private void RegisterEnemyDeath()
        {
            _enemiesAlive--;

            if (_enemiesAlive > 0)
                return;

            _currentWave++;

            ActionSystem.ActionSystem.Instance.Perform(new PreparationPhaseGA());
        }

        private Character.Character CreateCharacter(CharacterDefSO def)
        {
            return new Character.Character(def.GetCharacterDataInstance());
        }

        public bool HasNextWave()
        {
            return _currentWave < waveDef.WaveData.Count;
        }
    }
}