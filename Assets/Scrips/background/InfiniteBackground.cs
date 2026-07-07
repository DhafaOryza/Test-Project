using UnityEngine;

public class Background : MonoBehaviour
{
    [Header("Pengaturan Kecepatan")]
    [Tooltip("Ubah angka ini agar pergeseran lantai pas dengan kecepatan lari Player")]
    [SerializeField] private float scrollSpeed = 0.5f;

    private Material mat;
    private Transform cam;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        cam = Camera.main.transform;
    }

    void Update()
    {
        Vector2 Offset = new Vector2(cam.position.x, cam.position.y) * scrollSpeed;
        mat.mainTextureOffset = Offset;
    }
}
