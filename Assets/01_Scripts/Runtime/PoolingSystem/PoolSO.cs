using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts.Runtime.PoolingSystem
{
    [CreateAssetMenu(fileName = "NewPoolCatalog", menuName = "System/Pool Catalog")]
    public class PoolCatalogSO : ScriptableObject
    {
        public string categoryName = "New Category";
        public List<PoolDataSO> pools = new List<PoolDataSO>();
    }
}