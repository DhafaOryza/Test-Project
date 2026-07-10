using _01_Scripts.Runtime.Core.GameAction;
using UnityEngine;

namespace _01_Scripts.Runtime.GamePhaseSystem
{
    public class TurnManager : MonoBehaviour
    {
        private void Start()
        {
            GameManager.GameManager.Instance.HealthManager.Setup(6);
            
            GameManager.GameManager.Instance.ActionSystem.Perform(new PreparationPhaseGA());
        }
    }
}