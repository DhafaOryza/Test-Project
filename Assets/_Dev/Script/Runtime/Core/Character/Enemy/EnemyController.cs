using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Character.Enemy
{
    public class EnemyController : CharacterController
    {
        [SerializeField] 
        private Transform baseTransform;
        
        protected override void FixedUpdate()
        {
            if (_characterState == CharacterState.Attacking) return;
            if (_target == null)
            {
                transform.position = Vector3.MoveTowards(transform.position, baseTransform.position, 0.1f);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, 0.1f);
            }
        }
    }
}