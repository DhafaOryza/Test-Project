using UnityEngine;

namespace TopDown.Combat
{
    public class BulletTrail : MonoBehaviour
    {
        [SerializeField] private float speed = 60f;

        private Vector3 startPosition;
        private Vector3 targetPosition;
        private float progress;

        public void SetTargetPosition(Vector3 target)
        {
            startPosition = transform.position;
            targetPosition = target;
            progress = 0f;
        }

        void Update()
        {
            float distance = Vector3.Distance(startPosition, targetPosition);
            if (distance <= 0.01f) return;

            progress += Time.deltaTime * speed / distance;
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress);

            if (progress >= 1f)
                Destroy(gameObject);
        }

    }
}
