using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class OutlawCombat : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject outlawBulletPrefab;

    [Header("Ajustes de combate")]
    [SerializeField] private float detectionRange;
    [SerializeField] private float attackDistance;
    [SerializeField] private float tooCloseDistance;
    [SerializeField] private float shootCooldown;
    [SerializeField] private float aimLastPositionTime;

    [Header("Capas que bloquean disparos")]
    [SerializeField] private LayerMask obstacleMask;

    private NavMeshAgent navMeshAgent;
    private PlayerMovement currentTargetPlayer;
    private PlayerMovement lastPlayerShot;
    private Vector3 lastKnownPlayerPosition;
    private float currentShootCooldown;
    private float currentAimLastPositionTimer;
    private bool isRepositioning;
    private Vector3 currentRepositionTarget;
    
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    private void Update()
    {
        if (currentShootCooldown > 0f)
        {
            currentShootCooldown -= Time.deltaTime;
        }
        
        if (currentAimLastPositionTimer > 0f)
        {
            currentAimLastPositionTimer -= Time.deltaTime;
        }
    }
    
    // MÉTODOS PÚBLICOS

    public bool IsAnyPlayerInAttackRange(TrainCarZone currentCar)
    {
        List<PlayerMovement> playersInRange = currentCar.GetPlayersInRange(transform.position, detectionRange);
        return playersInRange.Count > 0;
    }

    public bool UpdateCombat(TrainCarZone currentCar)
    {
        // 1. Si no tengo objetivo, intento conseguir uno
        if (currentTargetPlayer == null)
        {
            currentTargetPlayer = GetNextTargetPlayer(currentCar, lastPlayerShot);
        }

        // 2. Si sigo sin objetivo, miro un rato hacia la última posición conocida
        if (currentTargetPlayer == null)
        {
            if (currentAimLastPositionTimer > 0f)
            {
                MoveToPosition(lastKnownPlayerPosition);
                LookAtPosition(lastKnownPlayerPosition);
                return true;
            }

            StopMoving();
            return false;
        }

        // 3. Si el jugador ya no está en el vagón, lo pierdo
        if (!IsPlayerStillInsideCar(currentCar, currentTargetPlayer))
        {
            SaveLastKnownPlayerPosition(currentTargetPlayer.transform.position);
            currentTargetPlayer = null;
            CancelReposition();
            return true;
        }

        // 4. Comprobamos la distancia al jugador
        float distanceToTarget = Vector3.Distance(transform.position, currentTargetPlayer.transform.position);

        // Si se ha ido demasiado lejos, lo pierdo
        if (distanceToTarget > detectionRange)
        {
            SaveLastKnownPlayerPosition(currentTargetPlayer.transform.position);
            currentTargetPlayer = null;
            CancelReposition();
            return true;
        }

        // 5. Mientras haya combate, el enemigo siempre mira al jugador
        LookAtPosition(currentTargetPlayer.transform.position);

        // 6. Si el jugador está demasiado cerca, intento retroceder un poco
        if (distanceToTarget < tooCloseDistance)
        {
            CancelReposition();
            MoveAwayFromPlayer(currentCar, currentTargetPlayer.transform.position);
            return true;
        }

        // 7. Si estoy recolocándome, cada frame compruebo si ya tengo línea de tiro.
        // Si ahora ya puedo disparar, cancelo la recolocación y disparo.
        if (isRepositioning)
        {
            bool hasClearShotWhileRepositioning = HasClearShotToPlayer(currentTargetPlayer);

            if (hasClearShotWhileRepositioning && currentShootCooldown <= 0f)
            {
                CancelReposition();
                StopMoving();

                PlayerMovement playerToShoot = GetNextTargetPlayer(currentCar, lastPlayerShot);

                if (playerToShoot == null)
                {
                    playerToShoot = currentTargetPlayer;
                }

                Shoot(playerToShoot);

                lastPlayerShot = playerToShoot;
                currentTargetPlayer = playerToShoot;
                currentShootCooldown = shootCooldown;

                return true;
            }

            // Si ya he llegado a la posición de recolocación, dejo de estar recolocándome
            if (HasReachedDestination())
            {
                CancelReposition();
            }
        }

        // 8. Si todavía tengo cooldown, no quiero que el enemigo se vuelva loco moviéndose.
        // Se queda quieto, mirando al jugador.
        if (currentShootCooldown > 0f)
        {
            StopMoving();
            return true;
        }

        // 9. Si ya puedo disparar, compruebo si tengo tiro claro
        bool hasClearShot = HasClearShotToPlayer(currentTargetPlayer);

        if (hasClearShot)
        {
            CancelReposition();
            StopMoving();

            PlayerMovement playerToShoot = GetNextTargetPlayer(currentCar, lastPlayerShot);

            if (playerToShoot == null)
            {
                playerToShoot = currentTargetPlayer;
            }

            Shoot(playerToShoot);

            lastPlayerShot = playerToShoot;
            currentTargetPlayer = playerToShoot;
            currentShootCooldown = shootCooldown;

            return true;
        }

        // 10. Si no tengo línea de tiro y no estoy ya recolocándome, busco una posición nueva
        if (!isRepositioning)
        {
            bool foundNewPosition = TryFindBetterAttackPosition(currentCar, currentTargetPlayer.transform.position);

            if (foundNewPosition)
            {
                isRepositioning = true;
                MoveToPosition(currentRepositionTarget);
                return true;
            }
            else
            {
                // Si no encuentro ningún punto bueno, me quedo quieto.
                StopMoving();
                return true;
            }
        }

        // 11. Si ya estoy recolocándome, sigo yendo hacia ese mismo punto.
        MoveToPosition(currentRepositionTarget);
        return true;
    }
    
    public void OnPlayerLeftCar(PlayerMovement player, Vector3 lastPosition)
    {
        if (currentTargetPlayer == player)
        {
            SaveLastKnownPlayerPosition(lastPosition);
            currentTargetPlayer = null;
            CancelReposition();
        }
    }
    
    public void OnPlayerFell(PlayerMovement player)
    {
        if (currentTargetPlayer == player)
        {
            currentTargetPlayer = null;
            currentAimLastPositionTimer = 0f;
            CancelReposition();
        }
    }

    public void ClearCombatData()
    {
        currentTargetPlayer = null;
        lastPlayerShot = null;
        currentAimLastPositionTimer = 0f;
        CancelReposition();
    }
    
    // GESTIÓN DE OBJETIVOS

    private PlayerMovement GetNextTargetPlayer(TrainCarZone currentCar, PlayerMovement playerToAvoid)
    {
        List<PlayerMovement> playersInRange = currentCar.GetPlayersInRange(transform.position, detectionRange);

        if (playersInRange.Count == 0)
        {
            return null;
        }

        if (playersInRange.Count == 1)
        {
            return playersInRange[0];
        }

        for (int i = 0; i < playersInRange.Count; i++)
        {
            if (playersInRange[i] != playerToAvoid)
            {
                return playersInRange[i];
            }
        }

        return playersInRange[0];
    }

    private bool IsPlayerStillInsideCar(TrainCarZone currentCar, PlayerMovement player)
    {
        if (player == null)
        {
            return false;
        }

        List<PlayerMovement> playersInsideCar = currentCar.GetPlayersInsideCar();

        return playersInsideCar.Contains(player);
    }
    
    private void SaveLastKnownPlayerPosition(Vector3 playerPosition)
    {
        lastKnownPlayerPosition = playerPosition;
        currentAimLastPositionTimer = aimLastPositionTime;
    }
    
    // DISPARO

    private void Shoot(PlayerMovement targetPlayer)
    {
        if (targetPlayer == null)
        {
            return;
        }

        if (outlawBulletPrefab == null)
        {
            return;
        }

        if (shootPoint == null)
        {
            return;
        }

        Vector3 shootDirection = (targetPlayer.transform.position - shootPoint.position).normalized;
        shootDirection.y = 0f;

        GameObject bulletObject = Instantiate(outlawBulletPrefab, shootPoint.position, Quaternion.identity);

        OutlawBullet outlawBullet = bulletObject.GetComponent<OutlawBullet>();

        outlawBullet.shootDirection = shootDirection;
    }

    private bool HasClearShotToPlayer(PlayerMovement targetPlayer)
    {
        if (targetPlayer == null || shootPoint == null)
        {
            return false;
        }

        Vector3 shootDirection = (targetPlayer.transform.position - shootPoint.position).normalized;
        float distanceToPlayer = Vector3.Distance(shootPoint.position, targetPlayer.transform.position);

        if (Physics.Raycast(shootPoint.position, shootDirection, distanceToPlayer, obstacleMask))
        {
            return false;
        }

        return true;
    }

    private bool HasClearShotFromPosition(Vector3 fromPosition, Vector3 targetPosition)
    {
        // Levantamos un poco el origen del raycast para que no choque con el suelo
        Vector3 rayOrigin = fromPosition + Vector3.up * 0.5f;

        Vector3 directionToTarget = (targetPosition - rayOrigin).normalized;
        float distanceToTarget = Vector3.Distance(rayOrigin, targetPosition);

        if (Physics.Raycast(rayOrigin, directionToTarget, distanceToTarget, obstacleMask))
        {
            return false;
        }

        return true;
    }
    
    // MOVIMIENTO

    private bool TryFindBetterAttackPosition(TrainCarZone currentCar, Vector3 targetPosition)
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

        for (int i = 0; i < possibleDirections.Length; i++)
        {
            // Buscamos posiciones alrededor del jugador, a la distancia ideal de ataque
            Vector3 candidatePosition = targetPosition + possibleDirections[i] * attackDistance;

            // Si esa posición se sale del vagón, no nos vale
            if (!currentCar.ContainsPoint(candidatePosition))
            {
                continue;
            }

            NavMeshHit navMeshHit;

            if (NavMesh.SamplePosition(candidatePosition, out navMeshHit, 1.5f, NavMesh.AllAreas))
            {
                // Si desde esta nueva posición sí habría línea de tiro, nos la quedamos
                if (HasClearShotFromPosition(navMeshHit.position, targetPosition))
                {
                    currentRepositionTarget = navMeshHit.position;
                    return true;
                }
            }
        }

        return false;
    }

    private void MoveAwayFromPlayer(TrainCarZone currentCar, Vector3 playerPosition)
    {
        if (currentCar == null)
        {
            return;
        }

        Vector3 directionAwayFromPlayer = (transform.position - playerPosition).normalized;
        Vector3 desiredPosition = transform.position + directionAwayFromPlayer * 2f;

        if (!currentCar.ContainsPoint(desiredPosition))
        {
            StopMoving();
            return;
        }

        NavMeshHit navMeshHit;

        if (NavMesh.SamplePosition(desiredPosition, out navMeshHit, 1.5f, NavMesh.AllAreas))
        {
            MoveToPosition(navMeshHit.position);
        }
        else
        {
            StopMoving();
        }
    }

    private void MoveToPosition(Vector3 targetPosition)
    {
        if (navMeshAgent == null)
        {
            return;
        }

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(targetPosition);
    }

    private void StopMoving()
    {
        if (navMeshAgent == null)
        {
            return;
        }

        navMeshAgent.isStopped = true;
    }
    
    private void CancelReposition()
    {
        isRepositioning = false;
        currentRepositionTarget = Vector3.zero;
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

    private void LookAtPosition(Vector3 targetPosition)
    {
        Vector3 lookDirection = targetPosition - transform.position;
        lookDirection.y = 0f;

        if (lookDirection == Vector3.zero)
        {
            return;
        }

        transform.forward = lookDirection.normalized;
    }
}
