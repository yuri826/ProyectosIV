using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class TrainCarZone : MonoBehaviour
{
    //Un header por variable no sirve de mucho, porque para eso ya está el propio nombre de la variable
    
    [Header("Points")]
    [field: SerializeField] public Transform[] outlawSpawnPoints { get; private set; }
    [SerializeField] private SabotagePoint[] sabotagePoints;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Transform[] safePoints;
    [field: SerializeField] public CollapseRockSpawnPoint[] collapseSpawnPoints { get; private set; }
    [SerializeField] private int maxBrokenPoints = 5;
    [SerializeField] private Transform arrowVolleyPoint;

    [Header("Player")]
    [SerializeField] private List<PlayerMovement> playersInsideCar = new List<PlayerMovement>();
    [field: SerializeField] public Transform playerRespawnPoint { get; private set; }
    
    [Header("ArrowVolley")]
    private List<ArrowVolleyProjectile> arrowVolleyProjectiles = new List<ArrowVolleyProjectile>();
    [SerializeField] private Transform arrowPoolPos;

    private Collider zoneCollider;

    private void Awake()
    {
        zoneCollider = GetComponent<Collider>();

        foreach (Transform arrow in arrowPoolPos)
        {
            ArrowVolleyProjectile arrowComponent = arrow.GetComponent<ArrowVolleyProjectile>();
            arrowVolleyProjectiles.Add(arrowComponent);
            arrowComponent.poolPos = arrowPoolPos;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null && !playersInsideCar.Contains(player)) playersInsideCar.Add(player);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null && playersInsideCar.Contains(player)) playersInsideCar.Remove(player);
    }

    #region Point management

    public Vector3 GetRandomPatrolPointFarFrom(Vector3 originPoint, float minDistance)
    {
        List<Vector3> validPoints = new List<Vector3>();

        foreach (var point in patrolPoints)
        {
            float distance = Vector3.Distance(originPoint, point.position);

            if (distance >= minDistance)
            {
                validPoints.Add(point.position);
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

        foreach (var point in safePoints)
        {
            Vector3 candidate = point.position;

            float distanceFromOrigin = Vector3.Distance(candidate, dangerOrigin);
            if (distanceFromOrigin < minDistanceFromOrigin) continue;

            float distanceFromExplosion = Vector3.Distance(candidate, dangerOrigin);
            if (distanceFromExplosion < minSafeDistanceFromExplosion) continue;

            validPoints.Add(candidate);
        }

        if (validPoints.Count == 0) return false;

        safePoint = validPoints[Random.Range(0, validPoints.Count)];
        return true;
    }

    public bool ContainsPoint(Vector3 worldPoint)
    {
        return zoneCollider.bounds.Contains(worldPoint);
    }
    
    private int GetBrokenPointsCount()
    {
        int brokenPointsCount = 0;

        foreach (var point in sabotagePoints)
        {
            if (point.IsBroken())
            {
                brokenPointsCount++;
            }
        }

        return brokenPointsCount;
    }

    private bool CanBreakMorePoints()
    {
        return GetBrokenPointsCount() < maxBrokenPoints;
    }

    public SabotagePoint GetRandomFreeSabotagePoint()
    {
        if (!CanBreakMorePoints()) return null;

        List<SabotagePoint> freePoints = new List<SabotagePoint>();

        foreach (var point in sabotagePoints)
        {
            if (point.CanBeTargeted())
            {
                freePoints.Add(point);
            }
        }

        if (freePoints.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, freePoints.Count);
        SabotagePoint selectedPoint = freePoints[randomIndex];

        bool pointReserved = selectedPoint.ReservePoint();

        if (!pointReserved) return null;

        return selectedPoint;
    }
    
    #endregion
    
    #region Player in cart management
    
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
    
    public List<PlayerMovement> GetPlayersInsideCar()
    {
        RefreshPlayersInsideCar();
        RemoveNullPlayers();
        return playersInsideCar;
    }

    private void RefreshPlayersInsideCar()
    {
        playersInsideCar.Clear();

        Vector3 center = zoneCollider.bounds.center;
        Vector3 halfExtents = zoneCollider.bounds.extents;

        Collider[] overlaps = Physics.OverlapBox(center, halfExtents, Quaternion.identity);

        foreach (var col in overlaps)
        {
            PlayerMovement player = col.GetComponent<PlayerMovement>();

            if (player is null) continue;

            if (!playersInsideCar.Contains(player)) playersInsideCar.Add(player);
        }
    }

    private void RemoveNullPlayers()
    {
        for (int i = playersInsideCar.Count - 1; i >= 0; i--)
        {
            if (playersInsideCar[i] is null)
            {
                playersInsideCar.RemoveAt(i);
            }
        }
    }
    
    #endregion

    public void SpawnArrowVolley(ArrowVolleyDirection dir)
    {
        print("shootCart");
        
        Vector3 shootDir = Vector3.down;
        int zOffset = 0;
        
        switch (dir)
        {
            case ArrowVolleyDirection.TopToBottom:
                shootDir = -Vector3.forward;
                zOffset = -8;
                break;
            case ArrowVolleyDirection.BottomToTop:
                shootDir = Vector3.forward;
                break;
        }
        
        int maxX = 8;
        Vector3 nnapaOffsetX = new Vector3(4f,7,-4);

        int loopIndex = 0;
        
        for (float i = maxX*-0.5f; i < maxX*0.5f; i ++)
        {
            Vector3 initPoint = arrowVolleyPoint.transform.position - new Vector3(maxX * 0.7f + i + 2.3f,
                8,
                zOffset) + nnapaOffsetX;
            
            print("zOffset: " + zOffset);

            if (loopIndex >= arrowVolleyProjectiles.Count) return;
            
            arrowVolleyProjectiles[loopIndex].Shoot(shootDir, initPoint);
            
            loopIndex++;
        }

        throw new AbandonedMutexException();
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
        
        #region ArrowVolleyPoints
        
        Gizmos.color = Color.cyan;

        float zOffset = 1;
        int maxX = 8;
        int maxZ = 5;
        Vector3 nnapaOffsetX = new Vector3(4f,7,-4);
        Vector3 nnapaOffsetZ = new Vector3(-maxX*0.5f,7,-4);
        
        for (float i = maxX*-0.5f; i < maxX*0.5f; i ++)
        {
            Vector3 initPoint = arrowVolleyPoint.transform.position - new Vector3(maxX * 0.7f + i * 2.3f,
                0,
                0) + nnapaOffsetX;
            Gizmos.color = Color.lawnGreen;
            Gizmos.DrawCube(initPoint, Vector3.one);
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(initPoint, Vector3.forward*maxX);
        }
        
        for (float i = maxX*-0.5f; i < maxX*0.5f; i ++)
        {
            Vector3 initPoint = arrowVolleyPoint.transform.position - new Vector3(maxX * 0.7f + i * 2.3f + 0.1f,
                0,
                -9) + nnapaOffsetX;
            Gizmos.color = Color.lightGreen;
            Gizmos.DrawCube(initPoint, Vector3.one);
            Gizmos.color = Color.blueViolet;
            Gizmos.DrawRay(initPoint, -Vector3.forward*maxX);
        }
        
        for (float i = maxX*-0.5f; i < maxX*0.5f; i ++)
        {
            Vector3 initPoint = arrowVolleyPoint.transform.position - new Vector3(maxX * 0.7f + i * 2.3f,
                0,
                zOffset) + nnapaOffsetX;
            
            Gizmos.color = Color.aquamarine;
            Gizmos.DrawCube(initPoint, Vector3.one);
        }
        
        // for (int i = 0; i < maxZ; i ++)
        // {
        //     Vector3 initPoint = arrowVolleyPoint.transform.position + new Vector3(
        //         -maxX*0.7f,
        //         0,
        //         maxZ*0.6f + i) + nnapaOffsetZ;
        //     
        //     Gizmos.color = Color.lightGreen;
        //     Gizmos.DrawCube(initPoint, Vector3.one);
        //     Gizmos.color = Color.cyan;
        //     Gizmos.DrawRay(initPoint, Vector3.forward*maxZ);
        // }
        
        #endregion
    }
}
