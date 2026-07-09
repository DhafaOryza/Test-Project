using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPoolCatalog", menuName = "System/Pool Catalog")]
public class PoolCatalogSO : ScriptableObject 
{
    public string categoryName = "New Category";
    public List<PoolDataSO> pools = new List<PoolDataSO>();

}