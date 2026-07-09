using System;
using System.Collections;
using _01_Scripts.Runtime.Core.Character.Enemy;
using _01_Scripts.Runtime.Interface;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Character.CharacterBehaviour
{
    [Serializable]
    public class MeleeCB : CharacterBehaviour
    {
        public override IEnumerator Attack(CharacterController characterController, int damage)
        {
            if (characterController.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
                Debug.Log("Kena damage");
                yield return null;
            }   
        }
        
    }
}