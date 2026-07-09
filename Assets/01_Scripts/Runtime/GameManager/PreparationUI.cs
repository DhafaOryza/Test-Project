using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts.Runtime.GameManager
{
    public class PreparationUI : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.3f;

        public void SetTimer(float current, float max)
        {
            fillImage.fillAmount = current / max;
            //
            // timerText.text = Mathf.CeilToInt(current).ToString();
        }
        
        public void Show()
        {
            gameObject.SetActive(true);

            canvasGroup.DOKill();

            canvasGroup.alpha = 0;

            canvasGroup
                .DOFade(1, fadeDuration)
                .SetEase(Ease.OutQuad);
        }

        public void Hide()
        {
            canvasGroup.DOKill();

            canvasGroup
                .DOFade(0, fadeDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        }
    }
}