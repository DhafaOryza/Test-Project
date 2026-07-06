using _Dev.Script.Runtime.Core.Character.Ally;
using UnityEngine;
using CharacterController = _Dev.Script.Runtime.Core.Character.CharacterController;

namespace _Dev.Script.Runtime.Core.Spawner
{
    public class AllySpawner : CharacterControllerSpawner
    {
        [SerializeField]
        private AllyController _allyController;
        
        public override CharacterController SpawnCharacterController()
        {
            AllyController allyController = Instantiate(_allyController);
            return allyController;
        }
    }
}