using UnityEngine;
using UnityEngine.AI;

public class OutlawSystem : MonoBehaviour
{
    [Header("Current State")]
    [SerializeField] private OutlawState currentState = OutlawState.PickSabotage;

    [Header("References")]
    [SerializeField] private TrainCarZone currentCarZone;
    [SerializeField] private GameObject outlawDynamitePrefab;
    
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Sabotage")]
    [SerializeField] private float plantDynamiteTime;
    [SerializeField] private float dynamiteFuseTime;
    [SerializeField] private float safeDistanceAfterPlant;
    [SerializeField] private float trainDamagePerExplosion;
    [SerializeField] private float dynamiteExplosionDamage;
    [SerializeField] private float dynamiteExplosionRadius;

    [Header("Patrol")]
    [SerializeField] private int patrolPointsAfterExplosion;
    [SerializeField] private float minDistanceBetweenPatrolPoints;

    [Header("Laugh")]
    [SerializeField] private float laughTime;

    private NavMeshAgent navMeshAgent;
    private OutlawCombat outlawCombat;

    private SabotagePoint currentTargetSabotagePoint;
    private OutlawDynamite currentDynamite;

    private float currentStateTimer;
    private int currentPatrolPointsDone;
    private Vector3 lastPatrolPoint;

    private bool isSandstormActive;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        outlawCombat = GetComponent<OutlawCombat>();

        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    private void Start()
    {
        if (currentCarZone == null)
        {
            currentCarZone = GetComponentInParent<TrainCarZone>();
        }

        if (SandstormSystem.Instance != null && SandstormSystem.Instance.IsSandstormActive())
        {
            SetSandstormState(true);
        }
    }

    private void Update()
    {
        if (isSandstormActive)
        {
            currentState = OutlawState.Sandstorm;
            navMeshAgent.isStopped = true;
            return;
        }

        if (currentState != OutlawState.Combat && currentState != OutlawState.Laugh)
        {
            if (outlawCombat.IsAnyPlayerInAttackRange(currentCarZone))
            {
                CancelCurrentSabotage();
                currentState = OutlawState.Combat;
            }
        }

        switch (currentState)
        {
            case OutlawState.PickSabotage:
                PickSabotagePoint();
                break;

            case OutlawState.MoveToSabotage:
                UpdateMoveToSabotage();
                break;

            case OutlawState.PlantDynamite:
                UpdatePlantDynamite();
                break;

            case OutlawState.MoveToSafePosition:
                UpdateMoveToSafePosition();
                break;

            case OutlawState.WaitExplosion:
                UpdateWaitExplosion();
                break;

            case OutlawState.Patrol:
                UpdatePatrol();
                break;

            case OutlawState.Combat:
                UpdateCombat();
                break;

            case OutlawState.Laugh:
                UpdateLaugh();
                break;

            case OutlawState.Sandstorm:
                break;
        }
        
        UpdateMovementRotation();
    }

    public void SetCurrentCarZone(TrainCarZone newCarZone)
    {
        currentCarZone = newCarZone;
    }

    public void SetSandstormState(bool sandstormActive)
    {
        isSandstormActive = sandstormActive;

        if (isSandstormActive)
        {
            CancelCurrentSabotage();
            outlawCombat.ClearCombatData();
            navMeshAgent.isStopped = true;
            currentState = OutlawState.Sandstorm;
        }
        else
        {
            currentState = OutlawState.PickSabotage;
        }
    }

    public void OnPlayerLeftCar(PlayerMovement player, Vector3 lastPosition)
    {
        outlawCombat.OnPlayerLeftCar(player, lastPosition);
    }

    public void OnPlayerFell(PlayerMovement player)
    {
        outlawCombat.OnPlayerFell(player);
        currentStateTimer = laughTime;
        navMeshAgent.isStopped = true;
        currentState = OutlawState.Laugh;
    }

    public void OnDead()
    {
        CancelCurrentSabotage();
    }

    private void PickSabotagePoint()
    {
        currentTargetSabotagePoint = currentCarZone.GetRandomFreeSabotagePoint();

        if (currentTargetSabotagePoint == null)
        {
            StartPatrol();
            return;
        }

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(currentTargetSabotagePoint.transform.position);

        currentState = OutlawState.MoveToSabotage;
    }

