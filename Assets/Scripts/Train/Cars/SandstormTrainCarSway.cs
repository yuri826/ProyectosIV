using UnityEngine;

public class SandstormTrainCarSway : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform visualRoot;

    [Header("Sway Settings")]
    [SerializeField] private Vector3 swayAxis = Vector3.right;
    [SerializeField] private float swayAngle = 4f;
    [SerializeField] private float swaySpeed = 1.5f;

    [Header("Desync")]
    [SerializeField] private bool randomizeInitialPhase = true;
    [SerializeField] private float initialPhaseOffset = 0f;

    [Header("Return Settings")]
    [SerializeField] private float returnSpeed = 4f;

    private Quaternion baseLocalRotation;
    private float swayTimer = 0f;

    private void Start()
    {
        visualRoot ??= transform;
        baseLocalRotation = visualRoot.localRotation;

        swayTimer = randomizeInitialPhase ? Random.Range(0f, Mathf.PI * 2f) : initialPhaseOffset;
    }

    private void Update()
    {
        if (visualRoot is null) return;

        bool sandstormActive = SandstormSystem.Instance is not null && SandstormSystem.Instance.isSandstormActive;

        if (sandstormActive)
        {
            swayTimer += Time.deltaTime * swaySpeed;

            float currentAngle = Mathf.Sin(swayTimer) * swayAngle;
            Quaternion swayRotation = Quaternion.AngleAxis(currentAngle, swayAxis.normalized);

            visualRoot.localRotation = baseLocalRotation * swayRotation;
        }
        else
        {
            visualRoot.localRotation = Quaternion.Lerp(
                visualRoot.localRotation,
                baseLocalRotation,
                Time.deltaTime * returnSpeed
            );
        }
    }
}
