using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Ability
{
    public class AbilityHolder : MonoBehaviour
    {
        private int _maxAbilitySlots;
        
        private readonly List<AbilityDefSO> _activeAbilities = new List<AbilityDefSO>();

        public bool AddAbility(AbilityDefSO ability, CharacterController characterController)
        {
            if (_activeAbilities.Count >= _maxAbilitySlots) return false;
            
            _activeAbilities.Add(ability);
            ability.OnAddAbility(characterController);
            return true;
        }

        public void RemoveAbility(AbilityDefSO ability, CharacterController characterController)
        {
            _activeAbilities.Remove(ability);
            ability.OnRemoveAbility(characterController);
        }
    }
}