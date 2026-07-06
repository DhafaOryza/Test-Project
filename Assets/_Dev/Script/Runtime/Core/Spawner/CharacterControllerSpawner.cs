using UnityEngine;
using CharacterController = _Dev.Script.Runtime.Core.Character.CharacterController;

namespace _Dev.Script.Runtime.Core.Spawner
{
    public abstract class CharacterControllerSpawner : MonoBehaviour
    {
        public abstract CharacterController SpawnCharacterController();
    }
}