using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEnvironmentScroll : MonoBehaviour
{
    private List<EnvironmentObject> levelObjects = new List<EnvironmentObject>();
    private float minSpawnDelay = 0.2f;
    private float maxSpawnDelay = 2f;

    [Header("Movement")] 
    [SerializeField] private float moveLerpQ;
    [SerializeField] private float lowSpeed = 25f;
    [SerializeField] private float middleSpeed = 35f;
    [SerializeField] private float highSpeed = 45f;
    private float currentSpeed = 0;
    [SerializeField] private int xMinPos;
    [SerializeField] private int xSpawnPos;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out EnvironmentObject obj))
            {
                levelObjects.Add(obj);
            }
        }
    }

    private void Update()
    {
        currentSpeed = TrainGameMode.instance.GetSpeedManager().GetCurrentSpeedState() switch
        {
            SpeedState.High => Mathf.Lerp(currentSpeed, highSpeed, Time.deltaTime * moveLerpQ),
            SpeedState.Low => Mathf.Lerp(currentSpeed, lowSpeed, Time.deltaTime * moveLerpQ),
            SpeedState.Middle => Mathf.Lerp(currentSpeed, middleSpeed, Time.deltaTime * moveLerpQ),
            _ => currentSpeed
        };

        foreach (EnvironmentObject obj in levelObjects)
        {
            obj.Move(currentSpeed, xMinPos, xSpawnPos);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(xMinPos, 0, 0), 1);
        
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector3(xSpawnPos, 0, 0), 1);
    }
}