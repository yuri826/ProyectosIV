using System;
using UnityEngine;

public class LevelEventData : ScriptableObject
{
    [field: SerializeField]
    public GameObject iconPrefab { get; set; }
    
    [Header("Event Duration")]
    [field: SerializeField]
    public float duration { get; set; }
    
    public bool hasTriggered { get; set; } = false;

    public virtual void Execute()
    {
        hasTriggered = true;
    }
}
