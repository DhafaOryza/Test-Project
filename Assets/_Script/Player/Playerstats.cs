using UnityEngine;

public class PlayerStats : MonoBehaviour
{   
    [Header ("Statistik player")]
    [SerializeField] private int maxHealth = 50;
    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;
    [SerializeField] private int maxShield = 10;
    public int CurrentShield { get; private set;}
    public int MaxShield => maxShield;

    public System.Action<int, int> OnHealthChanged;
    public System.Action<int , int> OnShieldChanged;
    public System.Action OnPlayerDied;

    public void Initialize()
    {
        CurrentHealth = maxHealth;
        CurrentShield = 0;

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        OnShieldChanged?.Invoke(CurrentShield, maxShield);
    }

    public bool isHealthFull()
    {
        return CurrentHealth >= maxHealth;
    }

    public void AddShield(int amount)
    {
        CurrentShield += amount;
        CurrentShield = Mathf.Clamp(CurrentShield, 0 ,maxShield);

        OnShieldChanged?.Invoke(CurrentShield, maxShield);
    }

    public void TakeDamage(int amount)
    {
        if (CurrentShield > 0)
        {
            if (amount <= CurrentShield)
            {
                CurrentShield -= amount;
                amount = 0;
            }
            else
            {
                amount -= CurrentShield;
                CurrentShield = 0;
            }

            OnShieldChanged?.Invoke(CurrentShield, maxShield);
        }

        if (amount > 0)
        {
            CurrentHealth -= amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

            if (CurrentHealth <= 0)
            {
                Debug.Log("Player kalah!");
                OnPlayerDied?.Invoke();
            }
        }
        
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
}