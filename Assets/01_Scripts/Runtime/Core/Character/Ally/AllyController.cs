using _01_Scripts.Runtime.Interaction;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Character.Ally
{
    public class AllyController : CharacterController
    {
        [SerializeField]
        private Draggle _draggle;
        
        public Draggle Draggle => _draggle;
        
    }
}