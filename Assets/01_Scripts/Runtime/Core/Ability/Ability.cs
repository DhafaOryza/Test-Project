using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Ability
{
    [System.Serializable]
    public abstract class Ability
    {
        [SerializeField] 
        protected float _cooldown;
        
        private bool _isReady = false;
        private bool _isActive = false;

        private CharacterController _owner;

        public abstract void OnAdd(CharacterController characterController);
        public abstract void OnRemove(CharacterController characterController);
        
        public abstract void Activate();

        public virtual IEnumerator Cooldown()
        {
            _isReady = false;

            yield return new WaitForSeconds(_cooldown);

            _isReady = true;
        }
    }
}