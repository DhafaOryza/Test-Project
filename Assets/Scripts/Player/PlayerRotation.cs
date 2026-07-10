using UnityEngine;

namespace TopDown.Movement
{
    public class PlayerRotation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private Transform player;
        private Camera mainCamera;

        public void Initialize(Transform playerTransform, Transform cameraTargetTransform, Camera camera)
        {
            Debug.Log("PlayerRotation Initialize");

            player = playerTransform;
            cameraTarget = cameraTargetTransform;
            mainCamera = camera;
        }

        private void Update()
        {
            if (Time.timeScale == 0f) return;

            // mengambil posisi mouse dengan Vector2
            Vector2 mouseScreenPos = Input.mousePosition;
            HandleMouseLook(mouseScreenPos);
        }

        private void HandleMouseLook(Vector2 screenPos)
        {
            // mengambil posisi mouse pada mainCamera ke world position
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, transform.position.z));
            
            // mematikan world position pada Z
            worldPos.z = 0f;

            // hasil dari pergeseran kamera target dengan menggunakan Lerp
            cameraTarget.position = Vector3.Lerp(player.position, worldPos, 0.25f);
        }
    }
}