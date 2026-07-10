using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public List<PoolCatalogSO> catalogs;
    private Dictionary<PoolIdSO, Queue<GameObject>> poolDictionary;
    private Dictionary<PoolIdSO, PoolDataSO> poolDataDictionary;
    private Transform poolContainer;

    public void Initialize()
    {
        //Debug.Log("[INIT 3] PoolManager: Initialize called...");
        poolContainer = new GameObject("PoolContainer").transform;
        poolContainer.SetParent(transform);
        InitializeCatalogs();
    }

    private void InitializeCatalogs()
    {
        poolDictionary = new Dictionary<PoolIdSO, Queue<GameObject>>();
        poolDataDictionary = new Dictionary<PoolIdSO, PoolDataSO>();

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
                    Debug.LogError($"[PoolManager] '{data.name}' (poolId={data.poolId.name}) — prefab NULL/Missing, pool NOT registered. " +
                                   "Spawn via this poolId will fail silently. Fix: assign prefab in Inspector.");
                    continue;
                }
                if (poolDictionary.ContainsKey(data.poolId)) continue;

                poolDataDictionary[data.poolId] = data;
                Queue<GameObject> newPool = new Queue<GameObject>();

                GameObject folder = new GameObject($"Pool_{data.poolId.name}");
                folder.transform.SetParent(poolContainer);

                for (int i = 0; i < data.initialSize; i++)
                {
                    GameObject obj = CreateNewObject(data, folder.transform);
                    newPool.Enqueue(obj);
                }

                poolDictionary.Add(data.poolId, newPool);
            }
        }
    }

    private GameObject CreateNewObject(PoolDataSO data, Transform parentFolder)
    {
        GameObject obj = Instantiate(data.prefab, parentFolder);
        obj.SetActive(false);
        return obj;
    }

    public GameObject Spawn(PoolIdSO poolId, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (poolId == null)
        {
            Debug.LogWarning("[PoolManager] Spawn called with poolId=NULL.");
            return null;
        }
        if (!poolDictionary.ContainsKey(poolId))
        {
            Debug.LogWarning($"[PoolManager] Pool '{poolId.name}' NOT registered. " +
                             "Possible causes: PoolDataSO has prefab=null, or the pool was never registered in any catalog. " +
                             "Spawn skipped — encounter may appear empty.");
            return null;
        }

        GameObject objToSpawn = null;
        PoolDataSO data = poolDataDictionary[poolId];
        Transform folder = poolContainer.Find($"Pool_{poolId.name}");

        if (poolDictionary[poolId].Count > 0)
        {
            objToSpawn = poolDictionary[poolId].Dequeue();
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
        if (poolId == null || !poolDictionary.ContainsKey(poolId))
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        Transform folder = poolContainer.Find($"Pool_{poolId.name}");
        obj.transform.SetParent(folder);
        poolDictionary[poolId].Enqueue(obj);
    }
}