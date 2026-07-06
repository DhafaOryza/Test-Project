using System;
using UnityEditor.U2D.Animation;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Character
{
    [Serializable]
    public class Character
    {
        [SerializeReference]
        private CharacterStats _characterStats;
        
        public CharacterStats CharacterStats => _characterStats;

        public Character (CharacterStats characterStats)
        {
            _characterStats = characterStats;
        }
    }
}