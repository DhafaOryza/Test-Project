using _Dev.Script.Runtime.Interaction;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Character.Ally
{
    public class AllyController : CharacterController
    {
        [SerializeField]
        private Draggle _draggle;
        
        public Draggle Draggle => _draggle;
        
    }
}