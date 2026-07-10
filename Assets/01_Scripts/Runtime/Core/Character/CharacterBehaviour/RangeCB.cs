using System.Collections;
using _01_Scripts.Runtime.Interface;
using _01_Scripts.Runtime.PoolingSystem;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Character.CharacterBehaviour
{
    public class RangeCB : CharacterBehaviour
    {
        [SerializeField] 
        private PoolIdSO projectilePoolId;
        
        public override IEnumerator Attack(CharacterController owner, CharacterController target, int damage)
        {
            if (target.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
                Debug.Log("Kena damage");
                yield return null;
            }  
        }
    }
}