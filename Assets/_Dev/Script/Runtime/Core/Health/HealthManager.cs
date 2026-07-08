using _Dev.Script.Runtime.Core.GameAction;
using LumineREx.Utils.Singleton;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Health
{
    public class HealthManager : Singleton<HealthManager>
    {
        [SerializeField]
        private HealthUI _healthUI;
        
        private int _maxHealth;
        private int _currentHealth;
        
        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _currentHealth;

        public void Setup(int maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
            
            _healthUI.UpdateHealth(_maxHealth);
        }
        
        public void TakeDamage(int amount)
        {
            _currentHealth -= amount;
            _healthUI.UpdateHealth(_currentHealth);

            if (_currentHealth <= 0)
            {
                ActionSystem.ActionSystem.Instance.ForcePerform(new ResolutionPhaseGA());
            }
        }
    }
}