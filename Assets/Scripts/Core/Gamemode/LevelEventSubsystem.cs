using System;
using UnityEngine;

[Serializable]
public class LevelEventSubsystem
{
    [SerializeField] private LevelEventInfo[] levelEvents;
    
    public void UpdateEventTimeline(int currentLevelTime)
    {
        foreach (var t in levelEvents)
        {
            if (t.HasBeenExecuted) continue;

            if (t.execTime == currentLevelTime)
            {
                t.Execute();
            }
        }
    }
}
