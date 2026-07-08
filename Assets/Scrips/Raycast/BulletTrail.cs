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
            // mengambil posisi awal,target dengan Vector3 dan progress degan float 
            startPosition = transform.position;
            targetPosition = target;
            progress = 0f;
        }

        void Update()
        {
            // memasukkan jarak dari start position dan target position
            float distance = Vector3.Distance(startPosition, targetPosition);
            if (distance <= 0.01f) return;

            // perhitungan untuk menembak
            progress += Time.deltaTime * speed / distance;

            // menggunakan perhitungan Lerp untuk menentukan posisi
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress);

            // menghapus bullet trial
            if (progress >= 1f)
                Destroy(gameObject);
        }

    }
}
