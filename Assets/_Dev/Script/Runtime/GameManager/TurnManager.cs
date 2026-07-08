using System;
using _Dev.Script.Runtime.Core.ActionSystem;
using _Dev.Script.Runtime.Core.GameAction;
using UnityEngine;

namespace _Dev.Script.Runtime.GameManager
{
    public class TurnManager : MonoBehaviour
    {
        private void Start()
        {
            ActionSystem.Instance.Perform(new PreparationPhaseGA());
        }
    }
}