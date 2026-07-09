using _01_Scripts.Runtime.Core.Character.Enemy;
using LumineREx.Utils.Singleton;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Spawner
{
    public class EnemySpawner : Singleton<EnemySpawner>
    {
        [SerializeField]
        private EnemyController _allyController;
        
        [SerializeField]
        private Transform baseTarget;
    
        public EnemyController SpawnCharacterController(Character.Character character, Transform position)
        {
            EnemyController enemyController = Instantiate(_allyController, position.position, Quaternion.identity);
            enemyController.Initialize(character, baseTarget);
            return enemyController;
        }
    }
}