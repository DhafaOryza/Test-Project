using System;
using _01_Scripts.Runtime.Core.ActionSystem;
using _01_Scripts.Runtime.Core.GameAction;
using _01_Scripts.Runtime.Core.Health;
using UnityEngine;

namespace _01_Scripts.Runtime.GameManager
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