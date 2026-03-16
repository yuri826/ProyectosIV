using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class TrainCarZone : MonoBehaviour
{
    [Header("PEnemy Spawn Points")]
    [SerializeField] private Transform[] outlawSpawnPoints;
    [SerializeField] private GameObject outlawObject;

    [Header("Sabotage Points")]
    // Aquí meteremos a mano todas las casillas sabotables del vagón
    [SerializeField] private SabotagePoint[] sabotagePoints;

    [Header("Broken Points Limit")]
    // Máximo de puntos rotos permitidos a la vez en este vagón
    [SerializeField] private int maxBrokenPoints = 5;

    [Header("Players in Car")]
    // Esta lista se rellena sola cuando jugadores entran y salen del vagón
    [SerializeField] private List<PlayerMovement> playersInsideCar = new List<PlayerMovement>();
    
    private Collider zoneCollider;
    
    private void Awake()
    {
        zoneCollider = GetComponent<Collider>();
    }
    
    // MÉTODOS DE ACCESO BÁSICOS

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
    
    // MÉTODOS PARA GESTIONAR PUNTOS SABOTABLES

    public int GetBrokenPointsCount()
    {
        int brokenPointsCount = 0;

        for (int i = 0; i < sabotagePoints.Length; i++)
        {
            if (sabotagePoints[i] == null)
            {
                continue;
            }

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
        // Si ya hemos llegado al máximo de puntos rotos, no devolvemos ninguno
        if (!CanBreakMorePoints())
        {
            return null;
        }

        List<SabotagePoint> freePoints = new List<SabotagePoint>();

        for (int i = 0; i < sabotagePoints.Length; i++)
        {
            if (sabotagePoints[i] == null)
            {
                continue;
            }

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

        // Lo reservamos aquí mismo para que otro enemigo no elija el mismo punto
        bool pointReserved = selectedPoint.ReservePoint();

        if (!pointReserved)
        {
            return null;
        }

        return selectedPoint;
    }
    
    // MÉTODOS PARA JUGADORES DENTRO DEL VAGÓN

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
    
    // MÉTODOS DE POSICIONES RANDOM DEL VAGÓN

    public Vector3 GetRandomPointInCarFarFrom(Vector3 originPoint, float minDistance)
    {
        List<Vector3> validPoints = new List<Vector3>();

        // Podemos usar los spawn points como posibles puntos de paseo
        for (int i = 0; i < outlawSpawnPoints.Length; i++)
        {
            if (outlawSpawnPoints[i] == null)
            {
                continue;
            }

            float distance = Vector3.Distance(originPoint, outlawSpawnPoints[i].position);

            if (distance >= minDistance)
            {
                validPoints.Add(outlawSpawnPoints[i].position);
            }
        }

        // También podemos usar los puntos sabotables como posibles puntos de paseo
        for (int i = 0; i < sabotagePoints.Length; i++)
        {
            if (sabotagePoints[i] == null)
            {
                continue;
            }

            float distance = Vector3.Distance(originPoint, sabotagePoints[i].transform.position);

            if (distance >= minDistance)
            {
                validPoints.Add(sabotagePoints[i].transform.position);
            }
        }

        if (validPoints.Count == 0)
        {
            // Si no encuentra ninguno suficientemente lejos, devolvemos el centro del vagón
            return transform.position;
        }

        int randomIndex = Random.Range(0, validPoints.Count);
        return validPoints[randomIndex];
    }


    public bool ContainsPoint(Vector3 worldPoint)
    {
        if (zoneCollider == null)
        {
            return false;
        }

        return zoneCollider.bounds.Contains(worldPoint);
    }
    
    // DETECCIÓN DE JUGADORES

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null)
        {
            if (!playersInsideCar.Contains(player))
            {
                playersInsideCar.Add(player);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null)
        {
            if (playersInsideCar.Contains(player))
            {
                playersInsideCar.Remove(player);
            }
        }
    }
    
    public bool TryGetRandomNavMeshPointInCar(Vector3 originPoint, float minDistance, out Vector3 randomPoint)
    {
        randomPoint = transform.position;

        // Hacemos varios intentos para encontrar un punto válido dentro del vagón
        for (int i = 0; i < 20; i++)
        {
            // Cogemos un punto random dentro de los bounds del collider del vagón
            Vector3 candidatePoint = new Vector3(
                Random.Range(zoneCollider.bounds.min.x, zoneCollider.bounds.max.x),
                transform.position.y,
                Random.Range(zoneCollider.bounds.min.z, zoneCollider.bounds.max.z)
            );

            // Si está demasiado cerca del origen, no nos vale
            if (Vector3.Distance(originPoint, candidatePoint) < minDistance)
            {
                continue;
            }

            // Si no cae dentro del NavMesh, no nos vale
            NavMeshHit navMeshHit;
            if (!NavMesh.SamplePosition(candidatePoint, out navMeshHit, 1.5f, NavMesh.AllAreas))
            {
                continue;
            }

            // Comprobamos que el punto final sigue estando dentro del vagón
            if (!ContainsPoint(navMeshHit.position))
            {
                continue;
            }

            randomPoint = navMeshHit.position;
            return true;
        }

        return false;
    }
    // LIMPIEZA DE REFERENCIAS

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

    public void SpawnOutlaws()
    {
        foreach (var spawnPoint in outlawSpawnPoints)
        {
            Instantiate(outlawObject, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
