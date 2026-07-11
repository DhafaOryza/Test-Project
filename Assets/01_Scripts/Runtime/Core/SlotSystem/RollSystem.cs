using System;
using System.Collections.Generic;
using _01_Scripts.Runtime.Core.Coin;
using _01_Scripts.Runtime.PoolingSystem;
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
        private TMP_Text _rollCostText;

        private void Start()
        {
            ResetRollPrice();
        }

        public void ResetRollPrice()
        {
            _currentPrice = _startingPrice;
            _rollCostText.text = _currentPrice.ToString();
        }

        public void Roll()
        {
            if (GameManager.GameManager.Instance.CoinManager.CanUseCoin(_startingPrice))
            {
                _currentPrice += 50;
                _slotSystem.Roll();
                _rollCostText.text = _currentPrice.ToString();
            }
        }
    }
}