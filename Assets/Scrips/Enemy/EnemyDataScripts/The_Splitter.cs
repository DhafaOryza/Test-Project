using UnityEngine;

public class The_Splitter : MonoBehaviour
{
    [Header("Split Settings")]
    public GameObject miniEnemyPrefab;
    public int splitAmmount = 2;
    public float scatterForce = 3f;

    public void SpawnMinis()
    {
        for (int i = 0; i < splitAmmount; i++)
        {
            // 1. Perbesar jarak offset (ubah dari 0.05f menjadi 0.5f atau lebih)
            Vector2 randomOffset = Random.insideUnitCircle * 1f; 
            
            // Simpan objek yang baru dispawn ke dalam variabel
            GameObject miniEnemy = Instantiate(miniEnemyPrefab, (Vector2)transform.position + randomOffset, Quaternion.identity);

            // 2. Berikan efek dorongan (scatter) agar mereka terpental saling menjauh
            Rigidbody2D rb = miniEnemy.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // AddForce untuk pemaksaan terpental
                rb.AddForce(randomOffset.normalized * scatterForce, ForceMode2D.Impulse);
            }
        }
    }
}