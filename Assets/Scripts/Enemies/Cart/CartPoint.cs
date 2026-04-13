using System;
using UnityEngine;

[Serializable]
public class CartPoint
{
    [field : SerializeField]
    public Transform TransformPoint { get; set; }
    [field : SerializeField]
    public bool IsTaken { get; set; }
}
