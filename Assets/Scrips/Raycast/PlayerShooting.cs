using System.Collections;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private float weaponRange = 20f;

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer lineRenderer;

    void Update()
    {
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

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Destroy(hit.collider.gameObject);
                Debug.Log("Kena Musuh!");
            }
        }
    }

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

        yield return new WaitForSeconds(0.05f); 
        lineRenderer.enabled = false;
    }
}