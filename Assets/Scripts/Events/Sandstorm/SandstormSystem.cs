using FMODUnity;
using UnityEngine;

public class SandstormSystem : MonoBehaviour
{
    [Header("Sandstorm State")]
    [field:SerializeField] public bool isSandstormActive { get; set; } = false;
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
    [SerializeField] private StudioEventEmitter sandstormAudioSource;

    [Header("Outlaws")]
    [SerializeField] private OutlawSystem[] outlawsInScene;

    public static SandstormSystem Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        //if (!isSandstormActive) return; //Retornar de un update chungo porque sale y no vuelve a entrar

        if (isSandstormActive)
        {
            currentWindStrengthMultiplier = GetModifiedStormStrengthMultiplier();
            remainingDuration -= Time.deltaTime;

            if (remainingDuration <= 0f) StopSandstorm();
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

    private void StopSandstorm()
    {
        isSandstormActive = false;
        remainingDuration = 0f;
        currentWindStrengthMultiplier = baseWindStrengthMultiplier;

        ApplySandstormToOutlaws(false);
        UpdateVisuals(false);
    }
    
    public Vector3 GetWindDisplacement(float baseSpeed)
    {
        return currentWindDirection.normalized * (baseSpeed * currentWindStrengthMultiplier);
    }

    //Inutilizado
    // public float GetRemainingDuration()
    // {
    //     return remainingDuration;
    // }

    private float GetModifiedStormStrengthMultiplier()
    {
        float speedModifier = middleSpeedStormMultiplier;

        switch (TrainGameMode.instance.GetSpeedManager().CurrentSpeedState)
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

        return baseWindStrengthMultiplier * speedModifier;
    }
    
    //Inutilizado
    // public float GetStormModifierForCurrentSpeed()
    // {
    //     switch (TrainGameMode.instance.GetSpeedManager().GetCurrentSpeedState())
    //     {
    //         case SpeedState.Low:
    //             return lowSpeedStormMultiplier;
    //
    //         case SpeedState.Middle:
    //             return middleSpeedStormMultiplier;
    //
    //         case SpeedState.High:
    //             return highSpeedStormMultiplier;
    //     }
    //
    //     return 1f;
    // }

    private void ApplySandstormToOutlaws(bool sandstormActive)
    {
        RefreshOutlawList();

        foreach (var outlaw in outlawsInScene)
        {
            outlaw?.SetSandstormState(sandstormActive);
        }
    }

    private void RefreshOutlawList()
    {
        outlawsInScene = FindObjectsByType<OutlawSystem>(FindObjectsSortMode.None);
    }

    private void UpdateVisuals(bool sandstormActive)
    {
        if (sandstormActive)
            sandstormVfxController?.PlayVfx(currentWindDirection);
        else
            sandstormVfxController?.StopVfx();


        if (sandstormActive)
            sandstormAudioSource?.Play();
        else
            sandstormAudioSource?.Stop();
    }
    
    //Inutilizado
    // private Vector3 GetRandomWindDirection()
    // {
    //     Vector3[] possibleDirections =
    //     {
    //         Vector3.forward,
    //         Vector3.back,
    //         Vector3.left,
    //         Vector3.right,
    //         (Vector3.forward + Vector3.right).normalized,
    //         (Vector3.forward + Vector3.left).normalized,
    //         (Vector3.back + Vector3.right).normalized,
    //         (Vector3.back + Vector3.left).normalized
    //     };
    //
    //     return possibleDirections[Random.Range(0, possibleDirections.Length)];
    // }
}
