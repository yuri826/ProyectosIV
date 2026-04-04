using UnityEngine;
using UnityEngine.AI;

public class SandstormMovingObject : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float arriveDistance = 0.1f;
    [SerializeField] private float stopTimeAtPoint = 0.75f;

    [Header("Movement")]
    [SerializeField] private float maxMoveSpeed = 2f;
    [SerializeField] private float acceleration = 4f;
    [SerializeField] private float deceleration = 5f;

    [Header("Player Push")]
    [SerializeField] private float playerDragStrengthMultiplier = 1f;

    [Header("Optional Visual Rotation")]
    [SerializeField] private bool useVisualRotation = false;
    [SerializeField] private Transform visualToRotate;
    [SerializeField] private Vector3 rotationAxis = Vector3.right;
    [SerializeField] private float maxRotationSpeed = 360f;
    [SerializeField] private float rotationAcceleration = 720f;
    [SerializeField] private float rotationDeceleration = 900f;

    private Transform currentTargetPoint;

    private bool isWaiting = false;
    private float waitTimer = 0f;

    private bool finishCurrentPathAfterStorm = false;
    private bool wasSandstormActiveLastFrame = false;

    private float currentMoveSpeed = 0f;
    private float currentRotationSpeed = 0f;

    private Vector3 previousPosition;
    private Vector3 currentFrameMovement;

    private void Start()
    {
        currentTargetPoint = pointB;
        previousPosition = transform.position;
        wasSandstormActiveLastFrame = false;
    }

    private void Update()
    {
        currentFrameMovement = Vector3.zero;

        if (pointA == null || pointB == null)
        {
            return;
        }

        bool sandstormActive = SandstormSystem.Instance != null && SandstormSystem.Instance.IsSandstormActive();

        // Solo queremos terminar el trayecto actual si la tormenta acaba
        // mientras el objeto estaba ya en movimiento.
        if (!sandstormActive && wasSandstormActiveLastFrame && !isWaiting && currentTargetPoint != null)
        {
            finishCurrentPathAfterStorm = true;
        }

        // Si no hay tormenta y no hay trayecto pendiente por terminar, quedarse quieto.
        if (!sandstormActive && !finishCurrentPathAfterStorm)
        {
            DecelerateToStop();
            UpdateVisualRotation(false);

            previousPosition = transform.position;
            wasSandstormActiveLastFrame = sandstormActive;
            return;
        }

        if (isWaiting)
        {
            // Si la tormenta termina mientras espera en A o B, se queda parado ahí.
            if (!sandstormActive)
            {
                isWaiting = false;
                finishCurrentPathAfterStorm = false;

                previousPosition = transform.position;
                wasSandstormActiveLastFrame = sandstormActive;
                return;
            }

            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                SwapTargetPoint();
            }

            UpdateVisualRotation(false);

            previousPosition = transform.position;
            wasSandstormActiveLastFrame = sandstormActive;
            return;
        }

        MoveTowardsCurrentTarget();
        UpdateVisualRotation(currentMoveSpeed > 0.01f);

        previousPosition = transform.position;
        wasSandstormActiveLastFrame = sandstormActive;
    }

    private void MoveTowardsCurrentTarget()
    {
        Vector3 toTarget = currentTargetPoint.position - transform.position;
        toTarget.y = 0f;

        float distanceToTarget = toTarget.magnitude;

        if (distanceToTarget <= arriveDistance)
        {
            transform.position = new Vector3(
                currentTargetPoint.position.x,
                transform.position.y,
                currentTargetPoint.position.z
            );

            currentFrameMovement = transform.position - previousPosition;
            currentFrameMovement.y = 0f;
            currentMoveSpeed = 0f;

            // Si la tormenta había terminado y solo estaba acabando este trayecto,
            // al llegar aquí se para definitivamente.
            if (finishCurrentPathAfterStorm)
            {
                finishCurrentPathAfterStorm = false;
                return;
            }

            isWaiting = true;
            waitTimer = stopTimeAtPoint;
            return;
        }

        Vector3 moveDirection = toTarget.normalized;
        float brakingDistance = GetBrakingDistance();

        if (distanceToTarget <= brakingDistance)
        {
            currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, 0f, deceleration * Time.deltaTime);
        }
        else
        {
            currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, maxMoveSpeed, acceleration * Time.deltaTime);
        }

        Vector3 displacement = moveDirection * (currentMoveSpeed * Time.deltaTime);

        if (displacement.magnitude > distanceToTarget)
        {
            displacement = moveDirection * distanceToTarget;
        }

        transform.position += displacement;

        currentFrameMovement = transform.position - previousPosition;
        currentFrameMovement.y = 0f;
    }

    private float GetBrakingDistance()
    {
        if (deceleration <= 0.01f)
        {
            return 0f;
        }

        return (currentMoveSpeed * currentMoveSpeed) / (2f * deceleration);
    }

    private void DecelerateToStop()
    {
        currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, 0f, deceleration * Time.deltaTime);
        currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, 0f, rotationDeceleration * Time.deltaTime);
    }

    private void SwapTargetPoint()
    {
        if (currentTargetPoint == pointA)
        {
            currentTargetPoint = pointB;
        }
        else
        {
            currentTargetPoint = pointA;
        }
    }

    private void UpdateVisualRotation(bool shouldRotate)
    {
        if (!useVisualRotation || visualToRotate == null)
        {
            return;
        }

        if (shouldRotate)
        {
            currentRotationSpeed = Mathf.MoveTowards(
                currentRotationSpeed,
                maxRotationSpeed,
                rotationAcceleration * Time.deltaTime
            );
        }
        else
        {
            currentRotationSpeed = Mathf.MoveTowards(
                currentRotationSpeed,
                0f,
                rotationDeceleration * Time.deltaTime
            );
        }

        visualToRotate.Rotate(rotationAxis.normalized, currentRotationSpeed * Time.deltaTime, Space.Self);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (currentFrameMovement == Vector3.zero)
        {
            return;
        }

        CharacterController characterController = collision.collider.GetComponent<CharacterController>();

        if (characterController != null)
        {
            Vector3 flatMovement = currentFrameMovement * playerDragStrengthMultiplier;
            flatMovement.y = 0f;

            characterController.Move(flatMovement);
        }
    }

    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pointA.position, 0.1f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pointB.position, 0.1f);
        }
    }
}
