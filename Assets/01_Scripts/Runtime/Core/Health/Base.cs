using System;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Health
{
    public class Base : MonoBehaviour
    {
        [SerializeField] 
        private string _tagTarget;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(_tagTarget))
            {
                GameManager.GameManager.Instance.HealthManager.TakeDamage(1);
                Destroy(other.gameObject);
            }
        }
    }
}