using FMODUnity;
using UnityEngine;

public class SandstormSystem : MonoBehaviour
{
    [Header("Sandstorm State")]
    [SerializeField] private bool isSandstormActive = false;
    [SerializeField] private float remainingDuration = 0f;

    [Header("Wind Settings")]
    [SerializeField] [Range(0f, 2f)] private float baseWindStrengthMultiplier = 0.8f;
    [SerializeField] [Range(0f, 2f)] private float currentWindStrengthMultiplier = 0.8f;
    [SerializeField] private Vector3 currentWindDirection = Vector3.right;

    [Header("Speed Modifiers")]
    [SerializeField] private float lowSpeedStormMultiplier = 0.7f;
    [SerializeField] private float middleSpeedStormMultiplier = 1f;
    [SerializeField] private float highSpeedStormMultiplier = 1.3f;

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
        currentWindStrengthMultiplier = GetModifiedStormStrengthMultiplier();
        
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
        currentWindStrengthMultiplier = GetModifiedStormStrengthMultiplier();

        ApplySandstormToOutlaws(true);
        UpdateVisuals(true);
    }

    public void StopSandstorm()
    {
        isSandstormActive = false;
        remainingDuration = 0f;
        currentWindStrengthMultiplier = baseWindStrengthMultiplier;

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
        return currentWindStrengthMultiplier;
    }

    public Vector3 GetWindDisplacement(float baseSpeed)
    {
        return currentWindDirection.normalized * (baseSpeed * currentWindStrengthMultiplier);
    }

    public float GetRemainingDuration()
    {
        return remainingDuration;
    }

    private float GetModifiedStormStrengthMultiplier()
    {
        float speedModifier = middleSpeedStormMultiplier;

        if (SpeedManager.instance != null)
        {
            switch (SpeedManager.instance.GetCurrentSpeedState())
            {
                case SpeedState.Low:
                    speedModifier = lowSpeedStormMultiplier;
                    break;

                case SpeedState.Middle:
                    speedModifier = middleSpeedStormMultiplier;
                    break;

                case SpeedState.High:
                    speedModifier = highSpeedStormMultiplier;
                    break;
            }
        }

        return baseWindStrengthMultiplier * speedModifier;
    }
    
    public float GetStormModifierForCurrentSpeed()
    {
        if (SpeedManager.instance == null)
        {
            return 1f;
        }

        switch (SpeedManager.instance.GetCurrentSpeedState())
        {
            case SpeedState.Low:
                return lowSpeedStormMultiplier;

            case SpeedState.Middle:
                return middleSpeedStormMultiplier;

            case SpeedState.High:
                return highSpeedStormMultiplier;
        }

        return 1f;
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


        if (sandstormActive)
        {
            sandstormAudioSource.Play();
        }
        else
        {
            sandstormAudioSource.Stop();
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

        return possibleDirections[Random.Range(0, possibleDirections.Length)];
    }
}
