using UnityEngine;
using UnityEngine.AI;

public class OutlawSystem : MonoBehaviour
{
    [Header("Current State")]
    [SerializeField] private OutlawState currentState = OutlawState.PickSabotage;

    [Header("References")]
    [SerializeField] private TrainCarZone currentCarZone;
    [SerializeField] private GameObject outlawDynamitePrefab;

    [Header("Sabitage")]
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
    }

    private void Update()
    {
        // Si hay tormenta de arena, este estado tiene prioridad total.
        if (isSandstormActive)
        {
            currentState = OutlawState.Sandstorm;
            navMeshAgent.isStopped = true;
            return;
        }

        if (currentCarZone == null)
        {
            return;
        }

        // Si no estoy ya en combate o en risa, y aparece un jugador cerca,
        // el combate tiene prioridad.
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
            default:
                break;
        }
    }
    
    // MÉTODOS PÚBLICOS

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

    
    // SABOTAJE
    
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

            Vector3 safePosition = currentCarZone.GetRandomPointInCarFarFrom(currentTargetSabotagePoint.transform.position, safeDistanceAfterPlant);
            
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
    
    // PATRULLA

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
        Vector3 randomPoint = currentCarZone.GetRandomPointInCarFarFrom(lastPatrolPoint, minDistanceBetweenPatrolPoints);
        
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(randomPoint);
    }
    
    // COMBATE

    private void UpdateCombat()
    {
        if (outlawCombat == null)
        {
            currentState = OutlawState.PickSabotage;
            return;
        }

        bool shouldStayInCombat = outlawCombat.UpdateCombat(currentCarZone);

        if (!shouldStayInCombat)
        {
            currentState = OutlawState.PickSabotage;
        }
    }

    
    // RISA

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
    
    // AYUDAS

    private void SpawnDynamite()
    {
        if (outlawDynamitePrefab == null)
        {
            return;
        }

        if (currentTargetSabotagePoint == null)
        {
            return;
        }

        GameObject dynamiteObject = Instantiate(outlawDynamitePrefab, currentTargetSabotagePoint.GetDynamitePoint().position, Quaternion.identity);

        currentDynamite = dynamiteObject.GetComponent<OutlawDynamite>();

        if (currentDynamite != null)
        {
            currentDynamite.Init(
                currentTargetSabotagePoint,
                dynamiteFuseTime,
                trainDamagePerExplosion,
                dynamiteExplosionDamage,
                dynamiteExplosionRadius
            );
        }
    }

    private void CancelCurrentSabotage()
    {
        
        currentTargetSabotagePoint.CancelReservation();
        currentTargetSabotagePoint = null;
    }

    private bool HasReachedDestination()
    {
        if (navMeshAgent == null)
        {
            return false;
        }

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
}
