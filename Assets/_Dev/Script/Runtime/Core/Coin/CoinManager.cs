using LumineREx.Utils.Singleton;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Coin
{
    public class CoinManager : Singleton<CoinManager>
    {
        [SerializeField]
        private CoinUI _coinUI;
        
        [SerializeField]
        private int _coinAmount;
        
        public int CoinAmount => _coinAmount;

        
        public void AddCoin(int amount)
        {
            _coinAmount += amount;
            _coinUI.UpdateUI(_coinAmount);
        }

        public bool CanUseCoin(int amount)
        {
            if (_coinAmount >= amount)
            {
                _coinAmount -= amount;
                _coinUI.UpdateUI(_coinAmount);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}