    private void UpdateMoveToSabotage()
    {
        if (outlawCombat.IsAnyPlayerInAttackRange(currentCarZone))
        {
            CancelCurrentSabotage();
            currentState = OutlawState.Combat;
            return;
        }

        if (currentTargetSabotagePoint == null)
        {
            currentState = OutlawState.PickSabotage;
            return;
        }

        if (HasReachedDestination())
        {
            navMeshAgent.isStopped = true;

            currentStateTimer = plantDynamiteTime;
            currentState = OutlawState.PlantDynamite;
        }
    }

    private void UpdatePlantDynamite()
    {
        if (outlawCombat.IsAnyPlayerInAttackRange(currentCarZone))
        {
            CancelCurrentSabotage();
            currentState = OutlawState.Combat;
            return;
        }

        if (currentTargetSabotagePoint == null)
        {
            currentState = OutlawState.PickSabotage;
            return;
        }

        currentStateTimer -= Time.deltaTime;

        if (currentStateTimer <= 0f)
        {
            SpawnDynamite();

            Vector3 safePosition = currentCarZone.GetRandomPointInCarFarFrom(
                currentTargetSabotagePoint.transform.position,
                safeDistanceAfterPlant
            );

            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(safePosition);

            currentTargetSabotagePoint = null;
            currentState = OutlawState.MoveToSafePosition;
        }
    }

    private void UpdateMoveToSafePosition()
    {
        if (currentDynamite == null)
        {
            StartPatrol();
            return;
        }

        if (HasReachedDestination())
        {
            navMeshAgent.isStopped = true;
            currentState = OutlawState.WaitExplosion;
        }
    }

    private void UpdateWaitExplosion()
    {
        if (currentDynamite == null)
        {
            StartPatrol();
        }
    }

    private void StartPatrol()
    {
        currentPatrolPointsDone = 0;
        lastPatrolPoint = transform.position;

        PickNextPatrolPoint();
        currentState = OutlawState.Patrol;
    }

    private void UpdatePatrol()
    {
        if (outlawCombat.IsAnyPlayerInAttackRange(currentCarZone))
        {
            currentState = OutlawState.Combat;
            return;
        }

        if (!HasReachedDestination())
        {
            return;
        }

        currentPatrolPointsDone++;

        if (currentPatrolPointsDone >= patrolPointsAfterExplosion)
        {
            currentState = OutlawState.PickSabotage;
            return;
        }

        lastPatrolPoint = transform.position;
        PickNextPatrolPoint();
    }

    private void PickNextPatrolPoint()
    {
        Vector3 randomPoint = currentCarZone.GetRandomPointInCarFarFrom(
            lastPatrolPoint,
            minDistanceBetweenPatrolPoints
        );

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(randomPoint);
    }

    private void UpdateCombat()
    {
        bool shouldStayInCombat = outlawCombat.UpdateCombat(currentCarZone);

        if (!shouldStayInCombat)
        {
            currentState = OutlawState.PickSabotage;
        }
    }

    private void UpdateLaugh()
    {
        currentStateTimer -= Time.deltaTime;

        if (currentStateTimer <= 0f)
        {
            if (outlawCombat.IsAnyPlayerInAttackRange(currentCarZone))
            {
                currentState = OutlawState.Combat;
            }
            else
            {
                currentState = OutlawState.PickSabotage;
            }
        }
    }

    private void SpawnDynamite()
    {
        if (currentTargetSabotagePoint == null)
        {
            return;
        }

        GameObject dynamiteObject = Instantiate(
            outlawDynamitePrefab,
            currentTargetSabotagePoint.GetDynamitePoint().position,
            Quaternion.identity
        );

        currentDynamite = dynamiteObject.GetComponent<OutlawDynamite>();

        currentDynamite.Init(
            currentTargetSabotagePoint,
            dynamiteFuseTime,
            trainDamagePerExplosion,
            dynamiteExplosionDamage,
            dynamiteExplosionRadius
        );
    }

    private void CancelCurrentSabotage()
    {
        currentTargetSabotagePoint?.CancelReservation();
        currentTargetSabotagePoint = null;
    }

    private bool HasReachedDestination()
    {
        if (navMeshAgent.pathPending)
        {
            return false;
        }

        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance + 0.15f)
        {
            return false;
        }

        return true;
    }
    
    private void UpdateMovementRotation()
    {
        if (currentState != OutlawState.MoveToSabotage &&
            currentState != OutlawState.MoveToSafePosition &&
            currentState != OutlawState.Patrol)
        {
            return;
        }

        Vector3 lookDirection = navMeshAgent.desiredVelocity;

        if (lookDirection.sqrMagnitude <= 0.0001f)
        {
            lookDirection = navMeshAgent.velocity;
        }

        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
