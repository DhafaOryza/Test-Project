using System;
using System.Collections;
using _Dev.Script.Runtime.Core.Character.Enemy;
using _Dev.Script.Runtime.Interface;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Character.CharacterBehaviour
{
    [Serializable]
    public class MeleeCB : CharacterBehaviour
    {
        public override IEnumerator Attack(CharacterController characterController, int damage)
        {
            if (characterController.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(10);
                Debug.Log("Kena damage");
                yield return null;
            }   
        }
        
    }
}