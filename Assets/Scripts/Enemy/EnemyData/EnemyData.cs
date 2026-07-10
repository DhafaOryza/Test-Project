using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public string enemyName;

    [Header("Base Stats")]
    public float maxHealth = 3f;
    public float speed = 3f;
    public float damage = 1f;
    public float attackCooldown = 2f;
    public int dropAmount = 1;
}
