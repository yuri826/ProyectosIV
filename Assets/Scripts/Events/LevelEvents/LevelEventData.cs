using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelEvent", menuName = "ScriptableObjects/LevelEvent")]
[Serializable]
public class LevelEventData : ScriptableObject
{
    [Header("Event Type")]
    public LevelEventType eventType;

    [Tooltip("Activation Time Moment")]
    public int triggerTime;

    public GameObject iconPrefab;

    [Header("Outlaw Wave")]
    public int outlawCount;

    [Header("Event Duration")]
    public float duration;

    [HideInInspector]
    public bool hasTriggered { get; set; }= false;
}
