using System.Collections;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private float weaponRange = 20f;
    [SerializeField] private int damage = 1; 

    [Header("References")]
    [SerializeField] private Transform firePoint; // Ujung senjata tempat peluru keluar
    [SerializeField] private LineRenderer lineRenderer; // Visual peluru

    void Update()
    {
        // Mengecek input klik kiri (Fire1) setiap frame
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // 1. Cari posisi mouse di layar, lalu ubah ke koordinat dunia game (World Point)
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Pastikan sumbu Z selalu 0 di game 2D

        // 2. Hitung arah tembakan: (Tujuan - Asal)
        Vector2 direction = (mousePosition - firePoint.position).normalized;

        // 3. Tembakkan garis gaib (Raycast)
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, weaponRange);

        // 4. Proses visual dan logika tabrakan
        StartCoroutine(ShowShootEffect(direction, hit));

        if (hit.collider != null) // Jika Raycast menabrak sesuatu
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // Pareto: Untuk sekarang kita buat musuhnya mati dalam 1x tembak.
                // Nantinya kamu bisa ganti ini dengan script EnemyHealth kalau mau lebih kompleks.
                Destroy(hit.collider.gameObject);
                Debug.Log("Kena Musuh!");
            }
        }
    }

    // Coroutine untuk membuat efek peluru kilat
    private IEnumerator ShowShootEffect(Vector2 direction, RaycastHit2D hit)
    {
        lineRenderer.enabled = true;
        
        // Titik awal garis (ujung senjata)
        lineRenderer.SetPosition(0, firePoint.position);

        if (hit.collider != null)
        {
            // Jika nabrak, garis berhenti di titik tabrakan
            lineRenderer.SetPosition(1, hit.point); 
        }
        else
        {
            // Jika meleset, garis diteruskan sampai batas maksimal jarak tembak
            lineRenderer.SetPosition(1, (Vector2)firePoint.position + direction * weaponRange);
        }

        // Tunggu sekian milidetik lalu matikan garisnya (memberi kesan peluru cepat)
        yield return new WaitForSeconds(0.05f); 
        lineRenderer.enabled = false;
    }
}