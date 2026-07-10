using System;
using System.Collections;
using _01_Scripts.Runtime.Core.Character.Enemy;
using _01_Scripts.Runtime.Interface;
using DG.Tweening;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Character.CharacterBehaviour
{
    [Serializable]
    public class MeleeCB : CharacterBehaviour
    {
        public override IEnumerator Attack(CharacterController owner, CharacterController target, int damage)
        {
            if (target.TryGetComponent(out IDamageable damageable))
            {
                float dir = Mathf.Sign(target.transform.position.x - owner.transform.position.x);
                owner.transform.DOPunchPosition(Vector3.right * dir * 0.3f, 0.15f);
                damageable.TakeDamage(damage);
                Debug.Log("Kena damage");
                yield return null;
            }   
        }
        
    }
}