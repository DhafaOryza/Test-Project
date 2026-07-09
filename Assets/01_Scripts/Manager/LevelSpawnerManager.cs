using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawnerManager : MonoBehaviour
{
    public PoolIdSO character1PoolId;
    public PoolIdSO character2PoolId;

    private PoolManager poolManager;

    private GameObject char1;
    private GameObject char2;

    public void Initialize()
    {
        poolManager = GameManager.Instance.poolManager;

        LevelSpawn();
    }

    private void LevelSpawn()
    {
        char1 = poolManager.Spawn(character1PoolId, new Vector3(-1f, 0, 0), Quaternion.identity);
        char2 = poolManager.Spawn(character2PoolId, new Vector3(1f, 0, 0), Quaternion.identity);

        StartCoroutine(CooldownForDelete());
    }

    private IEnumerator CooldownForDelete()
    {
        yield return new WaitForSeconds(3f);

        poolManager.Despawn(character2PoolId, char2);
    }
}
