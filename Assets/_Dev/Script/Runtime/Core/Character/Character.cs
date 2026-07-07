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
        
        public CharacterData CharacterData => _characterData;

        public Character ( CharacterData characterData)
        {
            _characterData = characterData;
        }
    }
}