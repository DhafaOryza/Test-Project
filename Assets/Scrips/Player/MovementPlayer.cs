using System.Collections;
using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 input;

    // agar PlayerAnimation bisa mengambil pergerakan player
    public Vector2 MoveInput => input;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // mengambil refereni horizontal dan vertical 
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        // dan melakukan Normalize agar kecepatan konsisten dnegan yang lainnya
        input.Normalize();
    }

    private void FixedUpdate()
    {
        // mengambil rigidbody dan mengkalikan input dengan speed
        rb.linearVelocity = input * speed;
    }
}
