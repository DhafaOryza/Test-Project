using System;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Health
{
    public class Base : MonoBehaviour
    {
        [SerializeField] 
        private string _tagTarget;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(_tagTarget))
            {
                HealthManager.Instance.TakeDamage(1);
                Destroy(other.gameObject);
            }
        }
    }
}