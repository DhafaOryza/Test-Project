using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts.Runtime.PoolingSystem
{
    public class PoolManager : MonoBehaviour
    {
        [SerializeField] 
        private List<PoolCatalogSO> catalogs;
        private Dictionary<PoolIdSO, Queue<GameObject>> _poolDictionary;
        private Dictionary<PoolIdSO, PoolDataSO> _poolDataDictionary;
        private Transform _poolContainer;

        public void Initialize()
        {
            Debug.Log("[INIT 3] PoolManager: Initialize called...");
            _poolContainer = new GameObject("PoolContainer").transform;
            _poolContainer.SetParent(transform);
            InitializeCatalogs();
        }

        private void InitializeCatalogs()
        {
            _poolDictionary = new Dictionary<PoolIdSO, Queue<GameObject>>();
            _poolDataDictionary = new Dictionary<PoolIdSO, PoolDataSO>();

            if (catalogs == null || catalogs.Count == 0)
            {
                Debug.LogWarning("[PoolManager] Catalogs are empty!");
                return;
            }

            foreach (PoolCatalogSO catalog in catalogs)
            {
                if (catalog == null) continue;
                foreach (PoolDataSO data in catalog.pools)
                {
                    if (data == null) continue;
                    if (data.poolId == null)
                    {
                        Debug.LogWarning($"[PoolManager] '{data.name}' — poolId NULL, skipped.");
                        continue;
                    }

                    if (data.prefab == null)
                    {
                        Debug.LogError(
                            $"[PoolManager] '{data.name}' (poolId={data.poolId.name}) — prefab NULL/Missing, pool NOT registered. " +
                            "Spawn via this poolId will fail silently. Fix: assign prefab in Inspector.");
                        continue;
                    }

                    if (_poolDictionary.ContainsKey(data.poolId)) continue;

                    _poolDataDictionary[data.poolId] = data;
                    Queue<GameObject> newPool = new Queue<GameObject>();

                    GameObject folder = new GameObject($"Pool_{data.poolId.name}");
                    folder.transform.SetParent(_poolContainer);

                    for (int i = 0; i < data.initialSize; i++)
                    {
                        GameObject obj = CreateNewObject(data, folder.transform);
                        newPool.Enqueue(obj);
                    }

                    _poolDictionary.Add(data.poolId, newPool);
                }
            }
        }

        private GameObject CreateNewObject(PoolDataSO data, Transform parentFolder)
        {
            GameObject obj = Instantiate(data.prefab, parentFolder);
            obj.SetActive(false);
            return obj;
        }

        public T Spawn<T>(PoolIdSO poolId, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            if (poolId == null)
            {
                Debug.LogWarning("[PoolManager] Spawn called with poolId=NULL.");
                return null;
            }

            if (!_poolDictionary.ContainsKey(poolId))
            {
                Debug.LogWarning($"[PoolManager] Pool '{poolId.name}' NOT registered. " +
                                 "Possible causes: PoolDataSO has prefab=null, or the pool was never registered in any catalog. " +
                                 "Spawn skipped — encounter may appear empty.");
                return null;
            }
            
            GameObject obj = Spawn(poolId, position, rotation, parent);

            if (obj == null)
                return null;

            if (obj.TryGetComponent(out T component))
                return component;

            Debug.LogError($"{obj.name} doesn't have component {typeof(T).Name}");
            return null;
        }

        public GameObject Spawn(PoolIdSO poolId, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (poolId == null)
            {
                Debug.LogWarning("[PoolManager] Spawn called with poolId=NULL.");
                return null;
            }

            if (!_poolDictionary.ContainsKey(poolId))
            {
                Debug.LogWarning($"[PoolManager] Pool '{poolId.name}' NOT registered. " +
                                 "Possible causes: PoolDataSO has prefab=null, or the pool was never registered in any catalog. " +
                                 "Spawn skipped — encounter may appear empty.");
                return null;
            }

            GameObject objToSpawn = null;
            PoolDataSO data = _poolDataDictionary[poolId];
            Transform folder = _poolContainer.Find($"Pool_{poolId.name}");

            if (_poolDictionary[poolId].Count > 0)
            {
                objToSpawn = _poolDictionary[poolId].Dequeue();
            }
            else if (data.isExpandable)
            {
                objToSpawn = CreateNewObject(data, folder);
            }

            if (objToSpawn != null)
            {
                objToSpawn.transform.position = position;
                objToSpawn.transform.rotation = rotation;

                if (parent != null)
                {
                    objToSpawn.transform.SetParent(parent);
                }

                objToSpawn.SetActive(true);
            }

            return objToSpawn;
        }

        public void Despawn(PoolIdSO poolId, GameObject obj)
        {
            if (poolId == null || !_poolDictionary.ContainsKey(poolId))
            {
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            Transform folder = _poolContainer.Find($"Pool_{poolId.name}");
            obj.transform.SetParent(folder);
            _poolDictionary[poolId].Enqueue(obj);
        }
    } 
}