using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Survival Stats")]
    public int maxHealth = 5;
    public int currentHealth;
    public int armor = 0;
    public float magnetRadiusMultiplier = 1f;
    public float lifestealChance = 0f;

    [Header("Weapon Stats")]
    public float fireRateMultiplier = 1f;
    public float reloadMultiplier = 1f;
    public int bonusPierce = 0;

    public void Initialize()
    {
        Debug.Log("PlayerStats Initialize");

        currentHealth = maxHealth;
        armor = 0;
        magnetRadiusMultiplier = 1f;
        lifestealChance = 0f;

        fireRateMultiplier = 1f;
        reloadMultiplier = 1f;
        bonusPierce = 0;
    }
}
