using System.Collections.Generic;
using _01_Scripts.Runtime.Core.Spawner;
using _01_Scripts.Runtime.PoolingSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Character.Ally
{
    public class AlliesManager : SerializedMonoBehaviour
    {
        [Header("Data")]
        [SerializeField]
        private List<PoolIdSO> _allyRewards = new();

        [SerializeField]
        private Dictionary<PoolIdSO, CharacterDefSO> _characterDefs = new();

        [Header("References")]
        [SerializeField]
        private Transform _spawnPoint;

        private readonly List<Character> _allies = new();

        public IReadOnlyList<Character> Allies => _allies;

        public void AddCharacterTest(int rewardIndex)
        {
            AddCharacter(rewardIndex -1);
        }
        
        public bool AddCharacter(int rewardIndex)
        {
            if (!TryGetPoolId(rewardIndex, out PoolIdSO poolId))
                return false;

            if (!TryCreateCharacter(poolId, out Character character))
                return false;

            _allies.Add(character);

            GameManager.GameManager.Instance.AllySpawner.SpawnCharacterController(character, poolId, _spawnPoint);

            return true;
        }

        private bool TryGetPoolId(int rewardIndex, out PoolIdSO poolId)
        {
            poolId = null;

            if (rewardIndex < 0 || rewardIndex >= _allyRewards.Count)
            {
                Debug.LogWarning($"Reward index {rewardIndex} is invalid.");
                return false;
            }

            poolId = _allyRewards[rewardIndex];
            return true;
        }

        private bool TryCreateCharacter(PoolIdSO poolId, out Character character)
        {
            character = null;

            if (!_characterDefs.TryGetValue(poolId, out CharacterDefSO characterDef))
            {
                Debug.LogWarning($"CharacterDef for '{poolId.name}' not found.");
                return false;
            }

            character = new Character(characterDef.GetCharacterDataInstance());
            return true;
        }
    }
}