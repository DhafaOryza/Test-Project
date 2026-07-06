using TMPro;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Coin
{
    public class CoinUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _coinText;
        
        public void UpdateUI()
        {
            _coinText.text = $"Coin: {_coinText.text}";
        }
    }
}