using UnityEngine;
using UnityEngine.AI;

public class OutlawSystem : MonoBehaviour
{
    [Header("Estado actual")]
    [SerializeField] private string currentState = "pickSabotage";

    [Header("Referencias")]
    [SerializeField] private TrainCarZone currentCarZone;
    [SerializeField] private GameObject outlawDynamitePrefab;

    [Header("Ajustes de sabotaje")]
    [SerializeField] private float plantDynamiteTime;
    [SerializeField] private float dynamiteFuseTime;
    [SerializeField] private float safeDistanceAfterPlant;
    [SerializeField] private float trainDamagePerExplosion;
    [SerializeField] private float dynamiteExplosionDamage;
    [SerializeField] private float dynamiteExplosionRadius;

    [Header("Ajustes de patrulla")]
    [SerializeField] private int patrolPointsAfterExplosion;
    [SerializeField] private float minDistanceBetweenPatrolPoints;

    [Header("Reacciones")]
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
        // 1. Si hay tormenta de arena, eso tiene prioridad total
        if (isSandstormActive)
        {
            currentState = "sandstorm";
            navMeshAgent.isStopped = true;
            return;
        }

        // 2. Si no estoy en risa y detecto un jugador en rango, el combate tiene prioridad
        if (currentState != "laugh" && currentState != "combat")
        {
            if (outlawCombat.IsAnyPlayerInAttackRange(currentCarZone))
            {
                CancelCurrentSabotage();
                currentState = "combat";
            }
        }

        // 3. Ejecutamos la lógica del estado actual
        switch (currentState)
        {
            case "pickSabotage":
                PickSabotagePoint();
                break;

            case "moveToSabotage":
                UpdateMoveToSabotage();
                break;

            case "plantDynamite":
                UpdatePlantDynamite();
                break;

            case "moveToSafePosition":
                UpdateMoveToSafePosition();
                break;

            case "waitExplosion":
                UpdateWaitExplosion();
                break;

            case "patrol":
                UpdatePatrol();
                break;

            case "combat":
                UpdateCombat();
                break;

            case "laugh":
                UpdateLaugh();
                break;

            case "sandstorm":
                // No hace nada. Lo bloquea al principio del Update.
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
            currentState = "sandstorm";
        }
        else
        {
            currentState = "pickSabotage";
        }
    }

    public void OnPlayerLeftCar(PlayerMovement player, Vector3 lastPosition)
    {
        outlawCombat.OnPlayerLeftCar(player, lastPosition);
    }
    
    public void OnPlayerFell(PlayerMovement player)
    {
        
        outlawCombat.OnPlayerFell(player);

        currentState = "laugh";
        currentStateTimer = laughTime;
        navMeshAgent.isStopped = true;
    }
    
    public void OnDead()
    {
        CancelCurrentSabotage();
    }
    
    // ESTADOS DE SABOTAJE

    private void PickSabotagePoint()
    {
        currentTargetSabotagePoint = currentCarZone.GetRandomFreeSabotagePoint();

        // Si no hay puntos válidos, patrullamos
        if (currentTargetSabotagePoint == null)
        {
            StartPatrol();
            return;
        }
        
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(currentTargetSabotagePoint.transform.position);

        currentState = "moveToSabotage";
    }

    private void UpdateMoveToSabotage()
    {
        // Si aparece un jugador, cancelamos sabotaje y vamos a combate
        if (outlawCombat.IsAnyPlayerInAttackRange(currentCarZone))
        {
            CancelCurrentSabotage();
            currentState = "combat";
            return;
        }

        if (currentTargetSabotagePoint == null)
        {
            currentState = "pickSabotage";
            return;
        }

        if (HasReachedDestination())
        {
            navMeshAgent.isStopped = true;
            currentStateTimer = plantDynamiteTime;
            currentState = "plantDynamite";
        }
    }
    
    private void UpdatePlantDynamite()
    {
        // Si aparece un jugador, cancelamos antes de terminar de plantar
        if (outlawCombat.IsAnyPlayerInAttackRange(currentCarZone))
        {
            CancelCurrentSabotage();
            currentState = "combat";
            return;
        }

        if (currentTargetSabotagePoint == null)
        {
            currentState = "pickSabotage";
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
            currentState = "moveToSafePosition";
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
            currentState = "waitExplosion";
        }
    }

    private void UpdateWaitExplosion()
    {
        // Espera quieto hasta que la dinamita desaparece al explotar
        if (currentDynamite == null)
        {
            StartPatrol();
        }
    }
    
    
    // ESTADO DE PATRULLA

    private void StartPatrol()
    {
        currentPatrolPointsDone = 0;
        lastPatrolPoint = transform.position;

        PickNextPatrolPoint();
        currentState = "patrol";
    }


    private void UpdatePatrol()
    {
        // Si aparece un jugador, entramos en combate
        if (outlawCombat.IsAnyPlayerInAttackRange(currentCarZone))
        {
            currentState = "combat";
            return;
        }

        if (!HasReachedDestination())
        {
            return;
        }

        currentPatrolPointsDone++;

        if (currentPatrolPointsDone >= patrolPointsAfterExplosion)
        {
            currentState = "pickSabotage";
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


    // ESTADO DE COMBATE

    private void UpdateCombat()
    {
        if (outlawCombat == null)
        {
            currentState = "pickSabotage";
            return;
        }

        bool shouldStayInCombat = outlawCombat.UpdateCombat(currentCarZone);

        if (!shouldStayInCombat)
        {
            currentState = "pickSabotage";
        }
    }
    
    
    // ESTADO DE RISA

    private void UpdateLaugh()
    {
        currentStateTimer -= Time.deltaTime;

        if (currentStateTimer <= 0f)
        {
            if (outlawCombat.IsAnyPlayerInAttackRange(currentCarZone))
            {
                currentState = "combat";
            }
            else
            {
                currentState = "pickSabotage";
            }
        }
    }


    // AYUDAS

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
        if (currentTargetSabotagePoint != null)
        {
            currentTargetSabotagePoint.CancelReservation();
            currentTargetSabotagePoint = null;
        }
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
}
