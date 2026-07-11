using System;
using System.Collections;
using _01_Scripts.Runtime.Interface;
using _01_Scripts.Runtime.PoolingSystem;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Character.CharacterBehaviour
{
    [Serializable]
    public class RangeCB : CharacterBehaviour
    {
        [SerializeField] 
        private PoolIdSO projectilePoolId;
        
        public override IEnumerator Attack(CharacterController owner, CharacterController target, int damage)
        {
            if (target.TryGetComponent(out IDamageable damageable))
            {
                Projectile.Projectile projectile = GameManager.GameManager.Instance.PoolManager.Spawn<Projectile.Projectile>(projectilePoolId, owner.transform.position, Quaternion.identity);
                    projectile.Shoot(target.transform);
                damageable.TakeDamage(damage);
                Debug.Log("Kena damage");
                yield return null;
            }  
        }
    }
}