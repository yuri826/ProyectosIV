using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    [Header("Follow")]
    [field: SerializeField] public Transform lookAt {get; set;}
    [SerializeField] private float followSmoothSpeed = 8f;

    [Header("Shake")]
    [SerializeField] private float shakeSpeed = 18f;
    [SerializeField] private float impactShakeFadeSpeed = 4f;

    private Vector3 lookOffset;
    private Vector3 currentBasePosition;

    private float collapseShakeAmount = 0f;
    private float currentImpactShake = 0f;

    private float noiseSeedX;
    private float noiseSeedY;

    private void Start()
    {
        lookOffset = transform.position - lookAt.position;

        noiseSeedX = Random.Range(0f, 100f);
        noiseSeedY = Random.Range(100f, 200f);

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

        transform.position = currentBasePosition + GetShakeOffset();

        currentImpactShake = Mathf.Lerp(currentImpactShake, 0f, Time.deltaTime * impactShakeFadeSpeed);
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
