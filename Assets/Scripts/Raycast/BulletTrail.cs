using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BulletTrail : MonoBehaviour
{
    [SerializeField] private float speed = 60f;
    [SerializeField] private PoolIdSO myPoolId;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float progress;
    private bool initialized;

    private void OnEnable()
    {
        initialized = false;
        progress = 0f;

        // Debug.Log($"[Trail {GetInstanceID()}] OnEnable");
    }

    public void SetTargetPosition(Vector3 target)
    {
        startPosition = transform.position;
        targetPosition = target;
        progress = 0f;
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;

        float distance = Vector3.Distance(startPosition, targetPosition);

        if (distance <= 0.01f)
        {
            initialized = false;
            GameManager.Instance.poolManager.Despawn(myPoolId, gameObject);
            return;
        }

        progress += Time.deltaTime * (speed / distance);
        transform.position = Vector3.Lerp(startPosition, targetPosition, progress);

        if (progress >= 1f)
        {
            // Debug.Log("cek Despwan");

            initialized = false;
            GameManager.Instance.poolManager.Despawn(myPoolId, gameObject);
        }
    }

    private void OnDestroy()
    {
        Debug.Log($"'{name}' was Detroyed.\n {new StackTrace(true)}", this);
    }
}