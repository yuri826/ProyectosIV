using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BGObjectManager : MonoBehaviour
{
    [SerializeField] private BGObject[] backgroundObjects;
    [SerializeField] private float baseSpeed;
    private float currentSpeed => baseSpeed * TrainGameMode.instance.GetCurrentSpeed();
    
    [Header("ObjVars")]
    [SerializeField] private Transform spawnPos;
    [SerializeField] private Transform despawnPos;
    [SerializeField] private float[] zSpawnRange;

    [Range(0,5)][SerializeField] private float[] introWaitRange;
    private float introAmount;

    private void Start()
    {
        foreach (BGObject bgObj in backgroundObjects)
        {
            bgObj.introAmount = Random.Range(introWaitRange[0], introWaitRange[1]);
            bgObj.despawnXPos = despawnPos.position.x;
            bgObj.spawnXPos = spawnPos.position.x;
            
            for (var i = 0; i < zSpawnRange.Length; i++)
            {
                bgObj.spawnZLimits[i] = zSpawnRange[i];
            }

            bgObj.MoveToSpawn();
        }
    }

    private void Update()
    {
        foreach (var bgObj in backgroundObjects)
        {
            bgObj.Move(currentSpeed);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(new Vector3(spawnPos.position.x, 0, zSpawnRange[0]), 1);
        Gizmos.DrawSphere(new Vector3(spawnPos.position.x, 0, zSpawnRange[1]), 1);
    }
}
