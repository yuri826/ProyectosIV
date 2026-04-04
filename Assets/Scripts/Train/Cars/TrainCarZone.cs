using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrainCarZone : MonoBehaviour
{
    [Header("Enemy Spawn Points")]
    [SerializeField] private Transform[] outlawSpawnPoints;

    [Header("Sabotage Points")]
    [SerializeField] private SabotagePoint[] sabotagePoints;

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
        RemoveNullPlayers();
        return playersInsideCar.Count > 0;
    }

    public List<PlayerMovement> GetPlayersInRange(Vector3 centerPoint, float range)
    {
        RemoveNullPlayers();

        List<PlayerMovement> playersInRange = new List<PlayerMovement>();

        for (int i = 0; i < playersInsideCar.Count; i++)
        {
            if (playersInsideCar[i] == null)
            {
                continue;
            }

            float distanceToPlayer = Vector3.Distance(centerPoint, playersInsideCar[i].transform.position);

            if (distanceToPlayer <= range)
            {
                playersInRange.Add(playersInsideCar[i]);
            }
        }

        return playersInRange;
    }

    public Vector3 GetRandomPointInCarFarFrom(Vector3 originPoint, float minDistance)
    {
        List<Vector3> validPoints = new List<Vector3>();

        for (int i = 0; i < outlawSpawnPoints.Length; i++)
        {
            float distance = Vector3.Distance(originPoint, outlawSpawnPoints[i].position);

            if (distance >= minDistance)
            {
                validPoints.Add(outlawSpawnPoints[i].position);
            }
        }

        for (int i = 0; i < sabotagePoints.Length; i++)
        {
            float distance = Vector3.Distance(originPoint, sabotagePoints[i].transform.position);

            if (distance >= minDistance)
            {
                validPoints.Add(sabotagePoints[i].transform.position);
            }
        }

        if (validPoints.Count == 0)
        {
            return transform.position;
        }

        int randomIndex = Random.Range(0, validPoints.Count);
        return validPoints[randomIndex];
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
}
