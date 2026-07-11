using System.Diagnostics;
using _01_Scripts.Runtime.Interface;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace _01_Scripts.Runtime.Interaction
{
    public class StatsInfoPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private RectTransform labelRect;
        [SerializeField] private Camera worldCamera;
        [SerializeField] private Vector3 worldOffset = Vector3.up;

        private Transform target;
        private Canvas canvas;
        private RectTransform canvasRect;

        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            canvasRect = canvas.GetComponent<RectTransform>();

            Hide();
        }

        private void LateUpdate()
        {
            if (target == null)
                return;

            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
                worldCamera,
                target.position + worldOffset);

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    screenPos,
                    null,
                    out Vector2 localPos))
            {
                labelRect.anchoredPosition = localPos;
            }
        }

        // public void Show(IStatsInfoz interactable)
        // {
        //     target = interactable.Transform;
        //     label.text = interactable.DisplayName;
        //     labelRect.gameObject.SetActive(true);
        // }

        public void Hide()
        {
            target = null;
            labelRect.gameObject.SetActive(false);
        }
    }
}