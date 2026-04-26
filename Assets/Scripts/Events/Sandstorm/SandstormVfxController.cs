using UnityEngine;

public class SandstormVfxController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem[] particleSystems;

    [Header("Direction Settings")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Vector3 forwardAxis = Vector3.forward;

    private void Awake()
    {
        visualRoot ??= transform;
    }

    public void PlayVfx(Vector3 windDirection)
    {
        SetDirection(windDirection);

        foreach (var part in particleSystems)
        {
            part?.Play();
        }
    }

    public void StopVfx()
    {
        foreach (var part in particleSystems)
        {
            part?.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void SetDirection(Vector3 windDirection)
    {
        Vector3 flatDirection = windDirection;
        flatDirection.y = 0f;

        if (flatDirection == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(flatDirection.normalized, Vector3.up);
        Quaternion axisCorrection = Quaternion.FromToRotation(forwardAxis.normalized, Vector3.forward);

        visualRoot.rotation = targetRotation * axisCorrection;
    }
}
