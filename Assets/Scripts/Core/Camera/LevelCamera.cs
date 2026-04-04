using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform lookAt;
    [SerializeField] private float followSmoothSpeed = 8f;

    private Vector3 lookOffset;

    private Transform overrideTarget;
    private bool useOverrideTarget = false;
    private float currentSmoothSpeed;

    private void Start()
    {
        if (lookAt != null)
        {
            lookOffset = transform.position - lookAt.position;
        }

        currentSmoothSpeed = followSmoothSpeed;
    }

    private void LateUpdate()
    {
        Transform currentTarget = GetCurrentTarget();

        if (currentTarget == null)
        {
            return;
        }

        Vector3 targetPosition = currentTarget.position + lookOffset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * currentSmoothSpeed
        );
    }

    private Transform GetCurrentTarget()
    {
        if (useOverrideTarget && overrideTarget != null)
        {
            return overrideTarget;
        }

        return lookAt;
    }

    public void SetOverrideTarget(Transform newTarget, float overrideSmoothSpeed)
    {
        overrideTarget = newTarget;
        useOverrideTarget = overrideTarget != null;
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
}
