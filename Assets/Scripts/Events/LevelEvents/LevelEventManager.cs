using System.Collections.Generic;
using UnityEngine;

public class LevelEventManager : MonoBehaviour
{
    [Header("Level Events")]
    [SerializeField] private List<LevelEventInstance> levelEvents = new List<LevelEventInstance>();

    [Header("External Systems")]
    [SerializeField] private TrainSpawnDirector trainSpawnDirector;
    [SerializeField] private SandstormSystem sandstormSystem;

    public void UpdateEventTimeline(int currentLevelTime)
    {
        for (int i = 0; i < levelEvents.Count; i++)
        {
            if (levelEvents[i].hasTriggered)
            {
                continue;
            }

            if (currentLevelTime < levelEvents[i].timeToSpawn)
            {
                continue;
            }

            TriggerEvent(levelEvents[i].eventData);
            levelEvents[i].eventData.hasTriggered = true;
        }
    }

    private void TriggerEvent(LevelEventData eventData)
    {
        switch (eventData.eventType)
        {
            case LevelEventType.OutlawWave:
                TriggerOutlawWave(eventData);
                break;

            case LevelEventType.Sandstorm:
                TriggerSandstorm(eventData);
                break;
        }
    }

    private void TriggerOutlawWave(LevelEventData eventData)
    {
        trainSpawnDirector.SpawnOutlawWave(eventData.outlawCount);
    }

    private void TriggerSandstorm(LevelEventData eventData)
    {
        sandstormSystem.StartSandstorm(eventData.duration);
    }
    
    public List<LevelEventData> GetLevelEvents()
    {
        List<LevelEventData> events = new List<LevelEventData>();

        foreach (var myEvent in levelEvents)
        {
            events.Add(myEvent.eventData);
        }
        return events;
    }
}
