using UnityEngine;
using UnityEngine.UI;

namespace _Dev.Script.Runtime.Core.Health
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] 
        private Image _fillImage;
        
        public void SetHealthBar(float currentHealth, float maxHealth)
        {
            _fillImage.fillAmount = currentHealth / maxHealth;
        }
    }
}