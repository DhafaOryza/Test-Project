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
            transform.position = Vector3.MoveTowards(transform.position, baseTransform.position, 0.1f);
        }
    }
}