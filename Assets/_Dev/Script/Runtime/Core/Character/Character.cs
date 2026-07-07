using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Character
{
    [Serializable]
    public class Character
    {
        [SerializeReference, ReadOnly]
        private CharacterData _characterData;
        
        [SerializeReference]
        private CharacterStats _characterStats;
        
        public CharacterData CharacterData => _characterData;
        public CharacterStats CharacterStats => _characterStats;

        public Character (CharacterStats characterStats, CharacterData characterData)
        {
            _characterStats = characterStats;
        }
    }
}