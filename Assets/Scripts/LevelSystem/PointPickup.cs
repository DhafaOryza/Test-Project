using UnityEngine;

public class PointPickup : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private int value = 1;
    [SerializeField] private float pickupRadius = 2f;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private PoolIdSO myPoolId;

    private Transform player;

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            player = GameManager.Instance.player;
        }
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

        GameManager.Instance.poolManager.Despawn(myPoolId, gameObject);
    }
}