using TMPro;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Health
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _healthText;

        public void UpdateHealth(int health)
        {
            _healthText.text = health.ToString();
        }
    }
}