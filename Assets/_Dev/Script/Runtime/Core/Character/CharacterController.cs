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
        
        private List<CharacterController> _targetInRange = new();
        private CharacterController _target;
        
        private void Start()
        {
            if (_character == null) return;
            
            StartCoroutine(Brain());
                
        }

        private void OnEnable()
        {
            if (characterDetection == null) return;
            characterDetection.OnCollisionDetected += OnCollisionDetected_Event;
            characterDetection.OnCollisionOut += OnCollisionDetected_Event;
        }

        private void OnDisable()
        {
            if (characterDetection == null) return;
            characterDetection.OnCollisionDetected -= OnCollisionDetected_Event;
            characterDetection.OnCollisionOut -= OnCollisionDetected_Event;
        }

        protected virtual void FixedUpdate()
        {
            if (_target == null) return;
            transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, 0.1f);
        }

        private IEnumerator Brain()
        {
            Debug.Log("Starting brain");
            
            while (true)
            {
                if (_target == null)
                {
                    _target = FindTarget();
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
    }
}