using UnityEngine;

[CreateAssetMenu(menuName = "Guns/Guns Data")]
public class WeaponData : ScriptableObject
{
    [Header("Shotgun")]
    public int pelletCount = 1;
    public float spreadAngle = 0;

    [Header("Ammo")]
    public int magazineSize = 30;

    [Header("Timing")]
    public float fireRate = 0.2f;
    public float reloadTime = 1.5f;

    [Header("Combat")]
    public float range = 15f;
    public int damage = 1;

    [Header("Fire Mode")]
    public bool isAutomatic = false;

    [Header("Visual")]
    public GameObject bulletTrailPrefab;

    [Header("Audio")]
    public AudioClip shootClip;
    public AudioClip reloadClip;
    public float shootVolume = 1f;
}