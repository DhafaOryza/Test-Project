using UnityEngine;

namespace TopDown.Movement
{
    public class PlayerRotation : MonoBehaviour
    {
        public void Initialize(Transform playerTransform, Transform cameraTargetTransform, Camera camera)
        {
            Debug.Log("PlayerRotation Initialize");

            GameManager.Instance.player = playerTransform;
            GameManager.Instance.cameraTarget = cameraTargetTransform;
            GameManager.Instance.mainCamera = camera;
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
            Vector3 worldPos = GameManager.Instance.mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, transform.position.z));
            
            // mematikan world position pada Z
            worldPos.z = 0f;

            // hasil dari pergeseran kamera target dengan menggunakan Lerp
            GameManager.Instance.cameraTarget.position = Vector3.Lerp(GameManager.Instance.player.position, worldPos, 0.25f);
        }
    }
}