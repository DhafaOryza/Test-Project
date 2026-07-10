using _01_Scripts.Runtime.Core.Character.Enemy;
using _01_Scripts.Runtime.PoolingSystem;
using LumineREx.Utils.Singleton;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Spawner
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private Transform baseTarget;
    
        public EnemyController SpawnCharacterController(Character.Character character, PoolIdSO poolIdSo, Transform position)
        {
            EnemyController enemyController = GameManager.GameManager.Instance.PoolManager.Spawn<EnemyController>(poolIdSo, position.position, Quaternion.identity);
            enemyController.Initialize(character, baseTarget);
            return enemyController;
        }
    }
}