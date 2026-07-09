using TMPro;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Coin
{
    public class CoinUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _coinText;
        
        public void UpdateUI(int amount)
        {
            _coinText.text = $"{amount}";
        }
    }
}