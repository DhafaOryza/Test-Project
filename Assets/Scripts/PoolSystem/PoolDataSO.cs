using UnityEngine;

[CreateAssetMenu(fileName = "NewPoolData", menuName = "Pooling/Pool Data")]
public class PoolDataSO : ScriptableObject
{
    [Header("Pool Configuration")]
    public PoolIdSO poolId;
    public GameObject prefab;
    
    [Header("Pool Settings")]
    public int initialSize = 10;
    public bool isExpandable = true;
}