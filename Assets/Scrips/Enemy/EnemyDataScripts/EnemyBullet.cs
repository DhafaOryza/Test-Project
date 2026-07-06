using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float bulletDamage;

    void Start()
    {
        Destroy(gameObject,5f);
    }

    public void Setdamage(float damageAmount)
    {
        bulletDamage = damageAmount;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // jika peluru megenai player maka akan memberikan damage
        if(collision.CompareTag("Player"))
        {
            PlayerHealth player = collision.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(bulletDamage);
            }
            Destroy(gameObject);
        }
        // jika peluru mengenai tembok/penghalang maka akan menghilang.
        else if (collision.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
