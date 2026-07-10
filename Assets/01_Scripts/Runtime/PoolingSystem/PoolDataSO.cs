using UnityEngine;

namespace _01_Scripts.Runtime.PoolingSystem
{
    [CreateAssetMenu(fileName = "NewPoolData", menuName = "System/Pool Data")]
    public class PoolDataSO : ScriptableObject
    {
        public PoolIdSO poolId;
        public GameObject prefab;
        public int initialSize = 10;
        public bool isExpandable = true;
    }
}