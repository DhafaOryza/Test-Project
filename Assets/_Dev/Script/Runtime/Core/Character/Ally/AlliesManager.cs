using System.Collections.Generic;
using _Dev.Script.Runtime.Core.Spawner;
using LumineREx.Utils.Singleton;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Character.Ally
{
    public class AlliesManager : Singleton<AlliesManager>
    {
        [Header("Data")]
        [SerializeField]
        private List<CharacterDefSO> _characterDefs;
        
        [Header("References")]
        [SerializeField]
        private Transform spawnPoint;
        
        private readonly List<Character> _allies = new List<Character>();
        
        public void AddCharacter(int number)
        {
            Character character = GetCharacter(number);
            if (character == null) return;
            
            _allies.Add(character);
            AllyController allyController = AllySpawner.Instance.SpawnCharacterController(character, spawnPoint);
        }

        public Character GetCharacter(int number)
        {
            switch (number) 
            {
                case 1: return new Character(_characterDefs[0].GetCharacterDataInstance());
                case 2: return new Character(_characterDefs[1].GetCharacterDataInstance());
                case 3: return new Character(_characterDefs[2].GetCharacterDataInstance());
                case 4: return new Character(_characterDefs[3].GetCharacterDataInstance());
                case 5: return new Character(_characterDefs[4].GetCharacterDataInstance());
                default: return null;
            }
        }
    }
}