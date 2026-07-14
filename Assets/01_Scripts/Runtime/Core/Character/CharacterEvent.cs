using System;

namespace _01_Scripts.Runtime.Core.Character
{
    public class CharacterEvent
    {
        public event Action OnAttackEvent;
        public event Action OnKillEvent;
        public event Action OnDeathEvent;
        
        public void TriggerOnAttackEvent()
        {
            OnAttackEvent?.Invoke();
        }

        public void TriggerOnKillEvent()
        {
            OnKillEvent?.Invoke();
        }

        public void TriggerOnDeathEvent()
        {
            OnDeathEvent?.Invoke();
        }
    }
}