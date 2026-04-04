// using System.Collections;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
//
// public class SpeedManager : MonoBehaviour
// {
//     public static SpeedManager instance;
//
//     [Header("Speed Limits")]
//     [SerializeField] private float minSpeed = 60f;
//     [SerializeField] private float maxSpeed = 180f;
//     [SerializeField] private float initSpeed = 100f;
//
//     [Header("Speed Thresholds")]
//     [SerializeField] private float lowSpeedThreshold = 80f;
//     [SerializeField] private float highSpeedThreshold = 120f;
//
//     [Header("Startup")]
//     [SerializeField] private float startupDuration = 3f;
//     [SerializeField] private AnimationCurve startupCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
//
//     [Header("Coal Boost")]
//     [SerializeField] private float coalSpeedBoostDuration = 1f;
//     [SerializeField] private AnimationCurve coalBoostCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
//
//     [Header("Decay")]
//     [Tooltip("Cantidad total que pierde en cada intervalo completo.")]
//     [SerializeField] private float speedDecayAmount = 10f;
//     [Tooltip("Tiempo en segundos en el que pierde speedDecayAmount.")]
//     [SerializeField] private float speedDecayInterval = 5f;
//
//     [Header("HUD")]
//     [SerializeField] private Image speedBarFill;
//     [SerializeField] private TextMeshProUGUI speedText;
//
//     [Header("Debug / Read Only")]
//     [SerializeField] private float currentSpeed = 0f;
//     [SerializeField] private SpeedState currentSpeedState = SpeedState.Low;
//
//     private bool startupTriggered = false;
//     private bool isStartingUp = false;
//     private float startupTimer = 0f;
//     private Coroutine coalBoostRoutine;
//
//     public float CurrentSpeed => currentSpeed;
//     public SpeedState CurrentSpeedState => currentSpeedState;
//
//     private void Awake()
//     {
//         instance = this;
//     }
//
//     private void Start()
//     {
//         currentSpeed = 0f;
//         UpdateSpeedState();
//         UpdateHUD();
//     }
//
//     private void Update()
//     {
//         UpdateStartup();
//         UpdateDecaySmooth();
//
//         ApplyRuntimeClamp();
//         UpdateSpeedState();
//         UpdateHUD();
//     }
//
//     public void StartStartup()
//     {
//         if (startupTriggered)
//         {
//             return;
//         }
//
//         startupTriggered = true;
//         isStartingUp = true;
//         startupTimer = 0f;
//         currentSpeed = 0f;
//     }
//
//     private void UpdateStartup()
//     {
//         if (!isStartingUp)
//         {
//             return;
//         }
//
//         startupTimer += Time.deltaTime;
//
//         float normalizedTime = Mathf.Clamp01(startupTimer / startupDuration);
//         float curveValue = startupCurve.Evaluate(normalizedTime);
//
//         currentSpeed = Mathf.LerpUnclamped(0f, initSpeed, curveValue);
//
//         if (normalizedTime >= 1f)
//         {
//             currentSpeed = initSpeed;
//             isStartingUp = false;
//         }
//     }
//
//     private void UpdateDecaySmooth()
//     {
//         if (!startupTriggered || isStartingUp)
//         {
//             return;
//         }
//
//         if (speedDecayInterval <= 0f)
//         {
//             return;
//         }
//
//         float decayPerSecond = speedDecayAmount / speedDecayInterval;
//         currentSpeed -= decayPerSecond * Time.deltaTime;
//     }
//
//     private void ApplyRuntimeClamp()
//     {
//         if (!startupTriggered)
//         {
//             currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
//             return;
//         }
//
//         if (isStartingUp)
//         {
//             currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
//             return;
//         }
//
//         currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
//     }
//
//     public void AddSpeed(float amount)
//     {
//         if (amount <= 0f)
//         {
//             return;
//         }
//
//         if (coalBoostRoutine != null)
//         {
//             StopCoroutine(coalBoostRoutine);
//         }
//
//         coalBoostRoutine = StartCoroutine(CoalBoostRoutine(amount));
//     }
//
//     private IEnumerator CoalBoostRoutine(float amount)
//     {
//         float startSpeed = currentSpeed;
//         float targetSpeed = Mathf.Clamp(currentSpeed + amount, 0f, maxSpeed);
//
//         float timer = 0f;
//
//         while (timer < coalSpeedBoostDuration)
//         {
//             timer += Time.deltaTime;
//
//             float normalizedTime = Mathf.Clamp01(timer / coalSpeedBoostDuration);
//             float curveValue = coalBoostCurve.Evaluate(normalizedTime);
//
//             currentSpeed = Mathf.LerpUnclamped(startSpeed, targetSpeed, curveValue);
//
//             yield return null;
//         }
//
//         currentSpeed = targetSpeed;
//         coalBoostRoutine = null;
//     }
//
//     private void UpdateSpeedState()
//     {
//         if (currentSpeed < lowSpeedThreshold)
//         {
//             currentSpeedState = SpeedState.Low;
//             return;
//         }
//
//         if (currentSpeed < highSpeedThreshold)
//         {
//             currentSpeedState = SpeedState.Middle;
//             return;
//         }
//
//         currentSpeedState = SpeedState.High;
//     }
//
//     private void UpdateHUD()
//     {
//         if (speedBarFill != null)
//         {
//             float normalizedSpeed = Mathf.InverseLerp(0f, maxSpeed, currentSpeed);
//             speedBarFill.fillAmount = normalizedSpeed;
//         }
//
//         if (speedText != null)
//         {
//             speedText.text = $"{Mathf.RoundToInt(currentSpeed)} km/h";
//         }
//     }
//
//     public void SetCurrentSpeed(float newSpeed)
//     {
//         currentSpeed = newSpeed;
//         ApplyRuntimeClamp();
//         UpdateSpeedState();
//         UpdateHUD();
//     }
//
//     public float GetCurrentSpeed()
//     {
//         return currentSpeed;
//     }
//
//     public SpeedState GetCurrentSpeedState()
//     {
//         return currentSpeedState;
//     }
//
//     public bool IsStartingUp()
//     {
//         return isStartingUp;
//     }
// }
