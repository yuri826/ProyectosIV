using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrainCarZone : MonoBehaviour
{
    [Header("Enemy Spawn Points")]
    [SerializeField] private Transform[] outlawSpawnPoints;

    [Header("Sabotage Points")]
    [SerializeField] private SabotagePoint[] sabotagePoints;

    [Header("Patrol Points")]
    [SerializeField] private Transform[] patrolPoints;

    [Header("Safe Points")]
    [SerializeField] private Transform[] safePoints;

    [Header("Broken Points Limit")]
    [SerializeField] private int maxBrokenPoints = 5;

    [Header("Players in Car")]
    [SerializeField] private List<PlayerMovement> playersInsideCar = new List<PlayerMovement>();

    [Header("Player Respawn")]
    [SerializeField] private Transform playerRespawnPoint;

    private Collider zoneCollider;

    private void Awake()
    {
        zoneCollider = GetComponent<Collider>();
    }

    public Transform[] GetSpawnPoints()
    {
        return outlawSpawnPoints;
    }

    public SabotagePoint[] GetSabotagePoints()
    {
        return sabotagePoints;
    }

    public List<PlayerMovement> GetPlayersInsideCar()
    {
        RefreshPlayersInsideCar();
        RemoveNullPlayers();
        return playersInsideCar;
    }

    public Transform GetPlayerRespawnPoint()
    {
        return playerRespawnPoint;
    }

    public int GetBrokenPointsCount()
    {
        int brokenPointsCount = 0;

        for (int i = 0; i < sabotagePoints.Length; i++)
        {
            if (sabotagePoints[i].IsBroken())
            {
                brokenPointsCount++;
            }
        }

        return brokenPointsCount;
    }

    public bool CanBreakMorePoints()
    {
        return GetBrokenPointsCount() < maxBrokenPoints;
    }

    public SabotagePoint GetRandomFreeSabotagePoint()
    {
        if (!CanBreakMorePoints())
        {
            return null;
        }

        List<SabotagePoint> freePoints = new List<SabotagePoint>();

        for (int i = 0; i < sabotagePoints.Length; i++)
        {
            if (sabotagePoints[i].CanBeTargeted())
            {
                freePoints.Add(sabotagePoints[i]);
            }
        }

        if (freePoints.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, freePoints.Count);
        SabotagePoint selectedPoint = freePoints[randomIndex];

        bool pointReserved = selectedPoint.ReservePoint();

        if (!pointReserved)
        {
            return null;
        }

        return selectedPoint;
    }

    public bool HasPlayersInside()
    {
        RefreshPlayersInsideCar();
        RemoveNullPlayers();
        return playersInsideCar.Count > 0;
    }

    public List<PlayerMovement> GetPlayersInRange(Vector3 centerPoint, float range)
    {
        RefreshPlayersInsideCar();
        RemoveNullPlayers();

        List<PlayerMovement> playersInRange = new List<PlayerMovement>();

        for (int i = 0; i < playersInsideCar.Count; i++)
        {
            float distanceToPlayer = Vector3.Distance(centerPoint, playersInsideCar[i].transform.position);

            if (distanceToPlayer <= range)
            {
                playersInRange.Add(playersInsideCar[i]);
            }
        }

        return playersInRange;
    }

    public Vector3 GetRandomPatrolPointFarFrom(Vector3 originPoint, float minDistance)
    {
        List<Vector3> validPoints = new List<Vector3>();

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float distance = Vector3.Distance(originPoint, patrolPoints[i].position);

            if (distance >= minDistance)
            {
                validPoints.Add(patrolPoints[i].position);
            }
        }

        if (validPoints.Count == 0)
        {
            return transform.position;
        }

        int randomIndex = Random.Range(0, validPoints.Count);
        return validPoints[randomIndex];
    }

    public bool TryGetSafePointFarFromExplosion(
        Vector3 dangerOrigin,
        float minDistanceFromOrigin,
        float minSafeDistanceFromExplosion,
        out Vector3 safePoint
    )
    {
        safePoint = transform.position;

        List<Vector3> validPoints = new List<Vector3>();

        for (int i = 0; i < safePoints.Length; i++)
        {
            Vector3 candidate = safePoints[i].position;

            float distanceFromOrigin = Vector3.Distance(candidate, dangerOrigin);
            if (distanceFromOrigin < minDistanceFromOrigin)
            {
                continue;
            }

            float distanceFromExplosion = Vector3.Distance(candidate, dangerOrigin);
            if (distanceFromExplosion < minSafeDistanceFromExplosion)
            {
                continue;
            }

            validPoints.Add(candidate);
        }

        if (validPoints.Count == 0)
        {
            return false;
        }

        safePoint = validPoints[Random.Range(0, validPoints.Count)];
        return true;
    }

    public bool ContainsPoint(Vector3 worldPoint)
    {
        return zoneCollider.bounds.Contains(worldPoint);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null && !playersInsideCar.Contains(player))
        {
            playersInsideCar.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null && playersInsideCar.Contains(player))
        {
            playersInsideCar.Remove(player);
        }
    }

    public bool TryGetRandomNavMeshPointInCar(Vector3 originPoint, float minDistance, out Vector3 randomPoint)
    {
        randomPoint = transform.position;

        for (int i = 0; i < 20; i++)
        {
            Vector3 candidatePoint = new Vector3(
                Random.Range(zoneCollider.bounds.min.x, zoneCollider.bounds.max.x),
                transform.position.y,
                Random.Range(zoneCollider.bounds.min.z, zoneCollider.bounds.max.z)
            );

            if (Vector3.Distance(originPoint, candidatePoint) < minDistance)
            {
                continue;
            }

            NavMeshHit navMeshHit;
            if (!NavMesh.SamplePosition(candidatePoint, out navMeshHit, 1.5f, NavMesh.AllAreas))
            {
                continue;
            }

            if (!ContainsPoint(navMeshHit.position))
            {
                continue;
            }

            randomPoint = navMeshHit.position;
            return true;
        }

        return false;
    }

    private void RefreshPlayersInsideCar()
    {
        playersInsideCar.Clear();

        Vector3 center = zoneCollider.bounds.center;
        Vector3 halfExtents = zoneCollider.bounds.extents;

        Collider[] overlaps = Physics.OverlapBox(center, halfExtents, Quaternion.identity);

        for (int i = 0; i < overlaps.Length; i++)
        {
            PlayerMovement player = overlaps[i].GetComponent<PlayerMovement>();

            if (player == null)
            {
                continue;
            }

            if (!playersInsideCar.Contains(player))
            {
                playersInsideCar.Add(player);
            }
        }
    }

    private void RemoveNullPlayers()
    {
        for (int i = playersInsideCar.Count - 1; i >= 0; i--)
        {
            if (playersInsideCar[i] == null)
            {
                playersInsideCar.RemoveAt(i);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (patrolPoints != null)
        {
            Gizmos.color = Color.cyan;

            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.2f);
                }
            }
        }

        if (safePoints != null)
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < safePoints.Length; i++)
            {
                if (safePoints[i] != null)
                {
                    Gizmos.DrawCube(safePoints[i].position, Vector3.one * 0.25f);
                }
            }
        }
    }
}
