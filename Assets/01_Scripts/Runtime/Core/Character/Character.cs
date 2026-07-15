using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Character
{
    [Serializable]
    public class Character
    {
        [SerializeReference, ReadOnly]
        private CharacterData _characterData;
        
        private CharacterEvent _characterEvent;
        
        public CharacterData CharacterData => _characterData;
        public CharacterEvent CharacterEvent => _characterEvent;

        public Character ( CharacterData characterData)
        {
            _characterData = characterData;
            _characterEvent = new CharacterEvent();
        }
    }
}