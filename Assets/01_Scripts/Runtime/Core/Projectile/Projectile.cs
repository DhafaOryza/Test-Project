using _01_Scripts.Runtime.PoolingSystem;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Projectile
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] 
        private float speed = 10f;
        [SerializeField]
        private PoolIdSO projectilePoolId;

        private Transform _target;

        public void Shoot(Transform target)
        {
            _target = target;
        }

        private void Update()
        {
            if (_target == null)
            {
                gameObject.SetActive(false);
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, _target.position, speed * Time.deltaTime);

            if (Vector3.SqrMagnitude(transform.position - _target.position) < 0.01f)
            {
                Hit();
            }
        }

        private void Hit()
        {
            // TODO: Damage target

            GameManager.GameManager.Instance.PoolManager.Despawn(projectilePoolId, gameObject);
        }
    }
}