using System;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Character
{
    public class CharacterDetection : MonoBehaviour
    {
        [SerializeField]
        private string targetTag;

        public event Action<CharacterController> OnCollisionDetected;
        public event Action<CharacterController> OnCollisionOut;

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log(other.gameObject.name);
            if (other.gameObject.CompareTag(targetTag))
            {
                if (other.gameObject.TryGetComponent(out CharacterController character))
                {
                    Debug.Log("Ada");
                    OnCollisionDetected?.Invoke(character);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(targetTag))
            {
                if (other.gameObject.TryGetComponent(out CharacterController character))
                { 
                    OnCollisionOut?.Invoke(character);
                }
            }
        }
    }}