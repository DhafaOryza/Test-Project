using System;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Character
{
    [Serializable]
    public class CharacterStats
    {
        [SerializeField] 
        private int _maxHealth;
        [SerializeField] 
        private int _attack;
        [SerializeField] 
        private int _defense;
        [SerializeField] 
        private float _speed;
        [SerializeField] 
        private float _radius;

        private int _currentHealth;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public int Attack => _attack;
        public int Defense => _defense;

        // 100 = normal speed
        public float Speed => _speed;

        public float Radius => _radius;

        public bool IsDead => _currentHealth <= 0;

        public CharacterStats(){}

        public CharacterStats(CharacterStats stats)
        {
            _maxHealth = stats._maxHealth;
            _attack = stats._attack;
            _defense = stats._defense;
            _speed = stats._speed;
            _radius = stats._radius;

            _currentHealth = _maxHealth;
        }

        public void Initialize()
        {
            _currentHealth = _maxHealth;
        }

        public int TakeDamage(int incomingDamage)
        {
            int finalDamage = Mathf.Max(1, incomingDamage - _defense);

            _currentHealth -= finalDamage;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

            return finalDamage;
        }

        public void Heal(int amount)
        {
            _currentHealth += amount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        }

        public void ModifyMaxHealth(int amount)
        {
            _maxHealth += amount;

            if (_maxHealth < 1)
                _maxHealth = 1;

            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        }
    }
}