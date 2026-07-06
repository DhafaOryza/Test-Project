using System;
using _Dev.Script.Runtime.Core.Coin;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.SlotSystem
{
    public class RollSystem : MonoBehaviour
    {
        [Header("Roll Configuration")]
        [SerializeField]
        private int _startingPrice = 150;
        
        private int _currentPrice;
        
        [Header("Reference")]
        [SerializeField]
        private SlotSystem _slotSystem;

        public void ResetRollPrice()
        {
            _currentPrice = _startingPrice;
        }

        public void Roll()
        {
            if (CoinManager.Instance.CanUseCoin(_startingPrice))
            {
                _currentPrice += 50;
                _slotSystem.Roll();
            }
        }
    }
}