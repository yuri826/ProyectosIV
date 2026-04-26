using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SpeedManager : GamemodeSubsystem
{
    [Header("Speed Limits")]
    [SerializeField] private float minSpeed = 60f;
    [SerializeField] private float maxSpeed = 180f;
    [SerializeField] private float initSpeed = 100f;

    [Header("Speed Thresholds")]
    [SerializeField] private float lowSpeedThreshold = 80f;
    [SerializeField] private float highSpeedThreshold = 120f;

    [Header("Startup")]
    [SerializeField] private float startupDuration = 3f;
    [SerializeField] private AnimationCurve startupCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Coal Boost")]
    [SerializeField] private float coalSpeedBoostDuration = 1f;
    [SerializeField] private AnimationCurve coalBoostCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Decay")]
    [Tooltip("Cantidad total que pierde en cada intervalo completo.")]
    [SerializeField] private float speedDecayAmount = 10f;
    [Tooltip("Tiempo en segundos en el que pierde speedDecayAmount.")]
    [SerializeField] private float speedDecayInterval = 5f;
    
    private float currentBrakeMultiplier = 1f;

    [Header("HUD")]
	[SerializeField] private RectTransform speedNeedle;
	[SerializeField] private TextMeshProUGUI speedText;
	[SerializeField] private float minNeedleAngle = 90f;
	[SerializeField] private float maxNeedleAngle = -90f;

    [Header("CoalBoost")]
    private float targetCoalSpeed;
    private bool isCoalBoosting;
    private float coalBoostTimer = 0;

    [Header("Debug / Read Only")]
    [SerializeField] private float currentSpeed = 0f;
    [SerializeField] private SpeedState currentSpeedState = SpeedState.Low;

    private bool startupTriggered;
    private bool isStartingUp;
    private float startupTimer;

    public float CurrentSpeed => currentSpeed;
    public SpeedState CurrentSpeedState => currentSpeedState;

    public override void OnStart()
    {
        currentSpeed = 0f;
        UpdateSpeedState();
        UpdateHUD();
    }

    public override void OnUpdate()
    {
        UpdateStartup();
        UpdateDecaySmooth();

        CoalBoost();

        ApplyRuntimeClamp();
        UpdateSpeedState();
        UpdateHUD();
    }

    public void StartStartup()
    {
        if (startupTriggered) return;

        startupTriggered = true;
        isStartingUp = true;
        startupTimer = 0f;
        currentSpeed = 0f;
    }

    private void UpdateStartup()
    {
        if (!isStartingUp) return;

        startupTimer += Time.deltaTime;

        float normalizedTime = Mathf.Clamp01(startupTimer / startupDuration);
        float curveValue = startupCurve.Evaluate(normalizedTime);

        currentSpeed = Mathf.LerpUnclamped(0f, initSpeed, curveValue);

        if (normalizedTime >= 1f)
        {
            currentSpeed = initSpeed;
            isStartingUp = false;
        }
    }

    private void UpdateDecaySmooth()
    {
        if ((!startupTriggered || isStartingUp) || (speedDecayInterval <= 0f)) return;

        float decayPerSecond = speedDecayAmount / speedDecayInterval;
        currentSpeed -= decayPerSecond * currentBrakeMultiplier * Time.deltaTime;
    }
    
    public void SetBrakeMultiplier(float brakeMultiplier)
    {
        currentBrakeMultiplier = brakeMultiplier;
    }

    public void ResetBrakeMultiplier()
    {
        currentBrakeMultiplier = 1f;
    }

    private void ApplyRuntimeClamp()
    {
        if (!startupTriggered || isStartingUp)
        {
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
            return;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
    }

    public void AddSpeed(float amount)
    {
        isCoalBoosting = true;
        coalBoostTimer = 0f;
        targetCoalSpeed = Mathf.Clamp(currentSpeed + amount, 0f, maxSpeed);
    }

    private void CoalBoost()
    {
        if (!isCoalBoosting)
        {
            return;
        }
        
        coalBoostTimer += Time.deltaTime;

        float normalizedTime = Mathf.Clamp01(coalBoostTimer / coalSpeedBoostDuration);
        float curveValue = coalBoostCurve.Evaluate(normalizedTime);

        currentSpeed = Mathf.LerpUnclamped(currentSpeed, targetCoalSpeed, curveValue);

        if (normalizedTime >= 1f)
        {
            currentSpeed = targetCoalSpeed;
            isCoalBoosting = false;
        }
    }

    private void UpdateSpeedState()
    {
        if (currentSpeed < lowSpeedThreshold)
        {
            currentSpeedState = SpeedState.Low;
            return;
        }
        
        if (currentSpeed < highSpeedThreshold)
        {
            currentSpeedState = SpeedState.Middle;
            return;
        }

        currentSpeedState = SpeedState.High;
    }

    private void UpdateHUD()
    {
        float normalizedSpeed = Mathf.InverseLerp(0f, maxSpeed, currentSpeed);
        float needleAngle = Mathf.Lerp(minNeedleAngle, maxNeedleAngle, normalizedSpeed);

        speedNeedle.localRotation = Quaternion.Euler(0f, 0f, needleAngle);
        speedText.text = $"{Mathf.RoundToInt(currentSpeed)} km/h";
    }

    public void SetCurrentSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
        ApplyRuntimeClamp();
        UpdateSpeedState();
        UpdateHUD();
    }
}
