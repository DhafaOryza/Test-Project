using LumineREx.Utils.Singleton;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Health
{
    public class HealthManager : Singleton<HealthManager>
    {
        private int _maxHealth;
        private int _currentHealth;
        
        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _currentHealth;
        
        public void TakeDamage(int amount)
        {
            _currentHealth -= amount;
        }
    }
}