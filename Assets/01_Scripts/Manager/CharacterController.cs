using Mono.Cecil.Cil;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public PoolIdSO poolId;
    PoolManager poolManager;

    GameObject characterObject;
    public void Initialize()
    {
        poolManager = GameManager.Instance.poolManager;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
    }
}
