using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Coroutine currentShake;
    private bool isLocked = false;

    private Vector3 originalPos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        transform.localPosition = Vector3.zero;
    }

    public void TriggerShake(float duration, float magnitude)
    {
        if (isLocked) return;

        if (currentShake != null) StopCoroutine(currentShake);
        currentShake = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    public void TriggerLockedShake(float duration, float magnitude)
    {
        isLocked = true;

        if (currentShake != null) StopCoroutine(currentShake);
        currentShake = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    public void UnlockShake()
    {
        isLocked = false;
        transform.localPosition = Vector3.zero;
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = Vector3.zero;
        currentShake = null;
    }
}