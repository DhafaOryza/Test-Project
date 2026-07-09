using UnityEngine;

[CreateAssetMenu(fileName = "NewPoolData", menuName = "System/Pool Data")]
public class PoolDataSO : ScriptableObject
{
    public PoolIdSO poolId;
    public GameObject prefab;
    public int initialsize = 10;
    public bool CanExpanded = true;
}