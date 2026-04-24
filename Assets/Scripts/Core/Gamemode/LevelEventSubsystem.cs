using System;
using UnityEngine;

[Serializable]
public class LevelEventSubsystem
{
    [SerializeField] private LevelEventInfo[] levelEvents;
    
    public void UpdateEventTimeline(int currentLevelTime)
    {
        for (int i = 0; i < levelEvents.Length; i++)
        {
            if (levelEvents[i].execTime == currentLevelTime)
            {
                levelEvents[i].levelEvent.ExecuteEvent();
            }
        }
    }
}
