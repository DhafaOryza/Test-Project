using System.Collections;
using System.Collections.Generic;
using _Dev.Script.Runtime.Core.Character.Enemy;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Character.CharacterBehaviour
{
    [System.Serializable]
    public abstract class CharacterBehaviour : MonoBehaviour
    {
        public abstract IEnumerator Attack(CharacterController characterController);
    }
}