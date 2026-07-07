using UnityEngine;

namespace TopDown.Movement
{
    public class PlayerRotation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private Transform player;
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Time.timeScale == 0f) return;

            Vector2 mouseScreenPos = Input.mousePosition;
            HandleMouseLook(mouseScreenPos);
        }

        private void HandleMouseLook(Vector2 screenPos)
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(
                new Vector3(screenPos.x, screenPos.y, -mainCamera.transform.position.z)
            );
            
            worldPos.z = 0f;
            cameraTarget.position = Vector3.Lerp(player.position, worldPos, 0.25f);
        }
    }
}