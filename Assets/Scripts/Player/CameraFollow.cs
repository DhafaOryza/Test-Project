using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 5f;
    private Transform target;

    public void Initialize(Transform tehCameraTarget)
    {
        Debug.Log("cameraFollow Initialize");
        target =  tehCameraTarget;

        if (target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }

    // menggunakan LateUpdate() agar bisa membaca posisi player sebelum player selesai bergerak,
    private void LateUpdate()
    {
        // Langsung tembak ke GameManager untuk mencari cameraTarget!
        if (GameManager.Instance == null || GameManager.Instance.cameraTarget == null) 
        {
            return;
        }

        Transform targetKamera = GameManager.Instance.cameraTarget;

        Vector3 desiredPos = new Vector3(targetKamera.position.x, targetKamera.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);
    }
}