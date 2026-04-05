using System;
using UnityEngine;

[Serializable]
public class LevelEventInstance
{
    [field:SerializeField] public LevelEventData eventData { get; set; }
    [field:SerializeField] public int timeToSpawn { get; set; }

    public bool hasTriggered => eventData.hasTriggered;

    public void ExecuteEvent()
    {
        eventData.Execute();
    }
}
