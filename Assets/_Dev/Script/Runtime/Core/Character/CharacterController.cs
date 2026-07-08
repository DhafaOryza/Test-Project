using System;
using System.Collections;
using System.Collections.Generic;
using _Dev.Script.Runtime.Core.Character.Enemy;
using _Dev.Script.Runtime.Enum;
using _Dev.Script.Runtime.Interface;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Character
{
    public class CharacterController : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private CharacterDetection characterDetection;
        
        [SerializeReference]
        protected Character _character;
        
        protected CharacterState _characterState;
        
        protected List<CharacterController> _targetInRange = new();
        protected CharacterController _target;
        
        public event Action OnDeathEvent;
        
        private void Start()
        {
            if (_character == null) return;
            _characterState = CharacterState.Idle;
            
            StartCoroutine(Brain());
                
        }

        public virtual void Initialize(Character character, Transform baseTransform = null)
        {
            _character = character;
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
                var behaviour = _character.CharacterData.CharacterBehaviour;
                
                behaviour.Tick(Time.deltaTime);
                
                if (_target == null)
                {
                    _characterState = CharacterState.Idle;
                    _target = FindTarget();
                    yield return null;
                    continue;
                }
                
                float sqrDistance = (transform.position - _target.transform.position).sqrMagnitude;

                float sqrRadius = _character.CharacterData.CharacterStats.Radius * _character.CharacterData.CharacterStats.Radius;
                
                if (sqrDistance > sqrRadius)
                {
                    _characterState = CharacterState.Chasing;
                }
                else
                {
                    
                    if (behaviour.IsReady(_character.CharacterData.CharacterStats.Speed))
                    {
                        Debug.Log("Gebug");
                        behaviour.Trigger(_character.CharacterData.CharacterStats.Speed);
                        StartCoroutine(behaviour.Attack(_target, _character.CharacterData.CharacterStats.Attack));
                    }
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
            Gizmos.DrawWireSphere(transform.position, _character.CharacterData.CharacterStats.Radius);
        }

        public void TakeDamage(int damage)
        {
            _character.CharacterData.CharacterStats.TakeDamage(damage);
            if (_character.CharacterData.CharacterStats.IsDead)
            {
                _characterState = CharacterState.Dead;
                Die();
            }
        }

        public void Die()
        {
            OnDeathEvent?.Invoke();
            Destroy(gameObject);
        }
    }
}