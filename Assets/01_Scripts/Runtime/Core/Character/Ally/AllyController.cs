using _01_Scripts.Runtime.Interaction;
using UnityEngine;
using UnityEngine.Serialization;

namespace _01_Scripts.Runtime.Core.Character.Ally
{
    public class AllyController : CharacterController
    {
        [FormerlySerializedAs("_draggle")] [SerializeField]
        private CharacterInteraction characterInteraction;
        
        public CharacterInteraction CharacterInteraction => characterInteraction;
        
    }
}