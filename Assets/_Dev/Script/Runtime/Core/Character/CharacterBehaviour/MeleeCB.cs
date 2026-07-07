using System.Collections;
using _Dev.Script.Runtime.Core.Character.Enemy;
using _Dev.Script.Runtime.Interface;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Character.CharacterBehaviour
{
    public class MeleeCB : CharacterBehaviourx
    {
        public override IEnumerator Attack(CharacterController characterController)
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