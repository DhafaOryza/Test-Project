using System;
using _01_Scripts.Runtime.Core.Coin;
using TMPro;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.SlotSystem
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

        [SerializeField] 
        private TMP_Text _rollPriceText;

        private void Start()
        {
            ResetRollPrice();
        }

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