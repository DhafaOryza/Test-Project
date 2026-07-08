using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed = 5f;

    // menggunakan LateUpdate() agar bisa membaca posisi player sebelum player selesai bergerak,
    private void LateUpdate()
    {
        if (target == null) return;

        // menghitung posisi tujuan menggunakan transform.position.z untuk mempertahankan posisi kamera walaupun kamera berpindah - pindah 
        Vector3 desiredPos = new Vector3(target.position.x, target.position.y, transform.position.z);

        // menggunakan perhitungan Lerp agar kamera halus saat berpindah - pindah 
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);
    }
}