using UnityEngine;

public class PointPickup : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private int value = 1;
    [SerializeField] private float pickupRadius = 2f;
    [SerializeField] private float moveSpeed = 8f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null) return;
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= pickupRadius)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerPoints playerPoints = other.GetComponent<PlayerPoints>();

        if (playerPoints != null)
        {
            playerPoints.AddPoint(value);
        }

        Destroy(gameObject);
    }
}