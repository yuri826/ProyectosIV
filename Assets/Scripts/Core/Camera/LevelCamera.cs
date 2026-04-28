using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    [Header("Follow")]
    [field: SerializeField] public Transform lookAt { get; set; }
    [SerializeField] private float followSmoothSpeed = 8f;

    [Header("Collapse Shake")]
    [SerializeField] private float collapseShakeSpeed = 18f;
    [SerializeField] private float collapseImpactFadeSpeed = 4f;

    [Header("Sandstorm Shake")]
    [SerializeField] private float sandstormShakeStrength = 0.04f;
    [SerializeField] private float sandstormShakeSpeed = 16f;

    private Vector3 lookOffset;
    private Vector3 currentBasePosition;

    private float collapseContinuousShakeAmount = 0f;
    private float collapseImpactShakeAmount = 0f;

    private bool isSandstormShakeActive;

    private float collapseNoiseSeedX;
    private float collapseNoiseSeedY;

    private void Start()
    {
        lookOffset = transform.position - lookAt.position;

        collapseNoiseSeedX = Random.Range(0f, 100f);
        collapseNoiseSeedY = Random.Range(100f, 200f);

        currentBasePosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = lookAt.position + lookOffset;

        currentBasePosition = Vector3.Lerp(
            currentBasePosition,
            targetPosition,
            Time.deltaTime * followSmoothSpeed
        );

        transform.position = currentBasePosition + GetTotalShakeOffset();

        collapseImpactShakeAmount = Mathf.Lerp(
            collapseImpactShakeAmount,
            0f,
            Time.deltaTime * collapseImpactFadeSpeed
        );
    }

    public void SetCollapseShake(float amount)
    {
        collapseContinuousShakeAmount = amount;
    }

    public void AddImpactShake(float amount)
    {
        if (amount > collapseImpactShakeAmount)
        {
            collapseImpactShakeAmount = amount;
        }
    }

    public void StartSandstormShake()
    {
        isSandstormShakeActive = true;
    }

    public void StopSandstormShake()
    {
        isSandstormShakeActive = false;
    }

    private Vector3 GetTotalShakeOffset()
    {
        return GetCollapseShakeOffset() + GetSandstormShakeOffset();
    }

    private Vector3 GetCollapseShakeOffset()
    {
        float totalCollapseShake = collapseContinuousShakeAmount + collapseImpactShakeAmount;

        if (totalCollapseShake <= 0f)
        {
            return Vector3.zero;
        }

        float sampleX = (Mathf.PerlinNoise(collapseNoiseSeedX, Time.time * collapseShakeSpeed) - 0.5f) * 2f;
        float sampleY = (Mathf.PerlinNoise(collapseNoiseSeedY, Time.time * collapseShakeSpeed) - 0.5f) * 2f;

        return new Vector3(sampleX, sampleY, 0f) * totalCollapseShake;
    }

    private Vector3 GetSandstormShakeOffset()
    {
        if (!isSandstormShakeActive)
        {
            return Vector3.zero;
        }

        float xOffset = Mathf.Sin(Time.time * sandstormShakeSpeed) * sandstormShakeStrength;
        float yOffset = (Mathf.PerlinNoise(Time.time * sandstormShakeSpeed, 0f) - 0.5f) * sandstormShakeStrength;

        return new Vector3(xOffset, yOffset, 0f);
    }
}
