using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPoolCatalog", menuName = "Pooling/Pool Catalog")]
public class PoolCatalogSO : ScriptableObject
{
    [Header("List of Pools")]
    public List<PoolDataSO> pools = new List<PoolDataSO>();
}