using UnityEngine;

namespace _01_Scripts.Runtime.Core.Ability
{
        [CreateAssetMenu(fileName = "AbilityDefSO", menuName = "Data/AbilityDefSO", order = 0)]
    public class AbilityDefSO : ScriptableObject
    {
        [SerializeReference]
        private Ability _ability;

        public void OnAddAbility(CharacterController characterController)
        {
            _ability.OnAdd(characterController);
        }

        public void OnRemoveAbility(CharacterController characterController)
        {
            _ability.OnRemove(characterController);
        }
    }
}