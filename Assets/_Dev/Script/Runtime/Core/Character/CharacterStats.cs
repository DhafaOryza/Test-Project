using System;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Character
{
    [Serializable]
    public class CharacterStats
    {
        private int _maxHealth;
        private int _attack;
        private int _defense;
        private int _speed;
        [SerializeField]
        private float _radius;
            
        private int _currentHealth;
        
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public int Attack => _attack;
        public int Defense => _defense;
        public int Speed => _speed;

        public float Radius => _radius;

        public CharacterStats(CharacterStats characterStats)
        {
            _maxHealth = characterStats.MaxHealth;
            _attack = characterStats.Attack;
            _defense = characterStats.Defense;
            _speed = characterStats.Speed;
            _radius = characterStats.Radius;
        }
        
        public void ApplyModifier(int modifier)
        {
            _currentHealth += modifier;
        }
    }
}