using System;
using _Dev.Script.Runtime.Core.ActionSystem;
using _Dev.Script.Runtime.Core.GameAction;
using _Dev.Script.Runtime.Core.Health;
using UnityEngine;

namespace _Dev.Script.Runtime.GameManager
{
    public class TurnManager : MonoBehaviour
    {
        private void Start()
        {
            HealthManager.Instance.Setup(6);
            
            ActionSystem.Instance.Perform(new PreparationPhaseGA());
        }
    }
}