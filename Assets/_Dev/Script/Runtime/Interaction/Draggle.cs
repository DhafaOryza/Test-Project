using UnityEngine;

namespace _Dev.Script.Runtime.Interaction
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

    [RequireComponent(typeof(Collider2D))]
    public class Draggle : MonoBehaviour
    {
        public event Action DragStarted;
        public event Action Dragging;
        public event Action DragEnded;

        [SerializeField] private Collider2D boundary;

        private Collider2D selfCollider;

        private bool isDragging;
        private Vector3 offset;

        void Awake()
        {
            selfCollider = GetComponent<Collider2D>();
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

            offset = transform.position - mouse;

            isDragging = true;

            DragStarted?.Invoke();
        }

        private void OnMouseDrag()
        {
            if (!isDragging)
                return;

            Vector3 target = MousePosition() + offset;

            transform.position = Clamp(target);

            Dragging?.Invoke();
        }

        private void OnMouseUp()
        {
            if (!isDragging)
                return;

            isDragging = false;

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
            Bounds self = selfCollider.bounds;

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