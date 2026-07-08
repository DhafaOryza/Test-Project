using UnityEngine;

namespace _Dev.Script.Runtime.Core.Character
{
    [System.Serializable]
    public class CharacterData
    {
        [SerializeField, Tooltip("The name of the character")]
        private string _nameCharacter;
        [SerializeReference, Tooltip("The all stats of the character")]
        private CharacterStats _characterStats;
        [SerializeReference, Tooltip("The behaviour of the character ex. Melee, Range, etc.")]
        private CharacterBehaviour.CharacterBehaviour _characterBehaviour;
        
        public string NameCharacter => _nameCharacter;
        public CharacterStats CharacterStats => _characterStats;
        public CharacterBehaviour.CharacterBehaviour CharacterBehaviour => _characterBehaviour;

        public CharacterData() {}
        
        public CharacterData(CharacterData characterData)
        {
            _nameCharacter = characterData.NameCharacter;
            _characterStats = new CharacterStats(characterData.CharacterStats);
            _characterBehaviour = characterData.CharacterBehaviour;
        }
    }
}