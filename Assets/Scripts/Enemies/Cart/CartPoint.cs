using System;
using UnityEngine;

[Serializable]
public class CartPoint
{
    [field: SerializeField] public Transform AppearPoint { get; private set; }
    [field: SerializeField] public Transform TransformPoint { get; private set; }
    [field: SerializeField] public TrainCarZone TrainCarZone { get; private set; }
    [field: SerializeField] public bool IsTaken { get; set; }
}
