using UnityEngine;

namespace _01_Scripts.Runtime.Interaction
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

    [RequireComponent(typeof(Collider2D))]
    public class CharacterInteraction : MonoBehaviour
    {
        public event Action DragStarted;
        public event Action Dragging;
        public event Action DragEnded;

        [SerializeField] private Collider2D boundary;

        private Collider2D _selfCollider;

        private bool _isDragging;
        private Vector3 _offset;

        void Awake()
        {
            _selfCollider = GetComponent<Collider2D>();
        }

        public void SetBoundary(Collider2D area)
        {
            boundary = area;
            Debug.Log("Setting boundary");
        }

        private void OnMouseDown()
        {
            Debug.Log("OnMouseDown");
            Vector3 mouse = MousePosition();

            _offset = transform.position - mouse;

            _isDragging = true;

            DragStarted?.Invoke();
        }

        private void OnMouseDrag()
        {
            if (!_isDragging)
                return;

            Vector3 target = MousePosition() + _offset;

            transform.position = Clamp(target);

            Dragging?.Invoke();
        }

        private void OnMouseUp()
        {
            if (!_isDragging)
                return;

            _isDragging = false;

            DragEnded?.Invoke();
        }

        private Vector3 MousePosition()
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            pos.z = transform.position.z;

            return pos;
        }

        private Vector3 Clamp(Vector3 target)
        {
            if (boundary == null)
                return target;

            Bounds area = boundary.bounds;
            Bounds self = _selfCollider.bounds;

            float x = self.extents.x;
            float y = self.extents.y;

            target.x = Mathf.Clamp(target.x,
                area.min.x + x,
                area.max.x - x);

            target.y = Mathf.Clamp(target.y,
                area.min.y + y,
                area.max.y - y);

            return target;
        }
    }
}