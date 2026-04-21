using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform lookAt;
    [SerializeField] private float followSmoothSpeed = 8f;

    [Header("Shake")]
    [SerializeField] private float shakeSpeed = 18f;
    [SerializeField] private float impactShakeFadeSpeed = 4f;

    private Vector3 lookOffset;
    private Vector3 currentBasePosition;

    private Transform overrideTarget;
    private bool useOverrideTarget = false;
    private float currentSmoothSpeed;

    private float collapseShakeAmount = 0f;
    private float currentImpactShake = 0f;

    private float noiseSeedX;
    private float noiseSeedY;

    private void Start()
    {
        lookOffset = transform.position - lookAt.position;
        currentSmoothSpeed = followSmoothSpeed;

        noiseSeedX = Random.Range(0f, 100f);
        noiseSeedY = Random.Range(100f, 200f);

        currentBasePosition = transform.position;
    }

    private void LateUpdate()
    {
        Transform currentTarget = GetCurrentTarget();

        Vector3 targetPosition = currentTarget.position + lookOffset;

        currentBasePosition = Vector3.Lerp(
            currentBasePosition,
            targetPosition,
            Time.deltaTime * currentSmoothSpeed
        );

        transform.position = currentBasePosition + GetShakeOffset();

        currentImpactShake = Mathf.Lerp(currentImpactShake, 0f, Time.deltaTime * impactShakeFadeSpeed);
    }

    private Transform GetCurrentTarget()
    {
        if (useOverrideTarget)
        {
            return overrideTarget;
        }

        return lookAt;
    }

    public void SetOverrideTarget(Transform newTarget, float overrideSmoothSpeed)
    {
        overrideTarget = newTarget;
        useOverrideTarget = true;
        currentSmoothSpeed = overrideSmoothSpeed;
    }

    public void ClearOverrideTarget()
    {
        overrideTarget = null;
        useOverrideTarget = false;
        currentSmoothSpeed = followSmoothSpeed;
    }

    public Transform GetFollowTarget()
    {
        return lookAt;
    }

    public void SetCollapseShake(float amount)
    {
        collapseShakeAmount = amount;
    }

    public void AddImpactShake(float amount)
    {
        if (amount > currentImpactShake)
        {
            currentImpactShake = amount;
        }
    }

    private Vector3 GetShakeOffset()
    {
        float totalShake = collapseShakeAmount + currentImpactShake;

        if (totalShake <= 0f)
        {
            return Vector3.zero;
        }

        float sampleX = (Mathf.PerlinNoise(noiseSeedX, Time.time * shakeSpeed) - 0.5f) * 2f;
        float sampleY = (Mathf.PerlinNoise(noiseSeedY, Time.time * shakeSpeed) - 0.5f) * 2f;

        return new Vector3(sampleX, sampleY, 0f) * totalShake;
    }
}
