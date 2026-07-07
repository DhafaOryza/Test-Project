using System;
using System.Collections;
using System.Collections.Generic;
using _Dev.Script.Runtime.Core.Character.Enemy;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Character.CharacterBehaviour
{
    [Serializable]
    public abstract class CharacterBehaviour
    {
        protected bool IsAttacking;

        protected float Timer;

        public bool IsReady(float attackSpeed)
        {
            return !IsAttacking &&
                   Timer <= 0;
        }

        public void Tick(float dt)
        {
            Timer -= dt;
        }

        public void Trigger(float attackSpeed)
        {
            Timer = 1f / attackSpeed;
        }
        
        public abstract IEnumerator Attack(CharacterController characterController);
    }
}