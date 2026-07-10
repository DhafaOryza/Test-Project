using UnityEngine;

public class The_Splitter : EnemyMelee
{
    [Header("Split Settings")]
    public GameObject miniEnemyPrefab;
    public int splitAmount = 2;
    public float scatterForce = 3f;

    protected override void Die()
    {
        SpawnMinis();
        base.Die();
    }

    private void SpawnMinis()
    {
        for (int i = 0; i < splitAmount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * 1f; 
            GameObject miniEnemy = Instantiate(miniEnemyPrefab, (Vector2)transform.position + randomOffset, Quaternion.identity);

            Rigidbody2D miniRb = miniEnemy.GetComponent<Rigidbody2D>();
            if (miniRb != null)
            {
                miniRb.AddForce(randomOffset.normalized * scatterForce, ForceMode2D.Impulse);
            }
        }
    }
}