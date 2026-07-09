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
        
        public CharacterData CharacterData => _characterData;

        public Character ( CharacterData characterData)
        {
            _characterData = characterData;
        }
    }
}