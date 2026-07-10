using UnityEngine;

public class Background : MonoBehaviour
{
    [Header("Pengaturan Kecepatan")]
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
        // menghitung offset dari camera pada x dan y kemudian di kalikan dengan scroll speed
        Vector2 Offset = new Vector2(cam.position.x, cam.position.y) * scrollSpeed;
        mat.mainTextureOffset = Offset;
    }
}
