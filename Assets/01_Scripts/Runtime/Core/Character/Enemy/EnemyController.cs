using System.Collections;
using _01_Scripts.Runtime.Enum;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace _01_Scripts.Runtime.Core.Character.Enemy
{
    public class EnemyController : CharacterController
    {
        [FormerlySerializedAs("baseTransform")] [SerializeField] 
        private Transform _baseTransform;
        
        protected override void FixedUpdate()
        {
            if (_characterState == CharacterState.Attacking) return;
            if (_target == null)
            {
                transform.position = Vector3.MoveTowards(transform.position, _baseTransform.position, 0.1f);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, 0.1f);
            }
        }

        public override void Initialize(Character character, Transform baseTransform)
        {
            base.Initialize(character);
            _baseTransform = baseTransform;
            
        }
    }
}