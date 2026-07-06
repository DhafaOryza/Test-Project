using System;
using System.Collections;
using System.Collections.Generic;
using _Dev.Script.Runtime.Core.Character.Enemy;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Character
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField]
        private CharacterDetection characterDetection;
        
        [SerializeReference]
        private Character _character;
        
        private CharacterState _characterState;
        
        private List<CharacterController> _targetInRange = new();
        private CharacterController _target;
        
        private void Start()
        {
            if (_character == null) return;
            _characterState = CharacterState.Idle;
            
            StartCoroutine(Brain());
                
        }

        public void Initialize(Character character)
        {
            
        }

        private void OnEnable()
        {
            if (characterDetection == null) return;
            characterDetection.OnCollisionDetected += OnCollisionDetected_Event;
            characterDetection.OnCollisionOut += OnCollisionOut_Event;
        }

        private void OnDisable()
        {
            if (characterDetection == null) return;
            characterDetection.OnCollisionDetected -= OnCollisionDetected_Event;
            characterDetection.OnCollisionOut -= OnCollisionOut_Event;
        }

        protected virtual void FixedUpdate()
        {
            if (_target == null) return;
            if (_characterState != CharacterState.Chasing) return;
            transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, 0.1f);
        }

        private IEnumerator Brain()
        {
            Debug.Log("Starting brain");
            
            while (true)
            {
                if (_target == null)
                {
                    _characterState = CharacterState.Idle;
                    _target = FindTarget();
                    yield return null;
                    continue;
                }
                
                float sqrDistance = (transform.position - _target.transform.position).sqrMagnitude;

                float sqrRadius = _character.CharacterStats.Radius * _character.CharacterStats.Radius;
                
                if (sqrDistance > sqrRadius)
                {
                    _characterState = CharacterState.Chasing;
                }
                else
                {
                    
                    _characterState = CharacterState.Attacking;
                }
                
                yield return new WaitForSeconds(0.1f);
            }
        }

        private CharacterController FindTarget()
        {
            CharacterController closest = null;
            float closestDistance = float.MaxValue;

            foreach (var target in _targetInRange)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = target;
                }
            }

            return closest;
        }

        private void OnCollisionDetected_Event(CharacterController character)
        {
            Debug.Log(character.gameObject.name);
            _targetInRange.Add(character);
        }
        private void OnCollisionOut_Event(CharacterController character)
        {
            Debug.Log(character.gameObject.name);
            _targetInRange.Remove(character);
        }

        private void OnDrawGizmos()
        {
            if (_character == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _character.CharacterStats.Radius);
        }
    }
}