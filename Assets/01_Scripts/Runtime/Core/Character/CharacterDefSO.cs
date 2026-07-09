using UnityEngine;

namespace _01_Scripts.Runtime.Core.Character
{
    [CreateAssetMenu(fileName = "CharacterDefSO", menuName = "Data/CharacterDefSO", order = 0)]
    public class CharacterDefSO : ScriptableObject
    {
        [SerializeReference]
        private CharacterData _characterData;
        
        [SerializeField]
        private Sprite _characterSprite;
        
        public Sprite CharacterSprite => _characterSprite;
        
        public CharacterData GetCharacterDataInstance()
        {
            return new CharacterData(_characterData); 
        }
    }
}