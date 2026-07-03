using _Dev.Script.Runtime.Core.Character.Enemy;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Character.CharacterBehaviour
{
    public class MeleeCB : CharacterBehaviour
    {
        
        public override void GetTarget()
        {
            throw new System.NotImplementedException();
        }
        
        public override void Attack(EnemyController enemy)
        {
            transform.position = Vector3.MoveTowards(_characterTransform.position, enemy.transform.position, Time.deltaTime * 10);
        }
        
    }
}