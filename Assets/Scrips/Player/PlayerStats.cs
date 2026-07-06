using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Survival Stats")]
    public int maxHealth = 3;
    public int currentHealth = 3;
    public int armor = 0;
    public float magnetRadiusMultiplier = 1f;
    public float lifestealChance = 0f;

    [Header("Weapon Stats")]
    public float fireRateMultiplier = 1f;
    public float reloadMultiplier = 1f;
    public int bonusPierce = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
