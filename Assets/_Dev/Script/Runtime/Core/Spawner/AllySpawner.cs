using _Dev.Script.Runtime.Core.Character.Ally;
using LumineREx.Utils.Singleton;
using UnityEngine;
using CharacterController = _Dev.Script.Runtime.Core.Character.CharacterController;

namespace _Dev.Script.Runtime.Core.Spawner
{
    public class AllySpawner : Singleton<AllySpawner>
    {
        [SerializeField]
        private AllyController _allyController;
        [SerializeField]
        private Collider2D boundaryCollider;
        
        public AllyController SpawnCharacterController(Character.Character character, Transform position)
        {
            AllyController allyController = Instantiate(_allyController, position.position, Quaternion.identity);
            allyController.Initialize(character);
            allyController.Draggle.SetBoundary(boundaryCollider);
            return allyController;
        }
    }
}