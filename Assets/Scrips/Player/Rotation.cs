using UnityEngine;

namespace TopDown.Movement
{
    public class Rotation : MonoBehaviour
    {
        [SerializeField] protected float spriteAngleOffset = -90f;

        protected void LookAt(Vector3 targetPosition)
        {
            Vector2 direction = targetPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteAngleOffset);
        }
    }
}
