using System.Collections.Generic;
using _01_Scripts.Runtime.Core.Spawner;
using _01_Scripts.Runtime.PoolingSystem;
using UnityEngine;
using Sirenix.OdinInspector;

namespace _01_Scripts.Runtime.Core.Character.Ally
{
    public class AlliesManager : SerializedMonoBehaviour
    {
        [Header("Data")]
        [SerializeField]
        private Dictionary<PoolIdSO, CharacterDefSO> _characterDefs = new Dictionary<PoolIdSO, CharacterDefSO>();
        
        [Header("References")]
        [SerializeField]
        private Transform spawnPoint;
        
        private readonly List<Character> _allies = new List<Character>();
        
        public void AddCharacter(int number)
        {
            // Character character = GetCharacter(number);
            // if (character == null) return;
            //
            // _allies.Add(character);
            // AllyController allyController = AllySpawner.Instance.SpawnCharacterController(character, spawnPoint);
        }

        // public Character GetCharacter(int number)
        // {
        //     switch (number) 
        //     {
        //         case 1: return new Character(_characterDefs[0].GetCharacterDataInstance());
        //         case 2: return new Character(_characterDefs[1].GetCharacterDataInstance());
        //         case 3: return new Character(_characterDefs[2].GetCharacterDataInstance());
        //         case 4: return new Character(_characterDefs[3].GetCharacterDataInstance());
        //         case 5: return new Character(_characterDefs[4].GetCharacterDataInstance());
        //         default: return null;
        //     }
        // }
    }
}