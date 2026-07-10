using UnityEngine;

namespace _01_Scripts.Runtime.Core.Character
{
    [CreateAssetMenu(fileName = "CharacterDefSO", menuName = "Data/CharacterDefSO", order = 0)]
    public class CharacterDefSO : ScriptableObject
    {
        [SerializeReference]
        private CharacterData _characterData;
        
        public CharacterData GetCharacterDataInstance()
        {
            return new CharacterData(_characterData); 
        }
    }
}