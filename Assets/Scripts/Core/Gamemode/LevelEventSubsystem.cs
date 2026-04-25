using System;
using UnityEngine;

[Serializable]
public class LevelEventSubsystem
{
    [SerializeField] private LevelEventInfo[] levelEvents;
    
    public void UpdateEventTimeline(int currentLevelTime)
    {
        //Mira que eventos son ejecutables en el tiempo
        foreach (var t in levelEvents)
        {
            if (t.execTime == currentLevelTime)
            {
                t.levelEvent.ExecuteEvent();
            }
        }
    }
}
