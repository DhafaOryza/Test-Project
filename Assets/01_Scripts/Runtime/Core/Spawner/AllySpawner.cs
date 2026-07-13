using _01_Scripts.Runtime.Core.Character.Ally;
using _01_Scripts.Runtime.PoolingSystem;
using LumineREx.Utils.Singleton;
using UnityEngine;
using CharacterController = _01_Scripts.Runtime.Core.Character.CharacterController;

namespace _01_Scripts.Runtime.Core.Spawner
{
    public class AllySpawner : MonoBehaviour
    {
        [SerializeField]
        private Collider2D boundaryCollider;
        
        public AllyController SpawnCharacterController(Character.Character character, PoolIdSO poolIdSo, Transform position)
        {
            AllyController allyController = GameManager.GameManager.Instance.PoolManager.Spawn<AllyController>(poolIdSo, position.position, Quaternion.identity);
            allyController.Initialize(character);
            allyController.CharacterInteraction.SetBoundary(boundaryCollider);
            return allyController;
        }
    }   
}