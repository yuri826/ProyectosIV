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
        if (visualRoot == null)
        {
            visualRoot = transform;
        }
    }

    public void PlayVfx(Vector3 windDirection)
    {
        SetDirection(windDirection);

        for (int i = 0; i < particleSystems.Length; i++)
        {
            if (particleSystems[i] == null)
            {
                continue;
            }

            particleSystems[i].Play();
        }
    }

    public void StopVfx()
    {
        for (int i = 0; i < particleSystems.Length; i++)
        {
            if (particleSystems[i] == null)
            {
                continue;
            }

            particleSystems[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    public void SetDirection(Vector3 windDirection)
    {
        if (visualRoot == null)
        {
            return;
        }

        Vector3 flatDirection = windDirection;
        flatDirection.y = 0f;

        if (flatDirection == Vector3.zero)
        {
            return;
        }

        visualRoot.rotation = Quaternion.LookRotation(flatDirection.normalized, Vector3.up);
    }
}
