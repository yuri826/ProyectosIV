using UnityEngine;

public class SandstormSystem : MonoBehaviour
{
    [Header("Sandstorm State")]
    [SerializeField] private bool isSandstormActive = false;
    [SerializeField] private float remainingDuration = 0f;

    [Header("Wind Settings")]
    [SerializeField] [Range(0f, 2f)] private float windStrengthMultiplier = 0.8f;
    [SerializeField] private Vector3 currentWindDirection = Vector3.right;
    
    [Header("Visual & Audio")]
    [SerializeField] private SandstormVfxController sandstormVfxController;
    [SerializeField] private AudioSource sandstormAudioSource;

    [Header("Outlaws")]
    [SerializeField] private OutlawSystem[] outlawsInScene;

    public static SandstormSystem Instance;

    private void Awake()
    {
        Instance = this;
    }
    
    private void Update()
    {
        if (!isSandstormActive)
        {
            return;
        }

        remainingDuration -= Time.deltaTime;

        if (remainingDuration <= 0f)
        {
            StopSandstorm();
        }
    }

    public void StartSandstorm(float duration)
    {
        remainingDuration = duration;
        isSandstormActive = true;

        currentWindDirection = GetRandomWindDirection();

        ApplySandstormToOutlaws(true);
        UpdateVisuals(true);
    }

    public void StopSandstorm()
    {
        isSandstormActive = false;
        remainingDuration = 0f;

        ApplySandstormToOutlaws(false);
        UpdateVisuals(false);
    }

    public bool IsSandstormActive()
    {
        return isSandstormActive;
    }

    public Vector3 GetWindDirection()
    {
        return currentWindDirection.normalized;
    }

    public float GetWindStrengthMultiplier()
    {
        return windStrengthMultiplier;
    }
    
    public Vector3 GetWindDisplacement(float baseSpeed)
    {
        return currentWindDirection.normalized * (baseSpeed * windStrengthMultiplier);
    }

    public float GetRemainingDuration()
    {
        return remainingDuration;
    }

    private void ApplySandstormToOutlaws(bool sandstormActive)
    {
        RefreshOutlawList();

        for (int i = 0; i < outlawsInScene.Length; i++)
        {
            if (outlawsInScene[i] == null)
            {
                continue;
            }

            outlawsInScene[i].SetSandstormState(sandstormActive);
        }
    }

    private void RefreshOutlawList()
    {
        outlawsInScene = FindObjectsByType<OutlawSystem>(FindObjectsSortMode.None);
    }

    private void UpdateVisuals(bool sandstormActive)
    {
        if (sandstormVfxController != null)
        {
            if (sandstormActive)
            {
                sandstormVfxController.PlayVfx(currentWindDirection);
            }
            else
            {
                sandstormVfxController.StopVfx();
            }
        }

        if (sandstormAudioSource != null)
        {
            if (sandstormActive)
            {
                sandstormAudioSource.Play();
            }
            else
            {
                sandstormAudioSource.Stop();
            }
        }
    }

    private Vector3 GetRandomWindDirection()
    {
        Vector3[] possibleDirections =
        {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right,
            (Vector3.forward + Vector3.right).normalized,
            (Vector3.forward + Vector3.left).normalized,
            (Vector3.back + Vector3.right).normalized,
            (Vector3.back + Vector3.left).normalized
        };

        int randomIndex = Random.Range(0, possibleDirections.Length);
        return possibleDirections[randomIndex];
    }
}
