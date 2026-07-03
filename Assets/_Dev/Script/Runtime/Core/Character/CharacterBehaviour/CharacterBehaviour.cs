using System.Collections.Generic;
using _Dev.Script.Runtime.Core.Character.Enemy;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Character.CharacterBehaviour
{
    public abstract class CharacterBehaviour : MonoBehaviour
    {
        private List<EnemyController> _enemiesInRange;
        protected Transform _characterTransform;

        public virtual void Initialize(List<EnemyController> enemiesInRange, Transform characterTransform)
        {
            _enemiesInRange = enemiesInRange;
            _characterTransform = characterTransform;
        }
        
        public abstract void GetTarget();
        public abstract void Attack(EnemyController enemy);
    }
}